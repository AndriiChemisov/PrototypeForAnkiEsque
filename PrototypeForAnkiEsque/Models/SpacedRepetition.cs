using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototypeForAnkiEsque.Models
{
    public static class SpacedRepetition
    {
        public static void UpdateCard(Flashcard card, int quality)
        {
            if (quality >= 3)
            {
                switch (card.Interval)
                {
                    case 0:
                        card.Interval = 1;
                        break;
                    case 1:
                        card.Interval = 6;
                        break;
                    default:
                        card.Interval = (int)(card.Interval * card.EaseFactor);
                        break;
                }

                // EaseFactor should not go below 1.3, and ideally it should never go below that limit
                card.EaseFactor = (int)(Math.Max(1.3, card.EaseFactor - 0.1f));
            }
            else
            {
                // If the quality is less than 3, reset the interval and decrease ease factor
                card.Interval = 1;
                card.EaseFactor = (int)(Math.Max(1.3, card.EaseFactor - 0.2f));
            }

            // Set the date for the next review based on the interval and current date
            card.NextReview = DateTime.Now.AddDays(card.Interval);
            card.LastReviewed = DateTime.Now;
        }

    }
}
