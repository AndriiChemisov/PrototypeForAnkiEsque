namespace PrototypeForAnkiEsque.Models
{
    public class Flashcard
    {
        public int Id { get; set; }
        public string Front { get; set; }
        public string Back { get; set; }
        public DateTime LastReviewed { get; set; }
        public DateTime NextReview { get; set; }
        public int Interval { get; set; }
        public int EaseFactor { get; set; }
    }
}
