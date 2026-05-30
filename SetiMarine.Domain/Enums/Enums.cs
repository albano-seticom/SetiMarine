namespace SetiMarine.Domain.Enums;

public enum PerfilUsuario      { SuperAdmin = 0, Admin = 1, Operacional = 2 }
public enum TipoVaga           { Agua = 0, Seco = 1 }
public enum StatusVaga         { Livre = 0, Ocupada = 1, EmMovimentacao = 2, Manutencao = 3 }
public enum TipoEmbarcacao     { Lancha = 0, Veleiro = 1, Escuna = 2, JetSki = 3, Outro = 4 }
public enum TipoContrato       { Mensalista = 0, Diaria = 1, Visitante = 2 }
public enum StatusOS           { Pendente = 0, EmExecucao = 1, Finalizado = 2 }

public enum StatusMovimentacao
{
    Agendado = 0, AguardandoPreparo = 1, EmPreparo = 2, Pronto = 3,
    Descendo = 4, NaAgua = 5, Retornando = 6, Subindo = 7, Finalizado = 8
}

public enum TipoServico
{
    LimpezaQuinzenal  = 0,
    LimpezaPosPasseio = 1,
    VistoriaPeriodica = 2,
    ChecklistSaida    = 3,
    ChecklistRetorno  = 4,
    Abastecimento     = 5,
    Outro             = 99,
}

public enum StatusServico
{
    Pendente    = 0,
    EmExecucao  = 1,
    Finalizado  = 2,
    Cancelado   = 3,
}

public enum StatusItemChecklist
{
    Pendente  = 0,
    Ok        = 1,
    Problema  = 2,
    NaoAplica = 3,
}

public enum OrigemPedido
{
    Manual   = 0,
    WhatsApp = 1,
    Portal   = 2,
}

public enum StatusPedidoUso
{
    Recebido   = 0,
    Confirmado = 1,
    EmPreparo  = 2,
    Pronto     = 3,
    Cancelado  = 4,
}

public enum CategoriaProduto
{
    Acessorio   = 0,
    Combustivel = 1,
    Lubrificante = 2,
    Peca        = 3,
    Equipamento = 4,
    Alimento    = 5,
    Outro       = 99,
}

public enum TipoTransacao { Venda = 0, Aluguel = 1 }

public enum StatusAluguel { Ativo = 0, Devolvido = 1, Atrasado = 2 }

public enum TipoUsoEmbarcacao
{
    DescidaNaAgua = 0,
    UsoPier       = 1,
    Passeio       = 2,
    Outro         = 99,
}

public enum StatusAgendamento
{
    Pendente   = 0,
    Confirmado = 1,
    Concluido  = 2,
    Cancelado  = 3,
}

public enum TipoMovimentoEstoque { Entrada = 0, Saida = 1, Ajuste = 2 }

public enum TipoMovimentoCaixa { Entrada = 0, Saida = 1 }

public enum OrigemMovimentoCaixa
{
    VendaProduto       = 0,
    ServicoPedido      = 1,
    MensalidadeContrato = 2,
    Sangria            = 3,
    Suprimento         = 4,
    AjusteManual       = 5,
}

public enum TipoPedido    { Venda = 0, Emprestimo = 1, OrdemServico = 2 }
public enum TipoItemPedido { Venda = 0, Emprestimo = 1 }
public enum StatusPedido
{
    Rascunho    = 0,
    Aberto      = 1,
    EmAndamento = 2,
    Concluido   = 3,
    Cancelado   = 4,
}
