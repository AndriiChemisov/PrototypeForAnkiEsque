using PrototypeForAnkiEsque.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrototypeForAnkiEsque.Services
{
    public interface IDeckService
    {
        bool DeckExists(string deckName);
        Task CreateDeckAsync(string deckName, List<string> flashcardFronts, string easeRating);
        List<FlashcardDeck> GetPagedDecks(int pageNumber, int pageSize);
        int GetTotalDeckCount();
        FlashcardDeck GetDeckById(int id);
        Task UpdateDeckAsync(FlashcardDeck deck);
        Task DeleteDeckAsync(int deckId);
        string CalculateEaseRating(List<string> flashcardFronts);
        bool CheckIfDeckNameExists(string deckName);
    }
}
