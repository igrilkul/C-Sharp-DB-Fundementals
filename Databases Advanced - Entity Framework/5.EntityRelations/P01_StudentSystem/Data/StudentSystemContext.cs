using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data
{
   public class StudentSystemContext:DbContext
    {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options)
            :base(options)
        {

        }
        
        public DbSet<Student> Students { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            ConfigureStudentEntity(mb);
            ConfigureCourseEntity(mb);
            ConfigureResourceEntity(mb);
            ConfigureHomeworkEntity(mb);
            ConfigureStudentCourseEntity(mb);
        }

        private void ConfigureStudentCourseEntity(ModelBuilder mb)
        {
            mb.Entity<StudentCourse>(entity =>
            {
                entity.HasKey(e => new { e.StudentId, e.CourseId });

                entity.HasOne(e => e.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(e => e.StudentId);

                entity.HasOne(e => e.Course)
                .WithMany(s => s.StudentsEnrolled)
                .HasForeignKey(e => e.CourseId);
            }); 
        }

        private void ConfigureHomeworkEntity(ModelBuilder mb)
        {
            mb.Entity<Homework>(entity =>
            {
                entity.HasKey(e => e.HomeworkId);

                entity.Property(e => e.Content)
                .IsUnicode(false)
                .IsRequired();

                entity.HasOne(s => s.Student)
                .WithMany(h => h.HomeworkSubmissions);

                entity.HasOne(c => c.Course)
                .WithMany(h => h.HomeworkSubmissions);
            });
        }

        private void ConfigureResourceEntity(ModelBuilder mb)
        {
            mb.Entity<Resource>(entity =>
            {
                entity.HasKey(e => e.ResourceId);

                entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode();

                entity.Property(e => e.Url)
                .IsUnicode(false);

                entity.HasOne(e => e.Course)
                .WithMany(c => c.Resources);
            });
        }

        private void ConfigureCourseEntity(ModelBuilder mb)
        {
            mb.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId);

                entity.Property(e => e.Name)
                .HasMaxLength(80)
                .IsUnicode()
                .IsRequired();

                entity.Property(e => e.Description)
                .IsUnicode()
                .IsRequired(false);
            });
        }

        private static void ConfigureStudentEntity(ModelBuilder mb)
        {
            mb.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);

                entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode()
                .IsRequired();

                entity.Property(e => e.PhoneNumber)
                .HasColumnType("CHAR(10)")
                .IsRequired(false);

                entity.Property(e => e.Birthday)
                .IsRequired(false);

                entity.HasMany(e => e.HomeworkSubmissions)
                .WithOne(s => s.Student);
            });
        }
    }
}
