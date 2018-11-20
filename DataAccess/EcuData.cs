using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess
{
    [Table("EcuData")]
    public class EcuData
    {
        public int Id { get; set; }

        public decimal Afr { get; set; }

        public decimal ManifoldPressureKpa { get; set; }

        public DateTime Timestamp { get; set; }
    }
}