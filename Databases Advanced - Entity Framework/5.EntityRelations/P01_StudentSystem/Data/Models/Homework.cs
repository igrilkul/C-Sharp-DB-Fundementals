using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        public Homework()
        {
            this.SubmissionTime = DateTime.Now;
        }
        public int HomeworkId { get; set; }

        public string Content { get; set; }

        public ContentType ContentType { get; set; }

        public DateTime SubmissionTime { get;  set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }

    public enum ContentType
    {
        Application = 1,
        Pdf = 2,
        Zip = 3
    }
}
