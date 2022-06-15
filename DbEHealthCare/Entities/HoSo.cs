namespace DbEHealthcare.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HoSo")]
    public partial class HoSo
    {
        [Key]
        [StringLength(200)]
        public string email { get; set; }

        [StringLength(30)]
        public string tencv { get; set; }

        [StringLength(10)]
        public string sdt { get; set; }
    }
}
