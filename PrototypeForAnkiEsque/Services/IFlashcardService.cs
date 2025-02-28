using PrototypeForAnkiEsque.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
// This file is used to define the IFlashcardService interface. The IFlashcardService interface defines the methods that the FlashcardService class must implement.
// This file should be added to first, before FlashcardService is modified, as we use the interface to define the methods that the FlashcardService class must implement.
// Furthermore, we use objects of this interface type as per the Dependency Inversion Principle.
namespace PrototypeForAnkiEsque.Services
{
    public interface IFlashcardService
    {
        Task<List<Flashcard>> GetFlashcardsAsync();
        Task<List<Flashcard>> GetFlashcardsByDeckAsync(int deckId);
        Task AddFlashcardAsync(Flashcard card);
        Task UpdateFlashcardAsync(Flashcard card);
        Task DeleteFlashcardAsync(int cardId);
    }
}
