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
        public static int CurId = 0;
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public bool Shiny { get; set; }
        public string Nom { get; set; }

        public PokemonType Type { get; set; }

        public Pokemon(int id, int trainerId, string nom, PokemonType type, bool shiny)
        {
            Id = id;
            TrainerId = trainerId;
            Nom = nom;
            Type = type;
            Shiny = shiny;
        }
        
        public Pokemon(int trainerId = -1, string nom = "Tiplouf", PokemonType type = PokemonType.Water, bool shiny = false)
        {
            Id = CurId;
            TrainerId = trainerId;
            Nom = nom;
            Type = type;
            Shiny = shiny;

            CurId++;
        }
        
    }
}



