using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bookshop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreateCategoryProductAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Action" },
                    { 2, 2, "SciFi" },
                    { 3, 3, "History" },
                    { 4, 4, "Poetry" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "Price", "Title" },
                values: new object[,]
                {
                    { 1, "Elizabeth Moon", "Paksenarrion, a simple sheepfarmer's daughter, yearns for a life of adventure and glory, such as was known to heroes in songs and story. At age seventeen she runs away from home to join a mercenary company and begins her epic life . . . ", "978-0-671-72104-6", 19.989999999999998, "The Deed of Paksenarrion" },
                    { 2, "Olga Tokarczuk", "In the mid-eighteenth century, as new ideas -- and a new unrest -- begin to sweep the Continent, a young Jew of mysterious origins arrives in a village in Poland. Before long, he has changed not only his name but his persona; visited by what seem to be ecstatic experiences, Jacob Frank casts a charismatic spell that attracts an increasingly fervent following. ", "978-83-08-04939-6", 10.99, "The Books of Jacob" },
                    { 3, "Pat Barker", "The Ghost Road is the culminating masterpiece of Pat Barker's towering World War I fiction trilogy. The time of the novel is the closing months of the most senselessly savage of modern conflicts. In France, millions of men engaged in brutal trench warfare are all \"ghosts in the making.\" In England, psychologist William Rivers, with severe pangs of conscience, treats the mental casualties of the war to make them whole enough to fight again. One of these, Billy Prior, risen to the officer class from the working class, both courageous and sardonic, decides to return to France with his fellow officer, poet Wilfred Owen, to fight a war he no longer believes in. Meanwhile, Rivers, enfevered by influenza, returns in memory to his experience studying a South Pacific tribe whose ethos amounted to a culture of death. Across the gulf between his society and theirs, Rivers begins to form connections that cast new light on his--and our--understanding of war.", "978-0-4522-7672-7", 8.9900000000000002, "The Ghost Road" },
                    { 4, "Kazuo Ishiguro", "Hailsham seems like a pleasant English boarding school, far from the influences of the city. Its students are well tended and supported, trained in art and literature, and become just the sort of people the world wants them to be. But, curiously, they are taught nothing of the outside world and are allowed little contact with it.\nWithin the grounds of Hailsham, Kathy grows from schoolgirl to young woman, but it’s only when she and her friends Ruth and Tommy leave the safe grounds of the school (as they always knew they would) that they realize the full truth of what Hailsham is.", "1-4000-4339-5", 15.99, "Never Let Me Go" },
                    { 5, "Angela Carter", "Courted by the Prince of Wales and painted by Toulouse-Lautrec, she is an aerialiste extraordinaire and star of Colonel Kearney's circus. She is also part woman, part swan. Jack Walser, an American journalist, is on a quest to discover the truth behind her identity. Dazzled by his love for her, and desperate for the scoop of a lifetime, Walser has no choice but to join the circus on its magical tour through turn-of-the-nineteenth-century London, St Petersburg and Siberia.", "978-0-1400-7703-2", 13.99, "Nights at the Circus" },
                    { 6, "Elizabeth Strout", "At times stern, at other times patient, at times perceptive, at other times in sad denial, Olive Kitteridge, a retired schoolteacher, deplores the changes in her little town of Crosby, Maine, and in the world at large, but she doesn’t always recognize the changes in those around her: a lounge musician haunted by a past romance; a former student who has lost the will to live; Olive’s own adult child, who feels tyrannized by her irrational sensitivities; and her husband, Henry, who finds his loyalty to his marriage both a blessing and a curse.", "978-1-4000-6208-9", 15.99, "Olive Kitteridge" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
