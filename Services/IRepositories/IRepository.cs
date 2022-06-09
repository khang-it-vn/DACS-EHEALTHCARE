using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.IRepositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();    

        T GetBy(Expression<Func<T,bool>> expression); 

        bool FindBy(Expression<Func<T,bool>> expression);

        void Delete(T t);

        void Edit(T t);

        void Add(T t);
    }
    
}
