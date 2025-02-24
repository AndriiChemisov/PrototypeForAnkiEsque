namespace PrototypeForAnkiEsque.Models
{
    public class FlashcardDeckDto
    {
        public string DeckName { get; set; }
        public List<FlashcardDto> Flashcards { get; set; }
        public string EaseRating { get; set; } = "Hard";
    }
}
