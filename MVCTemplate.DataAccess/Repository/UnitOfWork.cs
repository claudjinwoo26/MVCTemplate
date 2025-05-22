using MVCtemplate.DataAccess.Data;
using MVCTemplate.DataAccess.Repository.IRepository;

namespace MVCTemplate.DataAccess.Repository 
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public IProductRepository Product { get; private set; }

        public ICategoryRepository Category { get; private set; }
        public IPersonRepository Person { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Product = new ProductRepository(_db);
            Category = new CategoryRepository(_db);
            Person = new PersonRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}


