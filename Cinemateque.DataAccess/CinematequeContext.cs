using System;
using Cinemateque.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace Cinemateque.DataAccess
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

      public virtual DbSet<Actor> Actors { get; set; }
      public virtual DbSet<Director> Directors { get; set; }
      public virtual DbSet<CrewMember> CrewMembers { get; set; }
      public virtual DbSet<Order> Order { get; set; }
      public virtual DbSet<User> User { get; set; }
      public virtual DbSet<UserFilms> UserFilms { get; set; }

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
         if (!optionsBuilder.IsConfigured)
         {
            throw new Exception("No sql connection string");
         }
      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<Actor>(entity =>
         {
            entity.Property(e => e.ActorName).HasMaxLength(30);
         });

         modelBuilder.Entity<Director>(entity =>
         {
            entity.Property(e => e.DirectorName).HasMaxLength(50);
         });                

         modelBuilder.Entity<Order>(entity =>
         {
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Film)
                   .WithMany(p => p.Order)
                   .HasForeignKey(d => d.FilmId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_Order_Film");

            entity.HasOne(d => d.User)
                   .WithMany(p => p.Order)
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_Order_User");
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
