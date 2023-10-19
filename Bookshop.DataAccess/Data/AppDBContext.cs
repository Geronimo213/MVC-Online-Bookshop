using Microsoft.EntityFrameworkCore;
using Bookshop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Bookshop.DataAccess.Data
{
    public class AppDBContext : IdentityDbContext<IdentityUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderLines> OrderLines { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 },
                new Category { Id = 4, Name = "Poetry", DisplayOrder = 4 },
                new Category { Id = 5, Name = "Fantasy", DisplayOrder = 5}
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "The Deed of Paksenarrion",
                    Author = "Elizabeth Moon",
                    Description = "Paksenarrion, a simple sheepfarmer's daughter, yearns for a life of adventure and glory, such as was known to heroes in songs and story. At age seventeen she runs away from home to join a mercenary company and begins her epic life . . . ",
                    ISBN = "978-0-671-72104-6",
                    Price = 19.99,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 2,
                    Title = "The Books of Jacob",
                    Author = "Olga Tokarczuk",
                    Description = "In the mid-eighteenth century, as new ideas -- and a new unrest -- begin to sweep the Continent, a young Jew of mysterious origins arrives in a village in Poland. Before long, he has changed not only his name but his persona; visited by what seem to be ecstatic experiences, Jacob Frank casts a charismatic spell that attracts an increasingly fervent following. ",
                    ISBN = "978-83-08-04939-6",
                    Price = 10.99,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 3,
                    Title = "The Ghost Road",
                    Author = "Pat Barker",
                    Description = "The Ghost Road is the culminating masterpiece of Pat Barker's towering World War I fiction trilogy. The time of the novel is the closing months of the most senselessly savage of modern conflicts. In France, millions of men engaged in brutal trench warfare are all \"ghosts in the making.\" In England, psychologist William Rivers, with severe pangs of conscience, treats the mental casualties of the war to make them whole enough to fight again. One of these, Billy Prior, risen to the officer class from the working class, both courageous and sardonic, decides to return to France with his fellow officer, poet Wilfred Owen, to fight a war he no longer believes in. Meanwhile, Rivers, enfevered by influenza, returns in memory to his experience studying a South Pacific tribe whose ethos amounted to a culture of death. Across the gulf between his society and theirs, Rivers begins to form connections that cast new light on his--and our--understanding of war.",
                    ISBN = "978-0-4522-7672-7",
                    Price = 8.99,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 4,
                    Title = "Never Let Me Go",
                    Author = "Kazuo Ishiguro",
                    Description = "Hailsham seems like a pleasant English boarding school, far from the influences of the city. Its students are well tended and supported, trained in art and literature, and become just the sort of people the world wants them to be. But, curiously, they are taught nothing of the outside world and are allowed little contact with it.\nWithin the grounds of Hailsham, Kathy grows from schoolgirl to young woman, but it’s only when she and her friends Ruth and Tommy leave the safe grounds of the school (as they always knew they would) that they realize the full truth of what Hailsham is.",
                    ISBN = "1-4000-4339-5",
                    Price = 15.99,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 5,
                    Title = "Nights at the Circus",
                    Author = "Angela Carter",
                    Description = "Courted by the Prince of Wales and painted by Toulouse-Lautrec, she is an aerialiste extraordinaire and star of Colonel Kearney's circus. She is also part woman, part swan. Jack Walser, an American journalist, is on a quest to discover the truth behind her identity. Dazzled by his love for her, and desperate for the scoop of a lifetime, Walser has no choice but to join the circus on its magical tour through turn-of-the-nineteenth-century London, St Petersburg and Siberia.",
                    ISBN = "978-0-1400-7703-2",
                    Price = 13.99,
                    ImageURL = ""
                },
                new Product
                {
                    Id = 6,
                    Title = "Olive Kitteridge",
                    Author = "Elizabeth Strout",
                    Description = "At times stern, at other times patient, at times perceptive, at other times in sad denial, Olive Kitteridge, a retired schoolteacher, deplores the changes in her little town of Crosby, Maine, and in the world at large, but she doesn’t always recognize the changes in those around her: a lounge musician haunted by a past romance; a former student who has lost the will to live; Olive’s own adult child, who feels tyrannized by her irrational sensitivities; and her husband, Henry, who finds his loyalty to his marriage both a blessing and a curse.",
                    ISBN = "978-1-4000-6208-9",
                    Price = 15.99,
                    ImageURL = ""
                }
            );
        }

    }
}
