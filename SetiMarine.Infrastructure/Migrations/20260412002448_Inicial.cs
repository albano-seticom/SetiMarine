using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cfg_configuracoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Chave = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfg_configuracoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cfg_planos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    LimiteEmbarcacoes = table.Column<int>(type: "integer", nullable: false),
                    LimiteUsuarios = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    Destaque = table.Column<bool>(type: "boolean", nullable: false),
                    LinkPagamento = table.Column<string>(type: "text", nullable: true),
                    Recursos = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfg_planos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sis_sessoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UltimaAtividadeEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Ip = table.Column<string>(type: "text", nullable: true),
                    UserAgent = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sis_sessoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sis_empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RazaoSocial = table.Column<string>(type: "text", nullable: false),
                    NomeFantasia = table.Column<string>(type: "text", nullable: false),
                    Cnpj = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PlanoId = table.Column<int>(type: "integer", nullable: true),
                    PlanoVenceEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Bloqueada = table.Column<bool>(type: "boolean", nullable: false),
                    MotivoBloqueio = table.Column<string>(type: "text", nullable: true),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    EnderecoCompleto = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    InstanciaWhatsApp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sis_empresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sis_empresas_cfg_planos_PlanoId",
                        column: x => x.PlanoId,
                        principalTable: "cfg_planos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mar_clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CpfCnpj = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_clientes_sis_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "sis_empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sis_usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    Perfil = table.Column<int>(type: "integer", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    TemaEscuro = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sis_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sis_usuarios_sis_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "sis_empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mar_contratos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    EmbarcacaoId = table.Column<int>(type: "integer", nullable: false),
                    VagaId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    ValorMensal = table.Column<decimal>(type: "numeric", nullable: false),
                    Inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_contratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_contratos_mar_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "mar_clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mar_embarcacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Fabricante = table.Column<string>(type: "text", nullable: true),
                    Modelo = table.Column<string>(type: "text", nullable: true),
                    AnoFabricacao = table.Column<int>(type: "integer", nullable: true),
                    ComprimentoMetros = table.Column<decimal>(type: "numeric", nullable: false),
                    BocaMetros = table.Column<decimal>(type: "numeric", nullable: true),
                    CaladoMetros = table.Column<decimal>(type: "numeric", nullable: true),
                    Cor = table.Column<string>(type: "text", nullable: true),
                    NumeroRegistro = table.Column<string>(type: "text", nullable: true),
                    FotoUrl = table.Column<string>(type: "text", nullable: true),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VagaFixaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_embarcacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_embarcacoes_mar_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "mar_clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mar_ordens_servico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    EmbarcacaoId = table.Column<int>(type: "integer", nullable: false),
                    TipoServico = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResponsavelId = table.Column<int>(type: "integer", nullable: true),
                    ValorEstimado = table.Column<decimal>(type: "numeric", nullable: true),
                    ValorFinal = table.Column<decimal>(type: "numeric", nullable: true),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinalizadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_ordens_servico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_ordens_servico_mar_embarcacoes_EmbarcacaoId",
                        column: x => x.EmbarcacaoId,
                        principalTable: "mar_embarcacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mar_ordens_servico_sis_usuarios_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "sis_usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mar_vagas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Setor = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ComprimentoMaxMetros = table.Column<decimal>(type: "numeric", nullable: false),
                    BocaMaxMetros = table.Column<decimal>(type: "numeric", nullable: true),
                    PosX = table.Column<decimal>(type: "numeric", nullable: false),
                    PosY = table.Column<decimal>(type: "numeric", nullable: false),
                    Largura = table.Column<decimal>(type: "numeric", nullable: false),
                    Altura = table.Column<decimal>(type: "numeric", nullable: false),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    EmbarcacaoAtualId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_vagas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_vagas_mar_embarcacoes_EmbarcacaoAtualId",
                        column: x => x.EmbarcacaoAtualId,
                        principalTable: "mar_embarcacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mar_vagas_sis_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "sis_empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mar_movimentacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    EmbarcacaoId = table.Column<int>(type: "integer", nullable: false),
                    VagaId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResponsavelId = table.Column<int>(type: "integer", nullable: true),
                    AgendadoPara = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    OrigemWhatsApp = table.Column<string>(type: "text", nullable: true),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_movimentacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_movimentacoes_mar_embarcacoes_EmbarcacaoId",
                        column: x => x.EmbarcacaoId,
                        principalTable: "mar_embarcacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mar_movimentacoes_mar_vagas_VagaId",
                        column: x => x.VagaId,
                        principalTable: "mar_vagas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mar_movimentacoes_sis_usuarios_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "sis_usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "mar_historico_movimentacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MovimentacaoId = table.Column<int>(type: "integer", nullable: false),
                    StatusAnterior = table.Column<int>(type: "integer", nullable: false),
                    StatusNovo = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true),
                    Em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_historico_movimentacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_historico_movimentacoes_mar_movimentacoes_MovimentacaoId",
                        column: x => x.MovimentacaoId,
                        principalTable: "mar_movimentacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mar_clientes_EmpresaId",
                table: "mar_clientes",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_contratos_ClienteId",
                table: "mar_contratos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_contratos_EmbarcacaoId",
                table: "mar_contratos",
                column: "EmbarcacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_contratos_VagaId",
                table: "mar_contratos",
                column: "VagaId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_embarcacoes_ClienteId",
                table: "mar_embarcacoes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_embarcacoes_VagaFixaId",
                table: "mar_embarcacoes",
                column: "VagaFixaId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_historico_movimentacoes_MovimentacaoId",
                table: "mar_historico_movimentacoes",
                column: "MovimentacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentacoes_EmbarcacaoId",
                table: "mar_movimentacoes",
                column: "EmbarcacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentacoes_ResponsavelId",
                table: "mar_movimentacoes",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentacoes_VagaId",
                table: "mar_movimentacoes",
                column: "VagaId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_ordens_servico_EmbarcacaoId",
                table: "mar_ordens_servico",
                column: "EmbarcacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_ordens_servico_ResponsavelId",
                table: "mar_ordens_servico",
                column: "ResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_vagas_EmbarcacaoAtualId",
                table: "mar_vagas",
                column: "EmbarcacaoAtualId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_vagas_EmpresaId_Codigo",
                table: "mar_vagas",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sis_empresas_PlanoId",
                table: "sis_empresas",
                column: "PlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_sis_usuarios_EmpresaId_Email",
                table: "sis_usuarios",
                columns: new[] { "EmpresaId", "Email" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_mar_contratos_mar_embarcacoes_EmbarcacaoId",
                table: "mar_contratos",
                column: "EmbarcacaoId",
                principalTable: "mar_embarcacoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mar_contratos_mar_vagas_VagaId",
                table: "mar_contratos",
                column: "VagaId",
                principalTable: "mar_vagas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_mar_embarcacoes_mar_vagas_VagaFixaId",
                table: "mar_embarcacoes",
                column: "VagaFixaId",
                principalTable: "mar_vagas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mar_clientes_sis_empresas_EmpresaId",
                table: "mar_clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_mar_vagas_sis_empresas_EmpresaId",
                table: "mar_vagas");

            migrationBuilder.DropForeignKey(
                name: "FK_mar_embarcacoes_mar_clientes_ClienteId",
                table: "mar_embarcacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_mar_vagas_mar_embarcacoes_EmbarcacaoAtualId",
                table: "mar_vagas");

            migrationBuilder.DropTable(
                name: "cfg_configuracoes");

            migrationBuilder.DropTable(
                name: "mar_contratos");

            migrationBuilder.DropTable(
                name: "mar_historico_movimentacoes");

            migrationBuilder.DropTable(
                name: "mar_ordens_servico");

            migrationBuilder.DropTable(
                name: "sis_sessoes");

            migrationBuilder.DropTable(
                name: "mar_movimentacoes");

            migrationBuilder.DropTable(
                name: "sis_usuarios");

            migrationBuilder.DropTable(
                name: "sis_empresas");

            migrationBuilder.DropTable(
                name: "cfg_planos");

            migrationBuilder.DropTable(
                name: "mar_clientes");

            migrationBuilder.DropTable(
                name: "mar_embarcacoes");

            migrationBuilder.DropTable(
                name: "mar_vagas");
        }
    }
}
