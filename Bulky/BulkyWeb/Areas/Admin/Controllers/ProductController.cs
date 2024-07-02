using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
    public class ProductController : Controller
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly IWebHostEnvironment webHostEnvironment;

		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			this.unitOfWork = unitOfWork;
			this.webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			List<Product> productList = unitOfWork.Product.GetAll("Category").ToList();
			return View(productList);
		}

		public IActionResult Upsert(int? id)
		{
			//IEnumerable<SelectListItem> categoryList = unitOfWork.Category.GetAll().Select(category => new SelectListItem
			//{
			//	Text = category.Name,
			//	Value = category.Id.ToString()
			//});

			// 3 way to pass list object in temp data
			// 1
			//ViewBag.CategoryList = categoryList;

			// 2
			//ViewData["CategoryList"] = categoryList;

			ProductVM productVM = new()
			{
				Product = new Product(),
				CategoryList = unitOfWork.Category.GetAll().Select(category => new SelectListItem
				{
					Text = category.Name,
					Value = category.Id.ToString()
				})
			};

			if(id == null || id == 0)
			{
				// create				
				return View(productVM);
			}
			else
			{
				productVM.Product = unitOfWork.Product.Get(product => product.Id == id);	
				return View(productVM);
			}

		}

		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? imageFile)
		{
			// To solve null problem things we can do is using [ValidateNever] on field we do not want validate. Another ways below here
			if (ModelState.IsValid)
			{
				if(imageFile != null)
				{
					string wwwRootPath = webHostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");

					if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
					{
						var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

						if(System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using(var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						imageFile.CopyTo(fileStream);
					}

					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}

				if(productVM.Product.Id == 0)
				{
					unitOfWork.Product.Add(productVM.Product);
				}
				else
				{
					unitOfWork.Product.Update(productVM.Product);
				}
				unitOfWork.Save();
				TempData["success"] = "Create category successful";
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = unitOfWork.Category.GetAll().Select(category => new SelectListItem
				{
					Text = category.Name,
					Value = category.Id.ToString()
				});
				return View(productVM);
			}
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}

			Product? product = unitOfWork.Product.Get(product => product.Id == id);

			if (product == null)
			{
				return NotFound();
			}
			return View(product);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Product? product = unitOfWork.Product.Get(product => product.Id == id);
			if (product == null)
			{
				return NotFound();
			}
			unitOfWork.Product.Remove(product);
			unitOfWork.Save();
			TempData["success"] = "Delete category successful";
			return RedirectToAction("Index");
		}

		#region API CALLS

		//[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> productList = unitOfWork.Product.GetAll("Category").ToList();
			return Json(new { data = productList });
		}

		//[HttpDelete] 
		public IActionResult DeleteWithAPI(int? id)
		{
			var productToBeDeleted = unitOfWork.Product.Get(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

			unitOfWork.Product.Remove(productToBeDeleted);
			unitOfWork.Save();

			return Json(new { success = false, message = "Delete Successful" });
		}

		#endregion
	}
}
