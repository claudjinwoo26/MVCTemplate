using MVCtemplate.DataAccess.Data;
using MVCTemplate.DataAccess.Repository.IRepository;

namespace MVCTemplate.DataAccess.Repository 
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IProductRepository Product { get; private set; }

        public ICategoryRepository Category { get; private set; }

        ICategoryRepository IUnitOfWork.Category => throw new NotImplementedException();

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Product = new ProductRepository(_db);
        }

        public UnitOfWork(ApplicationDbContext db, bool initializeCategory)
        {
            _db = db;
            Category = new CategoryRepository(_db);

            if (initializeCategory)
            {
                Category = new CategoryRepository(_db);
            }

        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}


