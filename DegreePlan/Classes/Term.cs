﻿using System;
using SQLite;

namespace DegreePlan.Classes
{
    [Table("tblTerms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
