namespace DbEHealthcare.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChiTietChuyenKhoa")]
    public partial class ChiTietChuyenKhoa
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(200)]
        public string email_BS { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ma_CK { get; set; }

        public bool? mota { get; set; }

        public virtual BacSi BacSi { get; set; }

        public virtual ChuyenKhoa ChuyenKhoa { get; set; }
    }
}
