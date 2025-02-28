using Microsoft.EntityFrameworkCore;
using PrototypeForAnkiEsque.Models;
// This file is used to define the database context for the application. The database context is used to interact with the database and define the tables and relationships between them.
// The ApplicationDbContext class is used to define the database context for the application. It inherits from the DbContext class provided by Entity Framework Core.
// The ApplicationDbContext class defines two DbSet properties, Flashcards and FlashcardDecks, which represent the tables in the database.
// The ApplicationDbContext class also defines a static Seed method that is used to seed the database with initial data.
// The Seed method checks if the Flashcards table is empty and adds some initial flashcard data if it is empty.
// The Seed method also checks if the FlashcardDecks table is empty and adds some initial flashcard deck data if it is empty.
// The ApplicationDbContext class is used to interact with the database and perform CRUD operations on the Flashcards and FlashcardDecks tables.
namespace PrototypeForAnkiEsque.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Flashcard> Flashcards { get; set; }
        public DbSet<FlashcardDeck> FlashcardDecks { get; set; }
        // The Seed method is used to seed the database with initial data.
        // The correct way to do this would be to create a separate class/service that handles seeding the database, but for simplicity, the Seed method is defined directly in the ApplicationDbContext class.
        // This will be changed to automatic imports in the future.
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

                if (!context.FlashcardDecks.Any())
                {
                    context.FlashcardDecks.AddRange(
                        new FlashcardDeck
                        {
                            Name = "動物 ー Animals",
                            FlashcardFronts = new List<string> {
                            "犬", "猫", "鳥",
                            "魚", "馬", "牛",
                            "羊", "豚"},
                            EaseRating = "100%"
                        }
                        );
                }

                context.SaveChanges();
            }
        }
    }
}
