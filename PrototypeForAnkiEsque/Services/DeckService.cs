using Microsoft.EntityFrameworkCore;
using PrototypeForAnkiEsque.Data;
using PrototypeForAnkiEsque.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// This file is used to define the DeckService class, which implements the IDeckService interface. The DeckService class is used to interact with the database and perform CRUD operations on the FlashcardDecks table.
// The DeckService class defines a constructor that takes an instance of the ApplicationDbContext class as a parameter. This allows the DeckService class to interact
// with the database using the ApplicationDbContext class.
// Simple explanation: This class is used to interact with the database and perform CRUD operations on the FlashcardDecks table so that we do not violate the Single Responsibility Principle.
namespace PrototypeForAnkiEsque.Services
{
    public class DeckService : IDeckService
    {
        private readonly ApplicationDbContext _context;

        public DeckService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeckExistsAsync(string deckName)
        {
            return await _context.FlashcardDecks
                .AnyAsync(d => d.Name.ToLower() == deckName.ToLower());
        }

        public async Task CreateDeckAsync(string deckName, List<string> flashcardFronts, string easeRating)
        {
            var newDeck = new FlashcardDeck
            {
                Name = deckName,
                FlashcardFronts = flashcardFronts,
                EaseRating = easeRating
            };

            _context.FlashcardDecks.Add(newDeck);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FlashcardDeck>> GetPagedDecksAsync(int pageNumber, int pageSize)
        {
            return await _context.FlashcardDecks
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .ToListAsync();
        }

        public async Task<int> GetTotalDeckCountAsync()
        {
            return await _context.FlashcardDecks.CountAsync();
        }

        public async Task<FlashcardDeck> GetDeckByIdAsync(int id)
        {
            return await _context.FlashcardDecks.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task UpdateDeckAsync(FlashcardDeck deck)
        {
            _context.FlashcardDecks.Update(deck);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDeckAsync(int deckId)
        {
            var deckToDelete = await _context.FlashcardDecks
                .FindAsync(deckId);

            if (deckToDelete != null)
            {
                _context.FlashcardDecks.Remove(deckToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> CalculateEaseRatingAsync(List<string> flashcardFronts)
        {
            var flashcards = await _context.Flashcards.Where(f => flashcardFronts.Contains(f.Front)).ToListAsync();
            if (!flashcards.Any()) return "100%";

            double averageEaseRating = flashcards.Average(f => f.EaseRating);

            return Math.Round((100 - (averageEaseRating / 2) * 100), 2).ToString() + "%";
        }

    }
}
