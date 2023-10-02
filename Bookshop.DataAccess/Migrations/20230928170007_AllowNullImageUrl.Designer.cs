﻿// <auto-generated />
using System;
using Bookshop.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Bookshop.DataAccess.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20230928170007_AllowNullImageUrl")]
    partial class AllowNullImageUrl
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-rc.1.23419.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Bookshop.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            DisplayOrder = 1,
                            Name = "Action"
                        },
                        new
                        {
                            Id = 2,
                            DisplayOrder = 2,
                            Name = "SciFi"
                        },
                        new
                        {
                            Id = 3,
                            DisplayOrder = 3,
                            Name = "History"
                        },
                        new
                        {
                            Id = 4,
                            DisplayOrder = 4,
                            Name = "Poetry"
                        },
                        new
                        {
                            Id = 5,
                            DisplayOrder = 5,
                            Name = "Fantasy"
                        });
                });

            modelBuilder.Entity("Bookshop.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Price")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Author = "Elizabeth Moon",
                            CategoryId = 5,
                            Description = "Paksenarrion, a simple sheepfarmer's daughter, yearns for a life of adventure and glory, such as was known to heroes in songs and story. At age seventeen she runs away from home to join a mercenary company and begins her epic life . . . ",
                            ISBN = "978-0-671-72104-6",
                            ImageURL = "",
                            Price = 19.989999999999998,
                            Title = "The Deed of Paksenarrion"
                        },
                        new
                        {
                            Id = 2,
                            Author = "Olga Tokarczuk",
                            CategoryId = 3,
                            Description = "In the mid-eighteenth century, as new ideas -- and a new unrest -- begin to sweep the Continent, a young Jew of mysterious origins arrives in a village in Poland. Before long, he has changed not only his name but his persona; visited by what seem to be ecstatic experiences, Jacob Frank casts a charismatic spell that attracts an increasingly fervent following. ",
                            ISBN = "978-83-08-04939-6",
                            ImageURL = "",
                            Price = 10.99,
                            Title = "The Books of Jacob"
                        },
                        new
                        {
                            Id = 3,
                            Author = "Pat Barker",
                            CategoryId = 3,
                            Description = "The Ghost Road is the culminating masterpiece of Pat Barker's towering World War I fiction trilogy. The time of the novel is the closing months of the most senselessly savage of modern conflicts. In France, millions of men engaged in brutal trench warfare are all \"ghosts in the making.\" In England, psychologist William Rivers, with severe pangs of conscience, treats the mental casualties of the war to make them whole enough to fight again. One of these, Billy Prior, risen to the officer class from the working class, both courageous and sardonic, decides to return to France with his fellow officer, poet Wilfred Owen, to fight a war he no longer believes in. Meanwhile, Rivers, enfevered by influenza, returns in memory to his experience studying a South Pacific tribe whose ethos amounted to a culture of death. Across the gulf between his society and theirs, Rivers begins to form connections that cast new light on his--and our--understanding of war.",
                            ISBN = "978-0-4522-7672-7",
                            ImageURL = "",
                            Price = 8.9900000000000002,
                            Title = "The Ghost Road"
                        },
                        new
                        {
                            Id = 4,
                            Author = "Kazuo Ishiguro",
                            CategoryId = 2,
                            Description = "Hailsham seems like a pleasant English boarding school, far from the influences of the city. Its students are well tended and supported, trained in art and literature, and become just the sort of people the world wants them to be. But, curiously, they are taught nothing of the outside world and are allowed little contact with it.\nWithin the grounds of Hailsham, Kathy grows from schoolgirl to young woman, but it’s only when she and her friends Ruth and Tommy leave the safe grounds of the school (as they always knew they would) that they realize the full truth of what Hailsham is.",
                            ISBN = "1-4000-4339-5",
                            ImageURL = "",
                            Price = 15.99,
                            Title = "Never Let Me Go"
                        },
                        new
                        {
                            Id = 5,
                            Author = "Angela Carter",
                            CategoryId = 4,
                            Description = "Courted by the Prince of Wales and painted by Toulouse-Lautrec, she is an aerialiste extraordinaire and star of Colonel Kearney's circus. She is also part woman, part swan. Jack Walser, an American journalist, is on a quest to discover the truth behind her identity. Dazzled by his love for her, and desperate for the scoop of a lifetime, Walser has no choice but to join the circus on its magical tour through turn-of-the-nineteenth-century London, St Petersburg and Siberia.",
                            ISBN = "978-0-1400-7703-2",
                            ImageURL = "",
                            Price = 13.99,
                            Title = "Nights at the Circus"
                        },
                        new
                        {
                            Id = 6,
                            Author = "Elizabeth Strout",
                            CategoryId = 5,
                            Description = "At times stern, at other times patient, at times perceptive, at other times in sad denial, Olive Kitteridge, a retired schoolteacher, deplores the changes in her little town of Crosby, Maine, and in the world at large, but she doesn’t always recognize the changes in those around her: a lounge musician haunted by a past romance; a former student who has lost the will to live; Olive’s own adult child, who feels tyrannized by her irrational sensitivities; and her husband, Henry, who finds his loyalty to his marriage both a blessing and a curse.",
                            ISBN = "978-1-4000-6208-9",
                            ImageURL = "",
                            Price = 15.99,
                            Title = "Olive Kitteridge"
                        });
                });

            modelBuilder.Entity("Bookshop.Models.Product", b =>
                {
                    b.HasOne("Bookshop.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}