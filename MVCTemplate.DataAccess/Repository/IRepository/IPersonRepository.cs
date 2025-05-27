
using MVCTemplate.Models;
using System.Linq.Expressions;

namespace MVCTemplate.DataAccess.Repository.IRepository
{
    public interface IPersonRepository : IRepository<Person>
    {
        void Update(Person category);

        void Remove(Person category);

        Person? CheckIfUnique(string name);

        Person? ContinueIfNoChangeOnUpdate(string name, int Id);

        Person GetFirstOrDefault(Expression<Func<Category, bool>> predicate);
    }
}
