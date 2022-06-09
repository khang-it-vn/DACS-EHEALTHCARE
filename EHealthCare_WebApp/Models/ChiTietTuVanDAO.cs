using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EHealthCare_WebApp.Models
{
    public class ChiTietTuVanDAO
    {
        public ChiTietTuVanDAO()
        {
        }

        public int id_cttv { get; set; }

        public string trieuChung { get; set; }
        public string chuanDoan { get; set; }

        public string chiDinh { get; set; }

        public bool ketQua { get; set; }

        public string ghiChu { get; set; }

    
    }
}