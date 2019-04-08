using P01_StudentSystem.Data;
using P01_StudentSystem.Data.Models;
using System;

namespace P01_StudentSystem
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using (var db = new StudentSystemContext())
            {
                db.Courses.Add(new Data.Models.Course
                {
                    Name = "Learn the goods",
                    Description = "Worth it!",
                    Price = 100m
                });

                db.Resources.Add(new Data.Models.Resource
                {
                    Name = "Res",
                    Url = "www",
                    ResourceType = Data.Models.ResourceType.Presentation,
                    CourseId = 1
                });

                string[] courseNames =
                {
                    "Advanced",
                    "Fun",
                    "Expert",
                    "Basics"
                };

                string[] languages =
                {
                    "C",
                    "C++",
                    "C#"
                };

                SeedCourses(courseNames, languages);

                var affectedRows = db.SaveChanges();
                Console.WriteLine(affectedRows);
            }
        }

        private static void SeedCourses(string[] courseNames, string[] languages)
        {
            using (var db = new StudentSystemContext())
            {
                foreach(var courseName in courseNames)
                {
                    foreach(var language in languages)
                    {
                        string co = $"{language} {courseName}";

                        var course = new Course
                        {
                            Name = co,
                            Description = "Desc",
                            Price = 99
                        };
                    }
                }
            }
        }
    }
}
