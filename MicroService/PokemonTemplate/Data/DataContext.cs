using Microsoft.EntityFrameworkCore;

namespace pokemonTemplate.Data
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        public DataContext(IConfiguration configuration)
        {

            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
        }
        public DbSet<Entities.PokemonTemplate> Pokemons { get; set; }
    }
}
