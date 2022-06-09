namespace DbEHealthcare.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChiTietTuVan")]
    public partial class ChiTietTuVan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ChiTietTuVan()
        {
            LichTuVans = new HashSet<LichTuVan>();
        }

        [Key]
        public int id_cttv { get; set; }

        [StringLength(500)]
        public string trieuChung { get; set; }

        [StringLength(500)]
        public string chuanDoan { get; set; }

        [StringLength(500)]
        public string chiDinh { get; set; }

        public bool? ketQua { get; set; }

        [StringLength(100)]
        public string ghiChu { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LichTuVan> LichTuVans { get; set; }
    }
}
