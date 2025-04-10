using MVCtemplate.DataAccess.Data;
using MVCTemplate.DataAccess.Repository.IRepository;

namespace MVCTemplate.DataAccess.Repository 
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IProductRepository Product { get; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Product = new ProductRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}


