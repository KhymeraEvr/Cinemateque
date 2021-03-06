﻿using System;
using Cinemateque.DataAccess.Models;
using Cinemateque.DataAccess.Models.Movie;
using Microsoft.EntityFrameworkCore;

namespace Cinemateque.DataAccess
{
   public partial class CinematequeContext : DbContext
   {
      public CinematequeContext()
      {
         Database.EnsureCreated();
      }

      public CinematequeContext(DbContextOptions<CinematequeContext> options)
          : base(options)
      {
      }

      public virtual DbSet<Actor> Actors { get; set; }
      public virtual DbSet<Director> Directors { get; set; }
      public virtual DbSet<CrewMember> CrewMembers { get; set; }
      public virtual DbSet<MovieDataEntity> Movies { get; set; }
      public virtual DbSet<User> User { get; set; }
      public virtual DbSet<UserFilms> UserFilms { get; set; }
      public virtual DbSet<Film> Film { get; set; }

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
