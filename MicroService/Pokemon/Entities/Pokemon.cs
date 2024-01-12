using pokemon.Data;

namespace pokemon.Entities
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

    public class Pokemon
    {

        public int Id { get; set; }
        public int TrainerId { get; set; }
        public bool Shiny { get; set; }
        public string Nom { get; set; }
        public PokemonType Type { get; set; }

        public Pokemon(int id, int trainerId = -1, string nom = "Tiplouf", PokemonType type = PokemonType.Water, bool shiny = false)
        {
            Id = id;
            TrainerId = trainerId;
            Nom = nom;
            Type = type;
            Shiny = shiny;
        }
    }
}



