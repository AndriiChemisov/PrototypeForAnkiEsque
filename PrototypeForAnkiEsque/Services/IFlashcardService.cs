using PrototypeForAnkiEsque.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
