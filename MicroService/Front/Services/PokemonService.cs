namespace Front.Services
{
    public enum PokemonType
    {
        Normal,
        Feu,
        Eau,
        Electrique,
        Plante,
        Glace,
        Combat,
        Poison,
        Sol,
        Vol,
        Psy,
        Insecte,
        Roche,
        Sprectre,
        Dragon,
        Tenebre,
        Acier,
        Fee
    }

    public class PokemonService
    {

        public int Id { get; set; }
        public int TrainerId { get; set; }
        public int IdPokedex { get; set; }
        public bool Shiny { get; set; }
        public string Nom { get; set; }
        public PokemonType Type { get; set; }

        public PokemonService(int id, int trainerId = -1, int idPokedex = 0, string nom = "Tiplouf", PokemonType type = PokemonType.Eau, bool shiny = false)
        {
            Id = id;
            TrainerId = trainerId;
            IdPokedex = idPokedex;
            Nom = nom;
            Type = type;
            Shiny = shiny;
        }
    }
}



