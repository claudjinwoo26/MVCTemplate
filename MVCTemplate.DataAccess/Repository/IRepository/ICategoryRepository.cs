
using MVCTemplate.Models;
using System.Linq.Expressions;

namespace MVCTemplate.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category category);

        void Remove(Category category);

        Category? CheckIfUnique(string name);

        Category? ContinueIfNoChangeOnUpdate(string name, int countryId);

        Category GetFirstOrDefault(Expression<Func<Category, bool>> predicate);
    }
}
