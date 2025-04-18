﻿using MVCtemplate.DataAccess.Data;
using MVCTemplate.DataAccess.Repository.IRepository;
using MVCTemplate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCTemplate.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public Product? CheckIfUnique(string name)
        {
            return _db.Products.FirstOrDefault(i => i.Name == name);
        }

        public Product? ContinueIfNoChangeOnUpdate(string name, int countryId)
        {
            return _db.Products.FirstOrDefault(i => i.Name == name && i.Id != countryId);
        }

        public void Update(Product product)
        {
            _db.Products.Update(product);
        }
    }
}
