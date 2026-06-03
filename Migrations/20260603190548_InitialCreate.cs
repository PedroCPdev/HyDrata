using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyDrata.GestaoApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_COOPERATIVAS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    DATA_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_COOPERATIVAS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_PLANOS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    VALOR_MENSALIDADE = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PLANOS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_PRODUTORES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: true),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    SENHA = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    STATUS = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    DATA_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PRODUTORES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TB_PRODUTOR_COOPERATIVA",
                columns: table => new
                {
                    PRODUTOR_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    COOPERATIVA_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATA_ASSOCIACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PRODUTOR_COOPERATIVA", x => new { x.PRODUTOR_ID, x.COOPERATIVA_ID });
                    table.ForeignKey(
                        name: "FK_PC_COOPERATIVA",
                        column: x => x.COOPERATIVA_ID,
                        principalTable: "TB_COOPERATIVAS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PC_PRODUTOR",
                        column: x => x.PRODUTOR_ID,
                        principalTable: "TB_PRODUTORES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_PROPRIEDADES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    PRODUTOR_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    PLANO_ID = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOME = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    AREA_HECTARES = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    CIDADE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ESTADO = table.Column<string>(type: "NVARCHAR2(2)", maxLength: 2, nullable: true),
                    LATITUDE = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: true),
                    LONGITUDE = table.Column<decimal>(type: "DECIMAL(9,6)", precision: 9, scale: 6, nullable: true),
                    STATUS = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    DATA_CADASTRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PROPRIEDADES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PROPRIEDADE_PLANO",
                        column: x => x.PLANO_ID,
                        principalTable: "TB_PLANOS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PROPRIEDADE_PRODUTOR",
                        column: x => x.PRODUTOR_ID,
                        principalTable: "TB_PRODUTORES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_PRODUTOR_COOPERATIVA_COOPERATIVA_ID",
                table: "TB_PRODUTOR_COOPERATIVA",
                column: "COOPERATIVA_ID");

            migrationBuilder.CreateIndex(
                name: "UQ_PRODUTOR_CPF",
                table: "TB_PRODUTORES",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_PRODUTOR_EMAIL",
                table: "TB_PRODUTORES",
                column: "EMAIL",
                unique: true,
                filter: "\"EMAIL\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PROPRIEDADES_PLANO_ID",
                table: "TB_PROPRIEDADES",
                column: "PLANO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PROPRIEDADES_PRODUTOR_ID",
                table: "TB_PROPRIEDADES",
                column: "PRODUTOR_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_PRODUTOR_COOPERATIVA");

            migrationBuilder.DropTable(
                name: "TB_PROPRIEDADES");

            migrationBuilder.DropTable(
                name: "TB_COOPERATIVAS");

            migrationBuilder.DropTable(
                name: "TB_PLANOS");

            migrationBuilder.DropTable(
                name: "TB_PRODUTORES");
        }
    }
}
