using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class HeroDbContext(DbContextOptions<HeroDbContext> options) : DbContext(options)
    {
        public DbSet<Hero> Heroes => Set<Hero>();
        public DbSet<Role> Roles => Set<Role>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Hero>().HasData(
                new Hero
                {
                    Id = 1,
                    Name = "Gloo",
                    ImageUrl = "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage_1_9_642/100_8b401d50920f2359060a9c7a3c833df1.png",
                    Description = "A mysterious creature that can split into many smaller ones."
                },
                new Hero
                {
                    Id = 2,
                    Name = "Lukas",
                    ImageUrl = "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage_1_9_20/100_454c13b2de7b7d1a20fbf553c620510d.png",
                    Description = "A legendary Sacred Beast that can take the form of a ranbunctious young man."
                },
                new Hero
                {
                    Id = 3,
                    Name = "Nolan",
                    ImageUrl = "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage/100_0495066df0d828c149e7fe89aa63078b.png",
                    Description = "A scholar that wanders the universe with split souls to save his daughter."
                },
                new Hero
                {
                    Id = 4,
                    Name = "Zhuxin",
                    ImageUrl = "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage_92/100_13cfeec4bec7a27a09677e519f1ef9d2.png",
                    Description = "A mysterious young woman who guides the ember butterflies using her Lantern ..."
                },
                new Hero
                {
                    Id = 5,
                    Name = "Hanabi",
                    ImageUrl = "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage/100_85d213390613bbc09220cf1d9f64c5c0.png",
                    Description = "Leader of the Scarlet Sect, in the Scarlet Shadow of the Cadia Riverlands."
                },
                new Hero
                {
                    Id = 6,
                    Name = "Lesley",
                    ImageUrl = "https://akmweb.youngjoygame.com/web/svnres/img/mlbb/homepage/100_f4f42410c90f84e4d46b129d5e8887e8.png",
                    Description = "Adopted daughter of House Vance, a clandestine sniper."
                }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = 1,
                    HeroRole = "Tank",
                    LogoUrl = "https://static.wikia.nocookie.net/mobile-legends/images/f/f0/Tank_Icon.png",
                    PrimaryFunction = "Protect teammates, soak damage, and initiate team fights.",
                    KeyAttributes = "High health, defense, and crowd control."
                },
                new Role
                {
                    Id = 2,
                    HeroRole = "Fighter",
                    LogoUrl = "https://static.wikia.nocookie.net/mobile-legends/images/1/1a/Fighter_Icon.png",
                    PrimaryFunction = "Balance damage and durability, capable of engaging in fights and soaking damage.",
                    KeyAttributes = "Balanced stats, good damage output, and decent survivability."
                },
                new Role
                {
                    Id = 3,
                    HeroRole = "Assassin",
                    LogoUrl = "https://static.wikia.nocookie.net/mobile-legends/images/3/3f/Assassin_Icon.png",
                    PrimaryFunction = "Quickly eliminate enemy heroes in team fights.",
                    KeyAttributes = "High burst damage, mobility, and stealth."
                },
                new Role
                {
                    Id = 4,
                    HeroRole = "Mage",
                    LogoUrl = "https://static.wikia.nocookie.net/mobile-legends/images/5/53/Mage_Icon.png",
                    PrimaryFunction = "Deal high magic damage, often with range and crowd control.",
                    KeyAttributes = "High magic power, magical damage, and often crowd control."
                },
                new Role
                {
                    Id = 5,
                    HeroRole = "Marksman",
                    LogoUrl = "https://static.wikia.nocookie.net/mobile-legends/images/1/10/Marksman_Icon.png",
                    PrimaryFunction = "Deal high physical damage, primarily from a distance.",
                    KeyAttributes = "High attack speed, physical damage, and ranged attack."
                }
            );

            modelBuilder.Entity("HeroRole").HasData(
                new { HeroesId = 1, RolesId = 1 },
                new { HeroesId = 2, RolesId = 2 },
                new { HeroesId = 3, RolesId = 3 },
                new { HeroesId = 4, RolesId = 4 },
                new { HeroesId = 5, RolesId = 5 },
                new { HeroesId = 6, RolesId = 3 },
                new { HeroesId = 6, RolesId = 5 }
            );
        }
    }
}
