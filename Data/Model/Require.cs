using System;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace EApprove.Data.Model
{
    public class Required  
    {
        public int ID { get; set; }
        public string col1 { get; set; }
        public string col2 { get; set; }
        public int col3 { get; set; }
        public string col4 { get; set; }
        public DateTime col5 { get; set; }
        public bool col6 { get; set; }

    }

} 