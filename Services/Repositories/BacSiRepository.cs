using DbEHealthcare;
using DbEHealthcare.Entities;
using Services.IRepositories;

namespace Services.Repositories
{
    public class BacSiRepository : RepositoryBase<BacSi>, IBacSiRepository
    {
        public BacSiRepository(DbEHealthCare db) : base(db)
        {
        }
    }
}
