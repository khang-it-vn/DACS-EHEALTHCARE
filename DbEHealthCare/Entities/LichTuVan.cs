namespace DbEHealthcare.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LichTuVan")]
    public partial class LichTuVan
    {
        [Key]
        [Column(Order = 0)]
        public DateTime ntv { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(200)]
        public string email_BS { get; set; }

        [StringLength(200)]
        public string email_BN { get; set; }

        public int? id_cttv { get; set; }

        public int? phongtuvan { get; set; }

        public virtual BacSi BacSi { get; set; }

        public virtual BenhNhan BenhNhan { get; set; }

        public virtual ChiTietTuVan ChiTietTuVan { get; set; }

        public bool trangthai { get; set; }
    }
}
