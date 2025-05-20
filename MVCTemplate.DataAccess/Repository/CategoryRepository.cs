using MVCtemplate.DataAccess.Data;
using MVCTemplate.DataAccess.Repository.IRepository;
using MVCTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MVCTemplate.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Add(Category entity)
        {
            throw new NotImplementedException();
        }

        public Category? CheckIfUnique(string name)
        {
            return _db.Categorys.FirstOrDefault(i => i.NameCategory == name);
        }

        public Category? ContinueIfNoChangeOnUpdate(string name, int countryId)
        {
            return _db.Categorys.FirstOrDefault(i => i.NameCategory == name && i.IdCategory != countryId);
        }

        public void Delete(Category entity)
        {
            throw new NotImplementedException();
        }

        public Category Get(Expression<Func<Category, bool>> filter, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Category> entity)
        {
            throw new NotImplementedException();
        }

        public void Update(Category category)
        {
            _db.Categorys.Update(category);
        }

        public IEnumerable<Category> GetAll(string? includeProperties)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Product> entity)
        {
            throw new NotImplementedException();
        }
    }
}
