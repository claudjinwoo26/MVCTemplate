
using MVCTemplate.Models;

namespace MVCTemplate.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);

        void Remove(Product product);

        Product? CheckIfUnique(string name);

        Product? ContinueIfNoChangeOnUpdate(string name, int countryId);
    }
}
