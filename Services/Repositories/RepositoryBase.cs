using DbEHealthcare;
using DbEHealthcare.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services.Repositories
{
    public class RepositoryBase <T> where T : class
    {
        protected DbEHealthCare dbContext { set; get; }
        protected DbSet<T> dbComponent { set; get; }

        public RepositoryBase(DbEHealthCare db)
        {
            this.dbContext = db;
            this.dbComponent = db.Set<T>();
        }

        /// <summary>
        /// Thực hiện lấy danh sách của đối tượng
        /// </summary>
        /// <returns>return về list đối tượng </returns>
        public virtual IEnumerable<T> GetAll()
        {
            return dbComponent.ToList();
        }

        /// <summary>
        /// Lấy về một đối tượng dựa theo điều kiện truyền vào
        /// </summary>
        /// <param name="where">là một Expression </param>
        /// <returns>return về đối tượng nếu không có return null </returns>
        public virtual T GetBy(Expression<Func<T, bool>> where)
        {
            T t = dbComponent.Where(where).AsEnumerable().FirstOrDefault();
            return t;

        }

        /// <summary>
        /// Xóa đối tượng khỏi cơ sở dữ liệu
        /// </summary>
        /// <param name="t">đối tượng cần xóa</param>
        public virtual void Delete(T t)
        {
            dbComponent.Remove(t);
        }

        /// <summary>
        /// Delete đối tượng theo điều kiện(expression) truyền vào 
        /// </summary>
        /// <param name="where"></param>
        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            List<T> listT = dbComponent.Where(where).AsEnumerable().ToList();
            if (listT != null)
            {
                foreach (T t in listT)
                {
                    dbComponent.Remove(t);
                }
            }
        }

        /// <summary>
        /// Thực hiện cập nhật thông tin đối tượng
        /// </summary>
        /// <param name="t">Đối tượng cần cập nhật</param>
        public virtual void Edit(T t)
        {
            try
            {
                dbComponent.Attach(t);

                dbContext.Entry(t).State =EntityState.Modified;
            }
            catch 
            {
                dbContext.Set<T>().AddOrUpdate(t);
            }

        }

        /// <summary>
        /// Thực hiện tìm kiếm đối tượng theo Expression truyền vào
        /// </summary>
        /// <param name="where">Expression</param>
        /// <returns>true if exists else false </returns>
        public virtual bool FindBy(Expression<Func<T, bool>> where)
        {
            T t = dbComponent.Where(where).FirstOrDefault();

            if (t != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// trả về list theo biểu thức truyền vào
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetListBy(Expression<Func<T, bool>> expression)
        {
            List<T> list = dbComponent.Where(expression).AsEnumerable().ToList();
            return list;
        }

        /// <summary>
        /// them doi tuong moi vao database
        /// </summary>
        /// <param name="t"></param>
        public virtual void Add(T t)
        {
            dbComponent.Add(t);
        }
    }
}
