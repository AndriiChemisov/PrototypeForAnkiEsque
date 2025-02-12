using System.Collections.Generic;
using System.Linq;
using PrototypeForAnkiEsque.Models;
using Microsoft.EntityFrameworkCore;
using PrototypeForAnkiEsque.Data;

namespace PrototypeForAnkiEsque.Services
{
    public class FlashcardService
    {
        private readonly ApplicationDbContext _context;

        public FlashcardService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to retrieve flashcards from the database
        public List<Flashcard> GetFlashcards()
        {
            return _context.Flashcards.ToList(); // Get all flashcards from the database
        }

        // Add more methods for adding, updating, or deleting flashcards as necessary
        // Method to add a flashcard
        public void AddFlashcard(Flashcard card)
        {
            _context.Flashcards.Add(card);
            _context.SaveChanges();
        }

        // Method to update a flashcard
        public void UpdateFlashcard(Flashcard card)
        {
            _context.Flashcards.Update(card);
            _context.SaveChanges();
        }

        // Method to delete a flashcard
        public async Task DeleteFlashcardAsync(int cardId)
        {
            var card = await _context.Flashcards.FindAsync(cardId);
            if (card != null)
            {
                _context.Flashcards.Remove(card);
                await _context.SaveChangesAsync(); // Save changes asynchronously
            }
        }

    }
}
