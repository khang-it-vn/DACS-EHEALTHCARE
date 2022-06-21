using DbEHealthcare;
using DbEHealthcare.Entities;
using Services.IRepositories;
using Services.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EHealthCareService
    {
        private static EHealthCareService instance;

        private DbEHealthCare dbContext { set; get; }

        public static EHealthCareService Instance
        {
            get
            {
                if (instance == null)
                    instance = new EHealthCareService();
                return instance;
            }

            private set
            {
                instance = value;
            }
        }

        private IChuyenKhoaRepository chuyenKhoas;

        

        public BenhNhan getBenhNhan(Expression<Func<BenhNhan, bool>> expression)
        {
            return benhNhans.GetBy(expression);
        }

        public BacSi LoginBs(BacSi bs)
        {
            throw new NotImplementedException();
        }

        private IBacSiRepository bacSis;

        private IBenhNhanRepository benhNhans;

        private IChiTietTuVanRepository chiTietTuVans;

      

        private ILichTuVanRepository lichTuVans;


        private EHealthCareService()
        {
            dbContext = new DbEHealthCare();
            this.chuyenKhoas = new ChuyenKhoaRepository(dbContext);
            this.bacSis = new BacSiRepository(dbContext);
            this.benhNhans = new BenhNhanRepository(dbContext);
            this.chiTietTuVans = new ChiTietTuVanRepository(dbContext);
            this.lichTuVans = new LichTuVanRepository(dbContext);
        }

        public void AddChiTietTuVan(ChiTietTuVan cttv)
        {
            chiTietTuVans.Add(cttv);
        }

        public void UpdateBN( BenhNhan bn)
        {
            benhNhans.Edit(bn);
            dbContext.Entry(bn).State = EntityState.Modified;
            dbContext.SaveChanges();
            // dbContext.Entry<BenhNhan>(bn).Reload();
        }

        public BenhNhan Login(BenhNhan bn)
        {
            return benhNhans.GetBy(b => b.email == bn.email && b.matkhau == bn.matkhau);
        }

        public BacSi getBacSiBy(Expression<Func<BacSi,bool>> expression)
        {
            return bacSis.GetBy(expression);
        }

        public List<ChuyenKhoa> getChuyenKhoas()
        {
            return chuyenKhoas.GetAll() as List<ChuyenKhoa>;
        }

        public void Save()
        {
            dbContext.SaveChanges();
        }

        public List<BacSi> getBacSis()
        {
            return bacSis.GetAll().ToList();
        }

        public List<LichTuVan> getLichTuVanBy(BacSi bs)
        {
            List<LichTuVan> ltvs = (List<LichTuVan>)lichTuVans.GetAll();
            return ltvs.Where(l => l.email_BS.CompareTo(bs.email) == 0).ToList();
        }

        public void Add(LichTuVan ltv)
        {
            lichTuVans.Add(ltv);
        }

        public List<LichTuVan> getLichTuVans()
        {
            return lichTuVans.GetAll() as List<LichTuVan>;
        }

        public void Edit(LichTuVan ltv)
        {
            lichTuVans.Edit(ltv);
        }

        public ChiTietTuVan getChiTietTuVan(int id)
        {
            return chiTietTuVans.GetBy(ct => ct.id_cttv == id);
        }

        public void UpdateBS(BacSi bacsi)
        {
            bacSis.Edit(bacsi);
        }

        public List<ChiTietTuVan> getChiTietTuVans()
        {
            return chiTietTuVans.GetAll().ToList();
        }

        public LichTuVan getLichTuVanBy(Expression<Func<LichTuVan, bool>> where)
        {
            return lichTuVans.GetBy(where);

        }

        public void Add(BenhNhan bn)
        {
            benhNhans.Add(bn);
        }

        public void UpdateCTTV(ChiTietTuVan cttv)
        {
            chiTietTuVans.Edit(cttv);
        }

        public void Delete(LichTuVan ltv)
        {
            lichTuVans.Delete(ltv);
        }
        public bool MailExists(BenhNhan bn)
        {
            return benhNhans.FindBy(b => b.email.CompareTo(bn.email.ToLower()) == 0);

        }
    }
}
