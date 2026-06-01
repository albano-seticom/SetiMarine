using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SetiMarine.Application.Services;

public class SetiFiscalClient
{
    private readonly HttpClient _http;
    private readonly SetiFiscalOptions _options;
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition      = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        Converters                  = { new JsonStringEnumConverter() }
    };

    public SetiFiscalClient(HttpClient http, SetiFiscalOptions options)
    {
        _http    = http;
        _options = options;
        _http.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
        _http.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
        _http.Timeout = TimeSpan.FromSeconds(60);
    }

    public Task<ResultadoFiscal> EmitirNfceAsync(EmitirNfceRequest req, CancellationToken ct = default)
    {
        req.EmpresaFiscalId ??= _options.EmpresaId;
        return EmitirAsync("api/nfce/emitir", req, ct);
    }

    public Task<ResultadoFiscal> EmitirNfeAsync(EmitirNfeRequest req, CancellationToken ct = default)
    {
        req.EmpresaFiscalId ??= _options.EmpresaId;
        return EmitirAsync("api/nfe/emitir", req, ct);
    }

    public Task<ResultadoFiscal> EmitirNfseAsync(EmitirNfseRequest req, CancellationToken ct = default)
    {
        req.EmpresaFiscalId ??= _options.EmpresaId;
        return EmitirAsync("api/nfse/emitir", req, ct);
    }

    public Task<ResultadoFiscal> CancelarAsync(string chaveAcesso, string justificativa, CancellationToken ct = default)
        => EmitirAsync($"api/documentos/{chaveAcesso}/cancelar", new { justificativa }, ct);

    private async Task<ResultadoFiscal> EmitirAsync(string url, object body, CancellationToken ct)
    {
        HttpResponseMessage resp;
        try
        {
            resp = await _http.PostAsJsonAsync(url, body, _json, ct);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            return ResultadoFiscal.Falha(TipoErroFiscal.ErroConexao,
                "Timeout: o SetiFiscal não respondeu no prazo de 60 segundos.");
        }
        catch (HttpRequestException ex)
        {
            return ResultadoFiscal.Falha(TipoErroFiscal.ErroConexao,
                $"Falha de conexão com o SetiFiscal: {ex.Message}");
        }

        var json = await resp.Content.ReadAsStringAsync(ct);

        return (int)resp.StatusCode switch
        {
            200 => Desserializar(json),
            422 => Classificar422(json),
            400 => Classificar400(json),
            401 => ResultadoFiscal.Falha(TipoErroFiscal.ApiKeyInvalida,
                       "API Key inválida ou ausente."),
            500 => Classificar500(json),
            _   => ResultadoFiscal.Falha(TipoErroFiscal.ErroInterno,
                       $"Resposta inesperada: HTTP {(int)resp.StatusCode}.")
        };
    }

    private ResultadoFiscal Desserializar(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<ResultadoFiscal>(json, _json)
                ?? ResultadoFiscal.Falha(TipoErroFiscal.ErroInterno, "Resposta vazia do SetiFiscal.");
        }
        catch
        {
            return ResultadoFiscal.Falha(TipoErroFiscal.ErroInterno, "Resposta do SetiFiscal não é JSON válido.");
        }
    }

    private ResultadoFiscal Classificar422(string json)
    {
        var resultado = Desserializar(json);
        if (resultado.Sucesso) return resultado;
        resultado.TipoErro = resultado.CodigoSefaz.HasValue
            ? TipoErroFiscal.RejeicaoSefaz
            : TipoErroFiscal.ConfiguracaoEmpresa;
        return resultado;
    }

    private static ResultadoFiscal Classificar400(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var erros = new Dictionary<string, string[]>();
            if (doc.RootElement.TryGetProperty("errors", out var errorsEl))
                foreach (var prop in errorsEl.EnumerateObject())
                    erros[prop.Name] = prop.Value.EnumerateArray()
                        .Select(e => e.GetString() ?? "").ToArray();
            else if (doc.RootElement.TryGetProperty("erro", out var erroEl))
                erros["geral"] = new[] { erroEl.GetString() ?? "Dados inválidos." };

            var resumo = erros.Count > 0
                ? string.Join("; ", erros.SelectMany(kv => kv.Value.Select(v => $"{kv.Key}: {v}")))
                : "Dados inválidos enviados.";

            return new ResultadoFiscal
            {
                Sucesso        = false,
                TipoErro       = TipoErroFiscal.Validacao,
                Erro           = resumo,
                ErrosValidacao = erros,
                ProcessadoEm   = DateTime.UtcNow
            };
        }
        catch
        {
            return ResultadoFiscal.Falha(TipoErroFiscal.Validacao, "Dados inválidos enviados.");
        }
    }

    private static ResultadoFiscal Classificar500(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var msg = doc.RootElement.TryGetProperty("erro", out var e)
                ? e.GetString() ?? "Erro interno no SetiFiscal."
                : "Erro interno no SetiFiscal.";
            return ResultadoFiscal.Falha(TipoErroFiscal.ErroInterno, msg);
        }
        catch
        {
            return ResultadoFiscal.Falha(TipoErroFiscal.ErroInterno, "Erro interno no SetiFiscal.");
        }
    }
}

// ═══════════════════════════════════════════════════════════
// OPÇÕES
// ═══════════════════════════════════════════════════════════

