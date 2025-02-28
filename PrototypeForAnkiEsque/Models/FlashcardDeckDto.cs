namespace PrototypeForAnkiEsque.Models
{
    public class FlashcardDeckDto
    {
        public string DeckName { get; set; }
        public List<FlashcardDto> Flashcards { get; set; }
        //Defaulting to hard, as this is the intended base state for the deck in the application. This will reset when the customer rates any card in the deck.
        public string EaseRating { get; set; } = "Hard";
    }
}
