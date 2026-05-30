using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProdutoAuxNotasFiscais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adicionar colunas fiscais nos itens do pedido
            migrationBuilder.AddColumn<decimal>(
                name: "AliquotaCOFINS",
                table: "mar_pedido_itens",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AliquotaICMS",
                table: "mar_pedido_itens",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AliquotaIPI",
                table: "mar_pedido_itens",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AliquotaISS",
                table: "mar_pedido_itens",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AliquotaPIS",
                table: "mar_pedido_itens",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cfop",
                table: "mar_pedido_itens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CstIcms",
                table: "mar_pedido_itens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ncm",
                table: "mar_pedido_itens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoCusto",
                table: "mar_pedido_itens",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "mar_notas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    PedidoId = table.Column<int>(type: "integer", nullable: true),
                    ClienteId = table.Column<int>(type: "integer", nullable: true),
                    Numero = table.Column<int>(type: "integer", nullable: false),
                    Serie = table.Column<string>(type: "text", nullable: false),
                    ChaveNFe = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataEmissao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataSaida = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ValorProdutos = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorICMS = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorIPI = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorPIS = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorCOFINS = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorISS = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_notas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_notas_mar_clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "mar_clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_mar_notas_mar_pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "mar_pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "mar_produtos_aux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProdutoId = table.Column<int>(type: "integer", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    PrecoCusto = table.Column<decimal>(type: "numeric", nullable: true),
                    PrecoVenda = table.Column<decimal>(type: "numeric", nullable: true),
                    PrecoAluguelHora = table.Column<decimal>(type: "numeric", nullable: true),
                    PrecoAluguelDia = table.Column<decimal>(type: "numeric", nullable: true),
                    MargemLucro = table.Column<decimal>(type: "numeric", nullable: true),
                    ControlaEstoque = table.Column<bool>(type: "boolean", nullable: false),
                    Estoque = table.Column<int>(type: "integer", nullable: false),
                    EstoqueMinimo = table.Column<int>(type: "integer", nullable: false),
                    AliquotaICMS = table.Column<decimal>(type: "numeric", nullable: false),
                    AliquotaIPI = table.Column<decimal>(type: "numeric", nullable: false),
                    AliquotaPIS = table.Column<decimal>(type: "numeric", nullable: false),
                    AliquotaCOFINS = table.Column<decimal>(type: "numeric", nullable: false),
                    AliquotaISS = table.Column<decimal>(type: "numeric", nullable: false),
                    CstIcms = table.Column<string>(type: "text", nullable: true),
                    CstPisCofins = table.Column<string>(type: "text", nullable: true),
                    Cfop = table.Column<string>(type: "text", nullable: true),
                    Ncm = table.Column<string>(type: "text", nullable: true),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_produtos_aux", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_produtos_aux_mar_produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "mar_produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "mar_nota_itens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NotaId = table.Column<int>(type: "integer", nullable: false),
                    ProdutoId = table.Column<int>(type: "integer", nullable: true),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Quantidade = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric", nullable: false),
                    Cfop = table.Column<string>(type: "text", nullable: true),
                    Ncm = table.Column<string>(type: "text", nullable: true),
                    CstIcms = table.Column<string>(type: "text", nullable: true),
                    AliquotaICMS = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorICMS = table.Column<decimal>(type: "numeric", nullable: false),
                    AliquotaIPI = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorIPI = table.Column<decimal>(type: "numeric", nullable: false),
                    AliquotaPIS = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorPIS = table.Column<decimal>(type: "numeric", nullable: false),
                    AliquotaCOFINS = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorCOFINS = table.Column<decimal>(type: "numeric", nullable: false),
                    AliquotaISS = table.Column<decimal>(type: "numeric", nullable: false),
                    ValorISS = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mar_nota_itens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mar_nota_itens_mar_notas_NotaId",
                        column: x => x.NotaId,
                        principalTable: "mar_notas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_mar_nota_itens_mar_produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "mar_produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mar_nota_itens_NotaId",
                table: "mar_nota_itens",
                column: "NotaId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_nota_itens_ProdutoId",
                table: "mar_nota_itens",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_notas_ClienteId",
                table: "mar_notas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_notas_PedidoId",
                table: "mar_notas",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_mar_produtos_aux_ProdutoId",
                table: "mar_produtos_aux",
                column: "ProdutoId",
                unique: true);

            // Migrar dados das colunas antigas de mar_produtos para mar_produtos_aux
            migrationBuilder.Sql(@"
                INSERT INTO mar_produtos_aux (
                    ""ProdutoId"", ""EmpresaId"",
                    ""PrecoVenda"", ""PrecoAluguelHora"", ""PrecoAluguelDia"",
                    ""ControlaEstoque"", ""Estoque"", ""EstoqueMinimo"",
                    ""AliquotaICMS"", ""AliquotaIPI"", ""AliquotaPIS"", ""AliquotaCOFINS"", ""AliquotaISS"",
                    ""AtualizadoEm""
                )
                SELECT
                    ""Id"", ""EmpresaId"",
                    ""PrecoVenda"", ""PrecoAluguelHora"", ""PrecoAluguelDia"",
                    ""ControlaEstoque"", ""Estoque"", ""EstoqueMinimo"",
                    0, 0, 0, 0, 0,
                    NOW()
                FROM mar_produtos;
            ");

            // Remover colunas antigas APÓS a migração de dados
            migrationBuilder.DropColumn(name: "ControlaEstoque", table: "mar_produtos");
            migrationBuilder.DropColumn(name: "Estoque",          table: "mar_produtos");
            migrationBuilder.DropColumn(name: "EstoqueMinimo",    table: "mar_produtos");
            migrationBuilder.DropColumn(name: "PrecoAluguelDia",  table: "mar_produtos");
            migrationBuilder.DropColumn(name: "PrecoAluguelHora", table: "mar_produtos");
            migrationBuilder.DropColumn(name: "PrecoVenda",       table: "mar_produtos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mar_nota_itens");

            migrationBuilder.DropTable(
                name: "mar_produtos_aux");

            migrationBuilder.DropTable(
                name: "mar_notas");

            migrationBuilder.DropColumn(
                name: "AliquotaCOFINS",
                table: "mar_pedido_itens");

            migrationBuilder.DropColumn(
                name: "AliquotaICMS",
                table: "mar_pedido_itens");

            migrationBuilder.DropColumn(
                name: "AliquotaIPI",
                table: "mar_pedido_itens");

            migrationBuilder.DropColumn(
                name: "AliquotaISS",
                table: "mar_pedido_itens");

            migrationBuilder.DropColumn(
                name: "AliquotaPIS",
                table: "mar_pedido_itens");

            migrationBuilder.DropColumn(
                name: "Cfop",
                table: "mar_pedido_itens");

            migrationBuilder.DropColumn(
                name: "CstIcms",
                table: "mar_pedido_itens");

            migrationBuilder.DropColumn(
                name: "Ncm",
                table: "mar_pedido_itens");

            migrationBuilder.DropColumn(
                name: "PrecoCusto",
                table: "mar_pedido_itens");

            migrationBuilder.AddColumn<bool>(
                name: "ControlaEstoque",
                table: "mar_produtos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Estoque",
                table: "mar_produtos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstoqueMinimo",
                table: "mar_produtos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoAluguelDia",
                table: "mar_produtos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoAluguelHora",
                table: "mar_produtos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecoVenda",
                table: "mar_produtos",
                type: "numeric",
                nullable: true);
        }
    }
}
