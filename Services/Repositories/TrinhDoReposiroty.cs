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
    public class TrinhDoReposiroty : RepositoryBase<TrinhDo>, ITrinhDoRepository
    {
        public TrinhDoReposiroty(DbEHealthCare db) : base(db)
        {
        }
    }
}
