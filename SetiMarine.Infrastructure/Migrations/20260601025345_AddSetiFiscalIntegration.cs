using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSetiFiscalIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoNbs",
                table: "mar_tipos_servico_config",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoServico",
                table: "mar_tipos_servico_config",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CodigoSefaz",
                table: "mar_notas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MensagemSefaz",
                table: "mar_notas",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoNbs",
                table: "mar_nota_itens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoServico",
                table: "mar_nota_itens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "mar_clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cep",
                table: "mar_clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CodigoMunicipio",
                table: "mar_clientes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "mar_clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ie",
                table: "mar_clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logradouro",
                table: "mar_clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Municipio",
                table: "mar_clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroEnd",
                table: "mar_clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Uf",
                table: "mar_clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoNbsDefault",
                table: "cfg_configuracao_marina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoServicoDefault",
                table: "cfg_configuracao_marina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SetiFiscalApiKey",
                table: "cfg_configuracao_marina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SetiFiscalBaseUrl",
                table: "cfg_configuracao_marina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SetiFiscalEmpresaId",
                table: "cfg_configuracao_marina",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoNbs",
                table: "mar_tipos_servico_config");

            migrationBuilder.DropColumn(
                name: "CodigoServico",
                table: "mar_tipos_servico_config");

            migrationBuilder.DropColumn(
                name: "CodigoSefaz",
                table: "mar_notas");

            migrationBuilder.DropColumn(
                name: "MensagemSefaz",
                table: "mar_notas");

            migrationBuilder.DropColumn(
                name: "CodigoNbs",
                table: "mar_nota_itens");

            migrationBuilder.DropColumn(
                name: "CodigoServico",
                table: "mar_nota_itens");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "mar_clientes");

            migrationBuilder.DropColumn(
                name: "Cep",
                table: "mar_clientes");

            migrationBuilder.DropColumn(
                name: "CodigoMunicipio",
                table: "mar_clientes");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "mar_clientes");

            migrationBuilder.DropColumn(
                name: "Ie",
                table: "mar_clientes");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "mar_clientes");

            migrationBuilder.DropColumn(
                name: "Municipio",
                table: "mar_clientes");

            migrationBuilder.DropColumn(
                name: "NumeroEnd",
                table: "mar_clientes");

            migrationBuilder.DropColumn(
                name: "Uf",
                table: "mar_clientes");

            migrationBuilder.DropColumn(
                name: "CodigoNbsDefault",
                table: "cfg_configuracao_marina");

            migrationBuilder.DropColumn(
                name: "CodigoServicoDefault",
                table: "cfg_configuracao_marina");

            migrationBuilder.DropColumn(
                name: "SetiFiscalApiKey",
                table: "cfg_configuracao_marina");

            migrationBuilder.DropColumn(
                name: "SetiFiscalBaseUrl",
                table: "cfg_configuracao_marina");

            migrationBuilder.DropColumn(
                name: "SetiFiscalEmpresaId",
                table: "cfg_configuracao_marina");
        }
    }
}
