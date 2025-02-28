using PrototypeForAnkiEsque.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
// This file is used to define the IDeckService interface. The IDeckService interface defines the methods that the DeckService class must implement.
// This file should be added to first, before DeckService is modified, as we use the interface to define the methods that the DeckService class must implement.
// Furthermore, we use objects of this interface type as per the Dependency Inversion Principle.
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
