
using DbEHealthcare;
using DbEHealthcare.Entities;
using Services.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repositories
{
    public class ChiTietTuVanRepository : RepositoryBase<ChiTietTuVan>, IChiTietTuVanRepository
    {
        public ChiTietTuVanRepository(DbEHealthCare db) : base(db)
        {
        }
    }
}
