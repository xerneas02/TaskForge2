using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonTemplate.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePokemonTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Pokemons",
                table: "Pokemons");

            migrationBuilder.RenameTable(
                name: "Pokemons",
                newName: "PokemonTemplates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PokemonTemplates",
                table: "PokemonTemplates",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PokemonTemplates",
                table: "PokemonTemplates");

            migrationBuilder.RenameTable(
                name: "PokemonTemplates",
                newName: "Pokemons");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Pokemons",
                table: "Pokemons",
                column: "Id");
        }
    }
}
