using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cinemateque.Models
{
    public partial class CinematequeContext : DbContext
    {
        public CinematequeContext()
        {
        }

        public CinematequeContext(DbContextOptions<CinematequeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Actor> Actor { get; set; }
        public virtual DbSet<Director> Director { get; set; }
        public virtual DbSet<Film> Film { get; set; }
        public virtual DbSet<FilmActors> FilmActors { get; set; }
        public virtual DbSet<FilmReward> FilmReward { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserFilms> UserFilms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=BF2142FOREVA\\TEW_SQLEXPRESS;Database=Cinemateque;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actor>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ActorName).HasMaxLength(30);
            });

            modelBuilder.Entity<Director>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DirectorName).HasMaxLength(50);
            });

            modelBuilder.Entity<Film>(entity =>
            {
                entity.Property(e => e.FilmName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Genre).HasMaxLength(50);

                entity.Property(e => e.PremiereDate).HasColumnType("datetime");

                entity.HasOne(d => d.Director)
                    .WithMany(p => p.Film)
                    .HasForeignKey(d => d.DirectorId)
                    .HasConstraintName("FK_Film_Director");
            });

            modelBuilder.Entity<FilmActors>(entity =>
            {
                entity.HasOne(d => d.Actor)
                    .WithMany(p => p.FilmActors)
                    .HasForeignKey(d => d.ActorId)
                    .HasConstraintName("FK_FilmActors_Actor");

                entity.HasOne(d => d.Film)
                    .WithMany(p => p.FilmActors)
                    .HasForeignKey(d => d.FilmId)
                    .HasConstraintName("FK_FilmActors_Film");
            });

            modelBuilder.Entity<FilmReward>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.RewardName).HasMaxLength(50);

                entity.HasOne(d => d.Film)
                    .WithMany(p => p.FilmReward)
                    .HasForeignKey(d => d.FilmId)
                    .HasConstraintName("FK_FilmReward_Film1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Passwrod).HasMaxLength(30);

                entity.Property(e => e.Role).HasMaxLength(50);

                entity.Property(e => e.Token).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            modelBuilder.Entity<UserFilms>(entity =>
            {
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Film)
                    .WithMany(p => p.UserFilms)
                    .HasForeignKey(d => d.FilmId)
                    .HasConstraintName("FK_UserFilms_Film");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserFilms)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserFilms_User");
            });
        }
    }
}
