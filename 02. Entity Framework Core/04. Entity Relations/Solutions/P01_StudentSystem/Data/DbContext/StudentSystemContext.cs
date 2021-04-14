using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {
        }

        public StudentSystemContext( DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelConfiguringStudent(modelBuilder);

            OnModelConfiguringCourse(modelBuilder);

            OnModelConfiguringResource(modelBuilder);

            OnModelCreatingHomework(modelBuilder);

            OnModelCreatingStudentCourses(modelBuilder);

        }

        private static void OnModelCreatingStudentCourses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(sc => new
                {
                    sc.StudentId,
                    sc.CourseId
                });

                entity
                    .HasOne(c => c.Student)
                    .WithMany(s => s.CourseEnrollments)
                    .HasForeignKey(c => c.StudentId)
                    .OnDelete(DeleteBehavior.Restrict); ;

                entity
                    .HasOne(s => s.Course)
                    .WithMany(c => c.StudentsEnrolled)
                    .HasForeignKey(s => s.CourseId)
                    .OnDelete(DeleteBehavior.Restrict); ;
            });
        }

        private static void OnModelCreatingHomework(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Homework>(entity =>
            {
                entity.HasKey(h => h.HomeworkId);

                entity
                    .Property(h => h.Content)
                    .IsUnicode(false)
                    .IsRequired(true);

                entity
                    .Property(h => h.ContentType)
                    .IsRequired(true);

                entity
                    .Property(h => h.SubmissionTime)
                    .IsRequired(true);

                entity
                    .HasOne(h => h.Student)
                    .WithMany(s => s.HomeworkSubmissions)
                    .HasForeignKey(h => h.StudentId);

                entity
                    .HasOne(h => h.Course)
                    .WithMany(c => c.HomeworkSubmissions)
                    .HasForeignKey(h => h.CourseId);
            });
        }

        private static void OnModelConfiguringResource(ModelBuilder modelBuilder)
        {
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
                .IsRequired(true);

                entity
                    .HasOne(r => r.Course)
                    .WithMany(c => c.Resources)
                    .HasForeignKey(c => c.CourseId);

            });
        }

        private static void OnModelConfiguringStudent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.StudentId);

                entity.Property(s => s.Name)
                .HasMaxLength(100)
                .IsUnicode(true)
                .IsRequired(true);

                entity.Property(s => s.PhoneNumber)
                .HasColumnType("CHAR(10)")
                .IsUnicode(false)
                .IsRequired(false);

                entity.Property(s => s.RegisteredOn)
                .IsRequired(true);
            });
        }

        private static void OnModelConfiguringCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.CourseId);

                entity.Property(c => c.Name)
                .HasMaxLength(80)
                .IsUnicode(true)
                .IsRequired(true);

                entity.Property(c => c.Description)
                .IsRequired(false)
                .IsUnicode(true);
                
                entity.Property(c => c.StartDate)
                .IsRequired(true);

                entity.Property(c => c.EndDate)
                .IsRequired(true);

                entity.Property(c => c.Price)
                .HasColumnType("DECIMAL(18,2)")
                .IsRequired(true);

            });
        }

    }
}
