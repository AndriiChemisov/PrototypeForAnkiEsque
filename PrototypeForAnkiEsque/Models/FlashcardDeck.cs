using System.ComponentModel.DataAnnotations;

namespace PrototypeForAnkiEsque.Models
{
    public class FlashcardDeck
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> FlashcardFronts { get; set; }
        public string EaseRating { get; set; }
    }
}