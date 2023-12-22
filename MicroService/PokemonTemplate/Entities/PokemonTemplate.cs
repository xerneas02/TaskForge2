namespace pokemonTemplate.Entities
{
    public enum PokemonType
    {
        Normal,
        Fire,
        Water,
        Electric,
        Grass,
        Ice,
        Fighting,
        Poison,
        Ground,
        Flying,
        Psychic,
        Bug,
        Rock,
        Ghost,
        Dragon,
        Dark,
        Steel,
        Fairy
    }

    public class PokemonTemplate
    {
        public int Id { get; set; }
        public string Nom { get; set; }

        public PokemonType Type { get; set; }

        public PokemonTemplate(int id, string nom, PokemonType type)
        {
            Id = id;
            Nom = nom;
            Type = type;
        }
        
        
    }
}



