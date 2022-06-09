namespace DbEHealthcare.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BenhNhan")]
    public partial class BenhNhan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BenhNhan()
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

        [StringLength(100)]
        public string dc { get; set; }

        [StringLength(13)]
        public string sdt { get; set; }

        public bool? gioitinh { get; set; }

        [StringLength(200)]
        public string hinhanh { get; set; }

        [StringLength(100)]
        public string quoctich { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ntns { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichTuVan> LichTuVans { get; set; }
    }
}
