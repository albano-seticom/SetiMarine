using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mar_pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: true),
                    EmbarcacaoId = table.Column<int>(type: "integer", nullable: true),
                    ResponsavelId = table.Column<int>(type: "integer", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PrevisaoConclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConcluidoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_pedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_pedidos_mar_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "mar_clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mar_pedidos_mar_embarcacoes_EmbarcacaoId",
                        column: x => x.EmbarcacaoId,
                        principalTable: "mar_embarcacoes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mar_pedidos_sis_usuarios_ResponsavelId",
                        column: x => x.ResponsavelId,
                        principalTable: "sis_usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "mar_pedido_itens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    DataDevolucaoPrevista = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataDevolucaoReal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StatusEmprestimo = table.Column<int>(type: "integer", nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_pedido_itens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_pedido_itens_mar_pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "mar_pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mar_pedido_itens_mar_produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "mar_produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mar_pedido_servicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PedidoId = table.Column<int>(type: "integer", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric", nullable: true),
                    Concluido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_pedido_servicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_pedido_servicos_mar_pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "mar_pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mar_pedido_itens_PedidoId",
                table: "mar_pedido_itens",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_pedido_itens_ProdutoId",
                table: "mar_pedido_itens",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_pedido_servicos_PedidoId",
                table: "mar_pedido_servicos",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_pedidos_ClienteId",
                table: "mar_pedidos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_pedidos_EmbarcacaoId",
                table: "mar_pedidos",
                column: "EmbarcacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_pedidos_ResponsavelId",
                table: "mar_pedidos",
                column: "ResponsavelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mar_pedido_itens");

            migrationBuilder.DropTable(
                name: "mar_pedido_servicos");

            migrationBuilder.DropTable(
                name: "mar_pedidos");
        }
    }
}
