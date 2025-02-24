using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PrototypeForAnkiEsque.Models;

namespace PrototypeForAnkiEsque.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<FlashcardDeck> FlashcardDecks { get; set; }

        public static void Seed(ApplicationDbContext context)
        {

            if (!context.Flashcards.Any())
            {
                context.Flashcards.AddRange(
                    new Flashcard { Front = "人", Back = "にんげん ー Human", EaseRating = 2},
                    new Flashcard { Front = "犬", Back = "いぬ ー Dog", EaseRating = 2 },
                    new Flashcard { Front = "猫", Back = "ねこ ー Cat", EaseRating = 2 },
                    new Flashcard { Front = "鳥", Back = "とり ー Bird", EaseRating = 2 },
                    new Flashcard { Front = "魚", Back = "さかな ー Fish", EaseRating = 2 },
                    new Flashcard { Front = "馬", Back = "うま ー Horse", EaseRating = 2 },
                    new Flashcard { Front = "牛", Back = "うし ー Cow", EaseRating = 2 },
                    new Flashcard { Front = "羊", Back = "ひつじ ー Sheep", EaseRating = 2 },
                    new Flashcard { Front = "豚", Back = "ぶた ー Pig", EaseRating = 2 }
                    );
                context.FlashcardDecks.AddRange(
                    new FlashcardDeck { Name = "動物 ー Animals", FlashcardIds = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9 }, EaseRating = "100%" }
                    );
                context.SaveChanges();
            }
        }
    }
}
