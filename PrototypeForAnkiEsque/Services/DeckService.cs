using Microsoft.EntityFrameworkCore;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task CreateDeckAsync(string deckName, List<int> flashcardIds, string easeRating)
        {
            var newDeck = new FlashcardDeck
            {
                Name = deckName,
                FlashcardIds = flashcardIds,
                EaseRating = easeRating
            };

            _context.FlashcardDecks.Add(newDeck);
            await _context.SaveChangesAsync();  // Persist changes to the database asynchronously
        }

        public List<FlashcardDeck> GetPagedDecks(int pageNumber, int pageSize)
        {
            return _context.FlashcardDecks
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();
        }

        public int GetTotalDeckCount()
        {
            return _context.FlashcardDecks.Count();
        }

        public FlashcardDeck GetDeckById(int id)
        {
            return _context.FlashcardDecks.FirstOrDefault(d => d.Id == id);
        }

        public async Task UpdateDeckAsync(FlashcardDeck deck)
        {
            _context.FlashcardDecks.Update(deck);
            _context.SaveChanges();
        }

        public async Task DeleteDeckAsync(int deckId)
        {
            var deckToDelete = await _context.FlashcardDecks
                .FirstOrDefaultAsync(d => d.Id == deckId);

            if (deckToDelete != null)
            {
                _context.FlashcardDecks.Remove(deckToDelete);
                await _context.SaveChangesAsync(); // Asynchronously save changes
            }
        }

        public string CalculateEaseRating(List<int> flashcardIds)
        {
            var flashcards = _context.Flashcards.Where(f => flashcardIds.Contains(f.Id)).ToList();
            if (!flashcards.Any()) return "100%";

            double averageEaseRating = flashcards.Average(f => f.EaseRating);

            // Convert to percentage
            return Math.Round((100 - (averageEaseRating / 2) * 100), 2).ToString() + "%";
        }

        public bool CheckIfDeckNameExists(string deckName)
        {
            // Check if any deck in the database has the same name, ignoring case
            return _context.FlashcardDecks
                .Any(d => d.Name.ToLower() == deckName.ToLower());
        }
    }
}
