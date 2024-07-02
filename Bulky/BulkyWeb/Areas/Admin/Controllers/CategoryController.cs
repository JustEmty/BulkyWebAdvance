using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.ROLE_ADMIN)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categoryList = unitOfWork.Category.GetAll().ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Category Name can not exactly match the Display Order");
            }

            if (ModelState.IsValid)
            {
                unitOfWork.Category.Add(category);
                unitOfWork.Save();
                TempData["success"] = "Create category successful";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // 3 ways to fetch data
            // 1 only use with id
            Category? category = unitOfWork.Category.Get(categoryId => categoryId.Id == id);

            // 2 can use id and other properties such as name, display order, ...
            //Category? category1 = applicationDbContext.Categories.FirstOrDefault(u => u.Id == id);

            // 3 can use id and other properties such as name, display order, ... suitable when calculation or filtering data then get it
            //Category? category2 = applicationDbContext.Categories.Where(u => u.Id == id).FirstOrDefault();

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(category);
                unitOfWork.Save();
                TempData["success"] = "Update category successful";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // 3 ways to fetch data
            // 1 only use with id
            Category? category = unitOfWork.Category.Get(categoryId => categoryId.Id == id);

            // 2 can use id and other properties such as name, display order, ...
            //Category? category1 = applicationDbContext.Categories.FirstOrDefault(u => u.Id == id);

            // 3 can use id and other properties such as name, display order, ... suitable when calculation or filtering data then get it
            //Category? category2 = applicationDbContext.Categories.Where(u => u.Id == id).FirstOrDefault();

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? category = unitOfWork.Category.Get(categoryId => categoryId.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            unitOfWork.Category.Remove(category);
            unitOfWork.Save();
            TempData["success"] = "Delete category successful";
            return RedirectToAction("Index");
        }
    }
}
