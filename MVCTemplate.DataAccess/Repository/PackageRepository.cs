﻿using Microsoft.EntityFrameworkCore;
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
    public class PackageRepository : Repository<Package>, IPackageRepository
    {
        private readonly ApplicationDbContext _db;

        public PackageRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }

        public Package? CheckIfUnique(string name) 
        {
            return _db.Packages.FirstOrDefault(i => i.Name == name);
        }

        public Package? ContinueIfNoChangeOnUpdate(string name, int Id)
        {
            return _db.Packages.FirstOrDefault(i => i.Name == name && i.Id != Id);
        }

        public Package GetFirstOrDefault(Expression<Func<Package, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Update(Package package)
        {
            _db.Packages.Update(package);
        }
    }
}
