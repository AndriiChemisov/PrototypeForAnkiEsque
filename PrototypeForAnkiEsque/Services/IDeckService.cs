using PrototypeForAnkiEsque.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrototypeForAnkiEsque.Services
{
    public interface IDeckService
    {
        Task<bool> DeckExistsAsync(string deckName);
        Task CreateDeckAsync(string deckName, List<string> flashcardFronts, string easeRating);
        Task<List<FlashcardDeck>> GetPagedDecksAsync(int pageNumber, int pageSize);
        Task<int> GetTotalDeckCountAsync();
        Task<FlashcardDeck> GetDeckByIdAsync(int id);
        Task UpdateDeckAsync(FlashcardDeck deck);
        Task DeleteDeckAsync(int deckId);
        Task<string> CalculateEaseRatingAsync(List<string> flashcardFronts);
    }
}
