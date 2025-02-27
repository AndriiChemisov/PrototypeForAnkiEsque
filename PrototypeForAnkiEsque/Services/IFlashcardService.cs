using PrototypeForAnkiEsque.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrototypeForAnkiEsque.Services
{
    public interface IFlashcardService
    {
        List<Flashcard> GetFlashcards();
        List<Flashcard> GetFlashcardsByDeck(int deckId);
        void AddFlashcard(Flashcard card);
        void UpdateFlashcard(Flashcard card);
        Task DeleteFlashcardAsync(int cardId);
    }
}
