namespace DbEHealthcare.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChuyenKhoa")]
    public partial class ChuyenKhoa
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChuyenKhoa()
        {
            ChiTietChuyenKhoas = new HashSet<ChiTietChuyenKhoa>();
        }

        [Key]
        public int ma_CK { get; set; }

        [Required]
        [StringLength(500)]
        public string ten_CK { get; set; }

        [Required]
        [StringLength(500)]
        public string mota_CK { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietChuyenKhoa> ChiTietChuyenKhoas { get; set; }
    }
}
