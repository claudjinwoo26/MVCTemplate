namespace MVCTemplate.DataAccess.Repository.IRepository;

public interface IUnitOfWork
{
    IProductRepository Product { get; }

    void Save();
}
