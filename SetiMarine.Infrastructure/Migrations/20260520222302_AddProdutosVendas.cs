using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProdutosVendas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mar_produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Unidade = table.Column<string>(type: "text", nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "numeric", nullable: true),
                    PrecoAluguelDia = table.Column<decimal>(type: "numeric", nullable: true),
                    Estoque = table.Column<int>(type: "integer", nullable: false),
                    EstoqueMinimo = table.Column<int>(type: "integer", nullable: false),
                    Alugavel = table.Column<bool>(type: "boolean", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_produtos_sis_empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "sis_empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mar_vendas_produto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    ClienteId = table.Column<int>(type: "integer", nullable: true),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    DataTransacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataDevolucaoPrevista = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataDevolucaoReal = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StatusAluguel = table.Column<int>(type: "integer", nullable: true),
                    Observacao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_vendas_produto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_vendas_produto_mar_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "mar_clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_mar_vendas_produto_mar_produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "mar_produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mar_vendas_produto_sis_usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "sis_usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_mar_produtos_EmpresaId",
                table: "mar_produtos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_vendas_produto_ClienteId",
                table: "mar_vendas_produto",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_vendas_produto_ProdutoId",
                table: "mar_vendas_produto",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_vendas_produto_UsuarioId",
                table: "mar_vendas_produto",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mar_vendas_produto");

            migrationBuilder.DropTable(
                name: "mar_produtos");
        }
    }
}
