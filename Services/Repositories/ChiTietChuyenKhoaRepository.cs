using DbEHealthcare.Entities;
using Services.IRepositories;
using Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbEHealthcare;

namespace Services.Repositories
{
    public class ChiTietChuyenKhoaRepository : RepositoryBase<ChiTietChuyenKhoa>, IChiTietChuyenKhoaRepository
    {
        public ChiTietChuyenKhoaRepository(DbEHealthCare db) : base(db)
        {
        }
    }
}
