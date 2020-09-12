using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KooshDaroo.Models
{
    public class PrescribeResource
    {
        public int id { get; set; }
        public int PrescribeId { get; set; }
        public int PharmacyId { get; set; }
        public bool Item01 { get; set; }
        public bool Item02 { get; set; }
        public bool Item03 { get; set; }
        public bool Item04 { get; set; }
        public bool Item05 { get; set; }
        public bool Item06 { get; set; }
        public bool Item07 { get; set; }
        public bool Item08 { get; set; }
        public bool Item09 { get; set; }
        public bool Item10 { get; set; }
        public bool PharmacyAccepted { get; set; }
        public DateTime PharmacyAcceptDateOf { get; set; }
        public bool MemberAccepted { get; set; }
        public DateTime MemberAcceptDateOf { get; set; }
        public bool MemberTakesDrugs { get; set; }
    }
}