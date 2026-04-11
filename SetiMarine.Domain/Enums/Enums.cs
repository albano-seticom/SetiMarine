namespace SetiMarine.Domain.Enums;

public enum PerfilUsuario  { SuperAdmin = 0, Admin = 1, Operacional = 2 }
public enum TipoVaga       { Agua = 0, Seco = 1 }
public enum StatusVaga     { Livre = 0, Ocupada = 1, EmMovimentacao = 2, Manutencao = 3 }
public enum TipoEmbarcacao { Lancha = 0, Veleiro = 1, Escuna = 2, JetSki = 3, Outro = 4 }
public enum StatusMovimentacao
{
    Agendado = 0, AguardandoPreparo = 1, EmPreparo = 2, Pronto = 3,
    Descendo = 4, NaAgua = 5, Retornando = 6, Subindo = 7, Finalizado = 8
}
public enum TipoContrato { Mensalista = 0, Diaria = 1, Visitante = 2 }
public enum StatusOS     { Pendente = 0, EmExecucao = 1, Finalizado = 2 }
