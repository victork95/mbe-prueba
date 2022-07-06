using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace ConsoleApp.Models
{
    public partial class testContext : DbContext
    {
        public testContext()
        {
        }

        public testContext(DbContextOptions<testContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<Schedulelog> Schedulelogs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false).Build();

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(config.GetConnectionString("ScheduleDatabase"), ServerVersion.Parse("10.5.9-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("latin1_swedish_ci")
                .HasCharSet("latin1");

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("schedule");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Action)
                    .HasMaxLength(50)
                    .HasColumnName("action");

                entity.Property(e => e.Actiondetail)
                    .HasMaxLength(555)
                    .HasColumnName("actiondetail");

                entity.Property(e => e.Executiondate)
                    .HasColumnType("datetime")
                    .HasColumnName("executiondate");

                entity.Property(e => e.Executed)
                    .HasDefaultValue(false)
                    .HasColumnName("executed");
            });

            modelBuilder.Entity<Schedulelog>(entity =>
            {
                entity.ToTable("schedulelog");

                entity.Property(e => e.Id)
                    .HasColumnType("int(11)")
                    .HasColumnName("id");

                entity.Property(e => e.Actionresult)
                    .HasMaxLength(100)
                    .HasColumnName("actionresult");

                entity.Property(e => e.Executiondate)
                    .HasColumnType("datetime")
                    .HasColumnName("executiondate");

                entity.Property(e => e.Schedulelogid)
                    .HasColumnType("int(11)")
                    .HasColumnName("schedulelogid");

                entity.Property(e => e.Success).HasColumnName("success");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
