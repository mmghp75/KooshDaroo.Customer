using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KooshDaroo.Models
{
    public class Pharmacy
    {
        public int id { get; set; }
        public string Title { get; set; }
        public bool is24h { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public string City { get; set; }
        public string PhoneNo { get; set; }
        public bool isInactive { get; set; }
    }
}