﻿using System;
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

        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Flashcards.Any())
            {
                context.Flashcards.AddRange(new Flashcard { Front = "人", Back = "Human", EaseFactor = 0, Interval = 0, LastReviewed = DateTime.Today,
                NextReview = DateTime.Today});
                context.SaveChanges();
            }
        }
    }
}
