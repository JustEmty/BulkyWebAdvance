using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		private readonly ApplicationDbContext applicationDbContext;

		public ProductRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
		{
			this.applicationDbContext = applicationDbContext;
		}

		public void Update(Product product)
		{
			Product productFromDb = applicationDbContext.Products.FirstOrDefault(p => p.Id == product.Id);
			if(productFromDb != null)
			{
				productFromDb.Title = product.Title;
				productFromDb.ISBN = product.ISBN;
				productFromDb.Price = product.Price;
				productFromDb.Price50 = product.Price50;
				productFromDb.Price100 = product.Price100;
				productFromDb.Description = product.Description;
				productFromDb.CategoryId = product.CategoryId;
				productFromDb.Author = product.Author;
				if(product.ImageUrl != null)
				{
					productFromDb.ImageUrl = product.ImageUrl;
				}
			}
		}
	}
}
