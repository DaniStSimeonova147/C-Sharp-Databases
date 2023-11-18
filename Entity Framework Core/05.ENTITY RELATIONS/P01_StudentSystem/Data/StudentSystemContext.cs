using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
   public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options) 
            : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);

                entity.Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired(true)
                .IsUnicode(true);

                entity.Property(s => s.PhoneNumber)
                .HasColumnType("CHAR(10)")
                .IsRequired(false)
                .IsUnicode(false);

                entity.Property(s => s.RegisteredOn)
                .IsRequired(true)
                .IsUnicode(true);

                entity.Property(s => s.Birthday)
                .IsRequired(false)
                .IsUnicode(true);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.CourseId);

                entity.Property(c => c.Name)
                .HasMaxLength(80)
                .IsRequired(true)
                .IsUnicode(true);

                entity.Property(c => c.Description)
                .IsRequired(false)
                .IsUnicode(true);

                entity.Property(c => c.StartDate)
                .IsRequired(true)
                .IsUnicode(true);

                entity.Property(c => c.EndDate)
                .IsRequired(true)
                .IsUnicode(true);

                entity.Property(c => c.Price)
                .IsRequired(true)
                .IsUnicode(true);
            });

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.HasKey(r => r.ResourceId);

                entity.Property(r => r.Name)
                .HasMaxLength(50)
                .IsRequired(true)
                .IsUnicode(true);

                entity.Property(r => r.Url)
                .IsRequired(true)
                .IsUnicode(false);

                entity.Property(r => r.ResourceType)
                .IsRequired(true)
                .IsUnicode(false);

                entity.Property(r => r.CourseId)
                .IsRequired(true)
                .IsUnicode(false);

                entity
                .HasOne(r => r.Course)
                .WithMany(c => c.Resources)
                .HasForeignKey(r => r.CourseId);
            });

            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(h => h.HomeworkId);

                entity.Property(h => h.Content)
                .HasMaxLength(250)
                .IsRequired(true)
                .IsUnicode(false);

                entity.Property(h => h.ContentType)
                .IsRequired(true)
                .IsUnicode(false);

                entity.Property(h => h.SubmissionTime)
                .IsRequired(true)
                .IsUnicode(true);

                entity.Property(h => h.StudentId)
                .IsRequired(true)
                .IsUnicode(true);

                entity
                .HasOne(h => h.Student)
                .WithMany(s => s.HomeworkSubmissions)
                .HasForeignKey(h => h.StudentId);

                entity.Property(h => h.CourseId)
                .IsRequired(true)
                .IsUnicode(true);

                entity
                .HasOne(h => h.Course)
                .WithMany(c => c.HomeworkSubmissions)
                .HasForeignKey(h => h.CourseId);
            });

            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(sc => new { sc.StudentId, sc.CourseId });

                entity
                .HasOne(c => c.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(c => c.StudentId);

                entity
                .HasOne(s => s.Course)
                .WithMany(c => c.StudentsEnrolled)
                .HasForeignKey(s => s.CourseId);
            });
        }
    }
}
