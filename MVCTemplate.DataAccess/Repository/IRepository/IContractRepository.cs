
using Microsoft.EntityFrameworkCore;
using MVCtemplate.DataAccess.Data;
using MVCTemplate.Models;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using Contract = MVCTemplate.Models.Contract;

namespace MVCTemplate.DataAccess.Repository.IRepository
{
    public interface IContractRepository : IRepository<Contract>
    {

        void Update(Contract product);

        void Remove(Contract product);

        Contract? CheckIfUnique(string name);

        Contract? ContinueIfNoChangeOnUpdate(string name, int countryId);

        Contract GetFirstOrDefault(Expression<Func<Contract, bool>> predicate); // rest of getfirstordefault is in productrepository

        List<Contract> ToList();

    }
}
