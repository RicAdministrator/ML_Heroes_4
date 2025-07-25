using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Heroes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heroes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeroRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrimaryFunction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyAttributes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeroRole",
                columns: table => new
                {
                    HeroesId = table.Column<int>(type: "int", nullable: false),
                    RolesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroRole", x => new { x.HeroesId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_HeroRole_Heroes_HeroesId",
                        column: x => x.HeroesId,
                        principalTable: "Heroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeroRole_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Heroes",
                columns: new[] { "Id", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "A mysterious creature that can split into many smaller ones.", "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage_1_9_642/100_8b401d50920f2359060a9c7a3c833df1.png", "Gloo" },
                    { 2, "A legendary Sacred Beast that can take the form of a ranbunctious young man.", "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage_1_9_20/100_454c13b2de7b7d1a20fbf553c620510d.png", "Lukas" },
                    { 3, "A scholar that wanders the universe with split souls to save his daughter.", "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage/100_0495066df0d828c149e7fe89aa63078b.png", "Nolan" },
                    { 4, "A mysterious young woman who guides the ember butterflies using her Lantern ...", "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage_92/100_13cfeec4bec7a27a09677e519f1ef9d2.png", "Zhuxin" },
                    { 5, "Leader of the Scarlet Sect, in the Scarlet Shadow of the Cadia Riverlands.", "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage/100_85d213390613bbc09220cf1d9f64c5c0.png", "Hanabi" },
                    { 6, "Adopted daughter of House Vance, a clandestine sniper.", "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage/100_f4f42410c90f84e4d46b129d5e8887e8.png", "Lesley" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "HeroRole", "KeyAttributes", "LogoUrl", "PrimaryFunction" },
                values: new object[,]
                {
                    { 1, "Tank", "High health, defense, and crowd control.", "https://static.wikia.nocookie.net/mobile-legends/images/f/f0/Tank_Icon.png", "Protect teammates, soak damage, and initiate team fights." },
                    { 2, "Fighter", "Balanced stats, good damage output, and decent survivability.", "https://static.wikia.nocookie.net/mobile-legends/images/1/1a/Fighter_Icon.png", "Balance damage and durability, capable of engaging in fights and soaking damage." },
                    { 3, "Assassin", "High burst damage, mobility, and stealth.", "https://static.wikia.nocookie.net/mobile-legends/images/3/3f/Assassin_Icon.png", "Quickly eliminate enemy heroes in team fights." },
                    { 4, "Mage", "High magic power, magical damage, and often crowd control.", "https://static.wikia.nocookie.net/mobile-legends/images/5/53/Mage_Icon.png", "Deal high magic damage, often with range and crowd control." },
                    { 5, "Marksman", "High attack speed, physical damage, and ranged attack.", "https://static.wikia.nocookie.net/mobile-legends/images/1/10/Marksman_Icon.png", "Deal high physical damage, primarily from a distance." }
                });

            migrationBuilder.InsertData(
                table: "HeroRole",
                columns: new[] { "HeroesId", "RolesId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 3 },
                    { 6, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeroRole_RolesId",
                table: "HeroRole",
                column: "RolesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeroRole");

            migrationBuilder.DropTable(
                name: "Heroes");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
