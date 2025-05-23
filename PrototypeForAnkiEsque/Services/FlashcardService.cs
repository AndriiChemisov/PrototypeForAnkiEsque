﻿using PrototypeForAnkiEsque.Models;
using PrototypeForAnkiEsque.Data;
using Microsoft.EntityFrameworkCore;
// This file is used to define the FlashcardService class, which implements the IFlashcardService interface. The FlashcardService class is used to interact with the database and perform CRUD operations on the Flashcards table.
// The FlashcardService class defines a constructor that takes an instance of the ApplicationDbContext class as a parameter. This allows the FlashcardService class to interact
// with the database using the ApplicationDbContext class.
// Simple explanation: This class is used to interact with the database and perform CRUD operations on the Flashcards table so that we do not violate the Single Responsibility Principle.
namespace PrototypeForAnkiEsque.Services
{
    public class FlashcardService : IFlashcardService
    {
        private readonly ApplicationDbContext _context;

        public FlashcardService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to retrieve all flashcards from the database
        public async Task<List<Flashcard>> GetFlashcardsAsync()
        {
            return await _context.Flashcards.ToListAsync();
        }

        // Method to retrieve flashcards by deck ID using the FlashcardIds in the FlashcardDeck
        public async Task<List<Flashcard>> GetFlashcardsByDeckAsync(int deckId)
        {
            // Get the deck by ID
            var deck = _context.FlashcardDecks
                               .FirstOrDefault(d => d.Id == deckId);

            if (deck != null && deck.FlashcardFronts != null)
            {
                // Get flashcards that match the IDs in the FlashcardIds list
                return _context.Flashcards
                               .Where(fc => deck.FlashcardFronts.Contains(fc.Front))
                               .ToList();
            }

            return new List<Flashcard>(); // Return an empty list if no deck is found
        }

        // Method to add a flashcard
        public async Task AddFlashcardAsync(Flashcard card)
        {
            _context.Flashcards.Add(card);
            await _context.SaveChangesAsync();
        }


        // Method to update a flashcard
        public async Task UpdateFlashcardAsync(Flashcard card)
        {
            _context.Flashcards.Update(card);
            await _context.SaveChangesAsync();
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
