using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KooshDaroo.Models
{
    public class Prescribe
    {
        public int id { get; set; }
        public byte[] Prescription { get; set; }
        public DateTime DateOf { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public bool isCancelled { get; set; }
        public string PhoneNo { get; set; }
    }
}