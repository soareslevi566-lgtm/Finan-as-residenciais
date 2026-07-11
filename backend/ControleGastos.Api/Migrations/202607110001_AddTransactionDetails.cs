using ControleGastos.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ControleGastos.Api.Migrations;

[DbContext(typeof(AppDbContext)), Migration("202607110001_AddTransactionDetails")]
public partial class AddTransactionDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(name: "Categoria", table: "Transacoes", type: "INTEGER", nullable: false, defaultValue: 8);
        // SQLite exige um valor constante ao adicionar coluna em tabela existente.
        // Apenas registros legados recebem essa data; novos lançamentos informam a data real.
        migrationBuilder.AddColumn<DateTime>(name: "Data", table: "Transacoes", type: "TEXT", nullable: false, defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        migrationBuilder.CreateIndex(name: "IX_Transacoes_Data", table: "Transacoes", column: "Data");
        migrationBuilder.CreateIndex(name: "IX_Transacoes_Categoria", table: "Transacoes", column: "Categoria");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Transacoes_Data", table: "Transacoes");
        migrationBuilder.DropIndex(name: "IX_Transacoes_Categoria", table: "Transacoes");
        migrationBuilder.DropColumn(name: "Categoria", table: "Transacoes");
        migrationBuilder.DropColumn(name: "Data", table: "Transacoes");
    }
}
