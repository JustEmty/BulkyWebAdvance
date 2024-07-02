using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		public ICategoryRepository Category { get; private set; }
		public IProductRepository Product { get; private set; }

		private ApplicationDbContext applicationDbContext;

		public UnitOfWork(ApplicationDbContext applicationDbContext) 
		{ 
			this.applicationDbContext = applicationDbContext;
			Category = new CategoryRepository(applicationDbContext);
			Product = new ProductRepository(applicationDbContext);
		}

		public void Save()
		{
			applicationDbContext.SaveChanges();
		}
	}
}
