using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Models;
using System.Collections.Generic;
using System.Linq;

namespace PrototypeForAnkiEsque.Services
{
    public class DeckService
    {
        private readonly ApplicationDbContext _context;

        public DeckService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool DeckExists(string deckName)
        {
            return _context.FlashcardDecks
                .Any(d => d.Name.ToLower() == deckName.ToLower());
        }

        public void CreateDeck(string deckName, List<int> flashcardIds)
        {
            var newDeck = new FlashcardDeck
            {
                Name = deckName,
                FlashcardIds = flashcardIds
            };

            _context.FlashcardDecks.Add(newDeck);
            _context.SaveChanges();  // Persist changes to the database
        }

        public List<FlashcardDeck> GetAllDecks()
        {
            return _context.FlashcardDecks.ToList();
        }

        public FlashcardDeck GetDeckById(int id)
        {
            return _context.FlashcardDecks.FirstOrDefault(d => d.Id == id);
        }
    }
}
