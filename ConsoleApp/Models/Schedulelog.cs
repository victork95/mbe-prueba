using System;
using System.Collections.Generic;

namespace ConsoleApp.Models
{
    public partial class Schedulelog
    {
        public int Id { get; set; }
        public int Schedulelogid { get; set; }
        public bool Success { get; set; }
        public string Actionresult { get; set; } = null!;
        public DateTime Executiondate { get; set; }
    }
}
