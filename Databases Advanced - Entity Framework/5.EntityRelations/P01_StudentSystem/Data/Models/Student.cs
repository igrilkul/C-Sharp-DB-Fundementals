using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
   public class Student
    {
        public Student()
        {
            
            this.RegisteredOn = DateTime.Now;
            
            this.CourseEnrollments = new List<StudentCourse>();
            this.HomeworkSubmissions = new List<Homework>();
        }

        public int StudentId { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime RegisteredOn { get;  set; }

        public DateTime? Birthday { get; set; }

        public ICollection<StudentCourse> CourseEnrollments { get; set; }

        public ICollection<Homework> HomeworkSubmissions { get; set; }
    }
}
