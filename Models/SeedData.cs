using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BookM.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BookMContext(
                serviceProvider.GetRequiredService<DbContextOptions<BookMContext>>()))
            {
                // 1. Check if database exists
                // context.Database.EnsureCreated(); // Optional: Use only if not using Migrations

                // 2. Look for any events. If there are events, we don't seed.
                if (context.Events.Any())
                {
                    return;   // DB has been seeded
                }

                // 3. Create Categories first
                var catCinema = new Category { Name = "Cinema" };
                var catConcert = new Category { Name = "Concert" };
                var catSport = new Category { Name = "Sport" };
                var catTheater = new Category { Name = "Theater" };

                context.Category.AddRange(catCinema, catConcert, catSport, catTheater);
                context.SaveChanges(); // Save categories to get their IDs

                // 4. Create Events
                context.Events.AddRange(
                    new Event
                    {
                        Title = "Mawazine Festival - Opening Night",
                        Description = "The biggest music festival in Morocco returns with international stars.",
                        EventDate = DateTime.Now.AddDays(10),
                        Location = "OLM Souissi, Rabat",
                        Capacity = 50000,
                        Ltype = "Outdoor", // Or 'LType' property if you named it that
                        CategoryId = catConcert.CategoryId,
                        ImageUrl = "https://images.unsplash.com/photo-1459749411177-2a296581dca0?auto=format&fit=crop&w=600&q=80"
                    },
                    new Event
                    {
                        Title = "Wydad vs Raja - The Derby",
                        Description = "The most anticipated football match of the season.",
                        EventDate = DateTime.Now.AddDays(3),
                        Location = "Complexe Mohammed V, Casablanca",
                        Capacity = 45000,
                        Ltype = "Stadium",
                        CategoryId = catSport.CategoryId,
                        ImageUrl = "https://images.unsplash.com/photo-1508098682722-e99c43a406b2?auto=format&fit=crop&w=600&q=80"
                    },
                    new Event
                    {
                        Title = "Inception - Special Screening",
                        Description = "Watch the Christopher Nolan masterpiece in IMAX.",
                        EventDate = DateTime.Now.AddDays(1),
                        Location = "Megarama, Casablanca",
                        Capacity = 200,
                        Ltype = "Indoor",
                        CategoryId = catCinema.CategoryId,
                        ImageUrl = "https://images.unsplash.com/photo-1536440136628-849c177e76a1?auto=format&fit=crop&w=600&q=80"
                    },
                    new Event
                    {
                        Title = "Marrakech du Rire",
                        Description = "An evening of laughter with the best comedians.",
                        EventDate = DateTime.Now.AddMonths(1),
                        Location = "Palais El Badi, Marrakech",
                        Capacity = 1500,
                        Ltype = "Outdoor",
                        CategoryId = catTheater.CategoryId,
                        ImageUrl = "https://images.unsplash.com/photo-1585699324551-f6c309eedeca?auto=format&fit=crop&w=600&q=80"
                    }
                );

                // 5. Save everything to SQL
                context.SaveChanges();
            }
        }
    }
}