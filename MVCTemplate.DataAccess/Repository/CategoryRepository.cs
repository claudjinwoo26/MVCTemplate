using Microsoft.EntityFrameworkCore;
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

        public Category? CheckIfUnique(string name) 
        {
            return _db.Categorys.FirstOrDefault(i => i.NameCategory == name);
        }

        public Category? ContinueIfNoChangeOnUpdate(string name, int categoryId)
        {
            return _db.Categorys.FirstOrDefault(i => i.NameCategory == name && i.IdCategory != categoryId);
        }

        public Category GetFirstOrDefault(Expression<Func<Category, bool>> predicate)
        {
            return _db.Categorys.FirstOrDefault(predicate);
        }

        public void Update(Category category)
        {
            _db.Categorys.Update(category);
        }
    }
}
