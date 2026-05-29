using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SetiMarine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperAdminSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sis_usuarios_sis_empresas_EmpresaId",
                table: "sis_usuarios");

            migrationBuilder.AlterColumn<int>(
                name: "EmpresaId",
                table: "sis_usuarios",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "EmpresaId",
                table: "sis_sessoes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Ordem",
                table: "cfg_planos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TextoBotao",
                table: "cfg_planos",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_sis_usuarios_sis_empresas_EmpresaId",
                table: "sis_usuarios",
                column: "EmpresaId",
                principalTable: "sis_empresas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sis_usuarios_sis_empresas_EmpresaId",
                table: "sis_usuarios");

            migrationBuilder.DropColumn(
                name: "Ordem",
                table: "cfg_planos");

            migrationBuilder.DropColumn(
                name: "TextoBotao",
                table: "cfg_planos");

            migrationBuilder.AlterColumn<int>(
                name: "EmpresaId",
                table: "sis_usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmpresaId",
                table: "sis_sessoes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_sis_usuarios_sis_empresas_EmpresaId",
                table: "sis_usuarios",
                column: "EmpresaId",
                principalTable: "sis_empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
