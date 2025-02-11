﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PrototypeForAnkiEsque.Data;

#nullable disable

namespace PrototypeForAnkiEsque.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250204150113_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PrototypeForAnkiEsque.Models.Flashcard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Back")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EaseFactor")
                        .HasColumnType("int");

                    b.Property<string>("Front")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Interval")
                        .HasColumnType("int");

                    b.Property<DateTime>("LastReviewed")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("NextReview")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Flashcards");
                });
#pragma warning restore 612, 618
        }
    }
}
