namespace pokemon.Entities
{
    public enum Type
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

        public Type Type { get; set; }

        public Pokemon(int id, int trainerId, string nom, Type type, bool shiny)
        {
            Id = id;
            TrainerId = trainerId;
            Nom = nom;
            Type = type;
            Shiny = shiny;
        }
        
        
    }
}



