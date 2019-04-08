using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
   public class Course
    {
        public Course()
        {
            this.StartDate = DateTime.Now;
            this.EndDate = StartDate.AddDays(60);

            this.Resources = new List<Resource>();
            this.StudentsEnrolled = new List<StudentCourse>();
            this.HomeworkSubmissions = new List<Homework>();
        }

        public int CourseId { get; set; }

        public string Name { get;  set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        public ICollection<StudentCourse> StudentsEnrolled { get; set; }

        public ICollection<Resource> Resources { get; set; }

        public ICollection<Homework> HomeworkSubmissions { get; set; }
    }
}
