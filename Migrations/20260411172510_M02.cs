using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerseEscalavel.Migrations
{
    public partial class M02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensComprados_Produtos_ProdutoId",
                table: "ItensComprados");

            migrationBuilder.DropIndex(
                name: "IX_ItensComprados_ProdutoId",
                table: "ItensComprados");

            migrationBuilder.DropColumn(
                name: "ProdutoId",
                table: "ItensComprados");

            migrationBuilder.CreateIndex(
                name: "IX_ItensComprados_IdProduto",
                table: "ItensComprados",
                column: "IdProduto");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensComprados_Produtos_IdProduto",
                table: "ItensComprados",
                column: "IdProduto",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItensComprados_Produtos_IdProduto",
                table: "ItensComprados");

            migrationBuilder.DropIndex(
                name: "IX_ItensComprados_IdProduto",
                table: "ItensComprados");

            migrationBuilder.AddColumn<int>(
                name: "ProdutoId",
                table: "ItensComprados",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ItensComprados_ProdutoId",
                table: "ItensComprados",
                column: "ProdutoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItensComprados_Produtos_ProdutoId",
                table: "ItensComprados",
                column: "ProdutoId",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
