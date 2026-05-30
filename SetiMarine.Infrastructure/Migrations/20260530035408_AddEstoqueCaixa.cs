using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstoqueCaixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ControlaEstoque",
                table: "mar_produtos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "cfg_configuracao_marina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    CaixaAutomatico = table.Column<bool>(type: "boolean", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cfg_configuracao_marina", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "mar_movimentos_estoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    EstoqueAntes = table.Column<int>(type: "integer", nullable: false),
                    EstoqueDepois = table.Column<int>(type: "integer", nullable: false),
                    Motivo = table.Column<string>(type: "text", nullable: true),
                    PedidoId = table.Column<int>(type: "integer", nullable: true),
                    VendaProdutoId = table.Column<int>(type: "integer", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_movimentos_estoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_movimentos_estoque_mar_pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "mar_pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_mar_movimentos_estoque_mar_produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "mar_produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_mar_movimentos_estoque_mar_vendas_produto_VendaProdutoId",
                        column: x => x.VendaProdutoId,
                        principalTable: "mar_vendas_produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_mar_movimentos_estoque_sis_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "sis_usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "mar_sessoes_caixa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    OperadorId = table.Column<int>(type: "integer", nullable: true),
                    DataAbertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValorAbertura = table.Column<decimal>(type: "numeric", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValorFechamentoContado = table.Column<decimal>(type: "numeric", nullable: true),
                    ValorFechamentoEsperado = table.Column<decimal>(type: "numeric", nullable: true),
                    Diferenca = table.Column<decimal>(type: "numeric", nullable: true),
                    ObservacoesFechamento = table.Column<string>(type: "text", nullable: true),
                    Aberto = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_sessoes_caixa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_sessoes_caixa_sis_usuarios_OperadorId",
                        column: x => x.OperadorId,
                        principalTable: "sis_usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "mar_movimentos_caixa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    SessaoCaixaId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Origem = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    PedidoId = table.Column<int>(type: "integer", nullable: true),
                    VendaProdutoId = table.Column<int>(type: "integer", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_movimentos_caixa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_movimentos_caixa_mar_pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "mar_pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_mar_movimentos_caixa_mar_sessoes_caixa_SessaoCaixaId",
                        column: x => x.SessaoCaixaId,
                        principalTable: "mar_sessoes_caixa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mar_movimentos_caixa_mar_vendas_produto_VendaProdutoId",
                        column: x => x.VendaProdutoId,
                        principalTable: "mar_vendas_produto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cfg_configuracao_marina_EmpresaId",
                table: "cfg_configuracao_marina",
                column: "EmpresaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentos_caixa_PedidoId",
                table: "mar_movimentos_caixa",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentos_caixa_SessaoCaixaId",
                table: "mar_movimentos_caixa",
                column: "SessaoCaixaId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentos_caixa_VendaProdutoId",
                table: "mar_movimentos_caixa",
                column: "VendaProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentos_estoque_PedidoId",
                table: "mar_movimentos_estoque",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentos_estoque_ProdutoId",
                table: "mar_movimentos_estoque",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentos_estoque_UsuarioId",
                table: "mar_movimentos_estoque",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_movimentos_estoque_VendaProdutoId",
                table: "mar_movimentos_estoque",
                column: "VendaProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_sessoes_caixa_OperadorId",
                table: "mar_sessoes_caixa",
                column: "OperadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cfg_configuracao_marina");

            migrationBuilder.DropTable(
                name: "mar_movimentos_caixa");

            migrationBuilder.DropTable(
                name: "mar_movimentos_estoque");

            migrationBuilder.DropTable(
                name: "mar_sessoes_caixa");

            migrationBuilder.DropColumn(
                name: "ControlaEstoque",
                table: "mar_produtos");
        }
    }
}
