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
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		private readonly ApplicationDbContext applicationDbContext;

		public CategoryRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
		{
			this.applicationDbContext = applicationDbContext;
		}

		public void Update(Category category)
		{
			applicationDbContext.Categories.Update(category);
		}
	}
}
