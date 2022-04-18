using System;
using SQLite;

namespace DegreePlan.Classes
{
    [Table("tblAssessments")]
    public class Assess
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public int CourseId { get; set; }
        public bool Notify { get; set; }
    }
}
