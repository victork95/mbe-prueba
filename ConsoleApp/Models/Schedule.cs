using System;
using System.Collections.Generic;

namespace ConsoleApp.Models
{
    public partial class Schedule
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public string Actiondetail { get; set; } = null!;
        public DateTime Executiondate { get; set; }
        public bool Executed { get; set; } = false;
    }
}
