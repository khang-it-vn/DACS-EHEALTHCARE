using DbEHealthcare.Entities;
using Services.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbEHealthcare;

namespace Services.Repositories
{
    public class AdminRepository : RepositoryBase<Admin>, IAdminRepository
    {
        public AdminRepository(DbEHealthCare db) : base(db)
        {
        }
    }
}
