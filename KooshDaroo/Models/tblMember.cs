using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace KooshDaroo.Models
{
    public class tblMember
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string PhoneNo { get; set; }
        public DateTime LastPrescribeDate { get; set; }
    }
}