public class SetiFiscalOptions
{
    public string BaseUrl   { get; set; } = "https://setifiscal.seticom.com.br";
    public string ApiKey    { get; set; } = null!;
    public Guid   EmpresaId { get; set; }
}

// ═══════════════════════════════════════════════════════════
// TIPO DE ERRO
// ═══════════════════════════════════════════════════════════

public enum TipoErroFiscal
{
    Validacao,
    ApiKeyInvalida,
    ConfiguracaoEmpresa,
    RejeicaoSefaz,
    ErroInterno,
    ErroConexao,
}

// ═══════════════════════════════════════════════════════════
// DTOs — RESPONSE
// ═══════════════════════════════════════════════════════════

public class ResultadoFiscal
{
    public bool    Sucesso { get; set; }
    public TipoErroFiscal? TipoErro { get; set; }
    public Dictionary<string, string[]>? ErrosValidacao { get; set; }
    public string? ChaveAcesso         { get; set; }
    public string? NumeroNota          { get; set; }
    public string? NumeroNfse          { get; set; }
    public string? Serie               { get; set; }
    public int?    CodigoSefaz         { get; set; }
    public string? MensagemSefaz       { get; set; }
    public string? XmlAutorizadoBase64 { get; set; }
    public string? XmlNfseBase64       { get; set; }
    public string? DanfePdfBase64      { get; set; }
    public string? Erro                { get; set; }
    public DateTime ProcessadoEm      { get; set; }

    public byte[]? ObterDanfePdf() =>
        DanfePdfBase64 is null ? null : Convert.FromBase64String(DanfePdfBase64);

    internal static ResultadoFiscal Falha(TipoErroFiscal tipo, string erro) => new()
    {
        Sucesso      = false,
        TipoErro     = tipo,
        Erro         = erro,
        ProcessadoEm = DateTime.UtcNow
    };
}

// ═══════════════════════════════════════════════════════════
// DTOs — REQUEST
// ═══════════════════════════════════════════════════════════

public class EmitirNfceRequest
{
    public Guid?            EmpresaFiscalId { get; set; }
    public ConsumidorNfce?  Consumidor      { get; set; }
    public List<ItemNota>   Itens           { get; set; } = new();
    public List<PagamentoFiscal>  Pagamentos { get; set; } = new();
    public decimal          ValorTotal      { get; set; }
    public decimal          ValorDesconto   { get; set; } = 0;
    public string?          InformacoesAdicionais { get; set; }
}

public class ConsumidorNfce
{
    public string? CpfCnpj { get; set; }
    public string? Nome    { get; set; }
}

public class EmitirNfeRequest
{
    public Guid?            EmpresaFiscalId  { get; set; }
    public string           NaturezaOperacao { get; set; } = "VENDA DE MERCADORIA";
    public Destinatario     Destinatario     { get; set; } = null!;
    public List<ItemNota>   Itens            { get; set; } = new();
    public List<PagamentoFiscal>  Pagamentos { get; set; } = new();
    public decimal          ValorTotal       { get; set; }
    public decimal          ValorDesconto    { get; set; } = 0;
    public int              ModalidadeFrete  { get; set; } = 9;
    public string?          InformacoesAdicionais { get; set; }
}

public class EmitirNfseRequest
{
    public Guid?     EmpresaFiscalId { get; set; }
    public int       NumeroRps       { get; set; }
    public string    SerieRps        { get; set; } = "1";
    public Tomador   Tomador         { get; set; } = null!;
    public ServicoNfse   Servico     { get; set; } = null!;
}

public class ItemNota
{
    public int     NumeroItem       { get; set; } = 1;
    public string  CodigoProduto    { get; set; } = "001";
    public string  Descricao        { get; set; } = null!;
    public string? Ncm              { get; set; }
    public string  Cfop             { get; set; } = "5102";
    public string  UnidadeComercial { get; set; } = "UN";
    public decimal Quantidade       { get; set; }
    public decimal ValorUnitario    { get; set; }
    public decimal ValorTotal       { get; set; }
    public decimal ValorDesconto    { get; set; } = 0;
    public string? CstOuCsosn       { get; set; }
    public string? OrigemMercadoria { get; set; }
}

public class PagamentoFiscal
{
    /// <summary>01=Dinheiro 03=Crédito 04=Débito 17=PIX 99=Outros</summary>
    public string  FormaPagamento { get; set; } = "01";
    public decimal Valor          { get; set; }
}

public class Destinatario
{
    public string  CpfCnpj         { get; set; } = null!;
    public string  Nome            { get; set; } = null!;
    public string? Email           { get; set; }
    public string  Logradouro      { get; set; } = null!;
    public string  Numero          { get; set; } = null!;
    public string? Complemento     { get; set; }
    public string  Bairro          { get; set; } = null!;
    public string  Municipio       { get; set; } = null!;
    public int     CodigoMunicipio { get; set; }
    public string  Uf              { get; set; } = null!;
    public string  Cep             { get; set; } = null!;
    public string? Telefone        { get; set; }
    public string? Ie              { get; set; }
}

public class Tomador : Destinatario { }

public class ServicoNfse
{
    public string  Descricao     { get; set; } = null!;
    public string  CodigoServico { get; set; } = null!;
    public string  CodigoNbs     { get; set; } = null!;
    public decimal ValorServico  { get; set; }
    public decimal AliquotaIss   { get; set; }
    public decimal ValorIss      { get; set; }
    public bool    IssRetido     { get; set; } = false;
}
