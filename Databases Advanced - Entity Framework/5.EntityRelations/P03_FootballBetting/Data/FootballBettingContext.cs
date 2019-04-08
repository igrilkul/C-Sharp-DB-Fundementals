using Microsoft.EntityFrameworkCore;
using P03_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P03_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }

        public FootballBettingContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Bet> Bets {get;set;}
        public DbSet<Color> Colors {get;set;}
        public DbSet<Country> Countries {get;set;}
        public DbSet<Game> Games {get;set;}
        public DbSet<Player> Players {get;set;}
        public DbSet<PlayerStatistic> PlayerStatistics {get;set;}
        public DbSet<Position> Positions {get;set;}
        public DbSet<Team> Teams {get;set;}
        public DbSet<Town> Towns {get;set;}
        public DbSet<User> Users {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            ConfigureTeam(mb);
            ConfigureTown(mb);
            ConfigureGame(mb);
            ConfigurePlayerStatistic(mb);
            ConfigurePlayer(mb);
        }

        private void ConfigurePlayer(ModelBuilder mb)
        {
            mb.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.PlayerId);

                entity.HasOne(p => p.Team)
                .WithMany(z => z.Players);

                entity.HasOne(p => p.Position)
                .WithMany(e => e.Players);
            });
        }

        private void ConfigurePlayerStatistic(ModelBuilder mb)
        {
            mb.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(e => new { e.GameId, e.PlayerId });

                entity.HasOne(e => e.Player)
                .WithMany(p => p.PlayerStatistics);

                entity.HasOne(e => e.Game)
                .WithMany(p => p.PlayerStatistics);
                
            });
        
        }
        private void ConfigureGame(ModelBuilder mb)
        {
            mb.Entity<Game>(e =>
            {
                e.HasKey(t => t.GameId);

                e.HasOne(t => t.HomeTeam)
                .WithMany(y => y.HomeGames)
                .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(t => t.AwayTeam)
                .WithMany(y => y.AwayGames)
                .OnDelete(DeleteBehavior.Restrict);

                
            });
        }

        private void ConfigureTown(ModelBuilder mb)
        {
            mb.Entity<Town>(entity =>
            {
                entity.HasKey(t => t.TownId);

                entity.HasOne(c => c.Country)
                .WithMany(t => t.Towns);
            });
        }

        private static void ConfigureTeam(ModelBuilder mb)
        {
            mb.Entity<Team>(entity =>
            {
                entity.HasKey(p => p.TeamId);

                entity.HasOne(e => e.PrimaryKitColor)
                .WithMany(p => p.PrimaryKitTeams)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.SecondaryKitColor)
                .WithMany(p => p.SecondaryKitTeams)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Town)
                .WithMany(t => t.Teams);

                entity.HasMany(e => e.Players)
                .WithOne(t => t.Team);
            });
        }
    }
}
