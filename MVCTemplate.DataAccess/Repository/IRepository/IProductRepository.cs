
using MVCTemplate.Models;

namespace MVCTemplate.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<ProductModel>
    {
        void Update(ProductModel product);

        void Remove(ProductModel product);

        ProductModel? CheckIfUnique(string name);

        ProductModel? ContinueIfNoChangeOnUpdate(string name, int countryId);
    }
}
