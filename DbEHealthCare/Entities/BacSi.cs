namespace DbEHealthcare.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BacSi")]
    public partial class BacSi
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BacSi()
        {
            LichTuVans = new HashSet<LichTuVan>();
        }

        [Key]
        [StringLength(200)]
        public string email { get; set; }

        [StringLength(100)]
        public string matkhau { get; set; }

        [StringLength(100)]
        public string hoten { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ntns { get; set; }

        [StringLength(100)]
        public string tdcm { get; set; }

        [StringLength(200)]
        public string dc_bv_tt { get; set; }

        [StringLength(200)]
        public string hinhanh { get; set; }

        [StringLength(500)]
        public string kinhnghiem { get; set; }

        public bool? gioitinh { get; set; }

        public int? ma_CK { get; set; }

        public virtual ChuyenKhoa ChuyenKhoa { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichTuVan> LichTuVans { get; set; }
    }
}
