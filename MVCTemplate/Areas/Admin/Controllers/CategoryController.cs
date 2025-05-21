using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCTemplate.DataAccess.Repository.IRepository;
using MVCTemplate.Models;
using MVCTemplate.Util;
using System.Diagnostics;

namespace MVCTemplate.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{Roles.Admin}")]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Create(Category category)
        {
            try
            {
                Models.Category? productCheck = _unitOfWork.Category.CheckIfUnique(category.NameCategory);
                if (productCheck != null)
                {
                    ModelState.AddModelError("Name", "Category already exists");
                }

                if (ModelState.IsValid)
                {
                    _unitOfWork.Category.Add(category);
                    _unitOfWork.Save();
                    return Ok(new { message = "Added Successfully" });
                }
                var errors = ModelState.ToDictionary(
                     kvp => kvp.Key,
                     kvp => kvp.Value?.Errors?.Select(e => e.ErrorMessage)?.ToArray() ?? []
                  );
                return BadRequest(new { errors, message = "Something went wrong" });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Error occurred while saving to database" });
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { message = "Invalid Operation" });
            }
            catch (Exception)

            {
                return BadRequest(new { message = "An unexpected error occurred" });
            }
        }
        [HttpPut]
        public IActionResult Update(Category obj)
        {
            try
            {
                obj.GenerateUpdatedAt();
                Category? category = _unitOfWork.Category.ContinueIfNoChangeOnUpdate(obj.NameCategory, obj.IdCategory);

                if (category != null)
                {
                    ModelState.AddModelError("Name", "Category Name Already exists");
                }
                if (ModelState.IsValid)
                {
                    _unitOfWork.Category.Update(obj);
                    _unitOfWork.Save();
                    return Ok(new { message = "Updated Successfully" });
                }
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors?.Select(e => e.ErrorMessage)?.ToArray() ?? []);

                return BadRequest(new { errors, message = "Something went wrong!" });

            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Error occurred while saving to database" });
            }
            catch (InvalidOperationException)
            {
                return BadRequest(new { message = "Invalid operation" });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An unexpected error occurred" });
            }
        }

        [HttpDelete]

        public IActionResult Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest(new { message = "Category Id not found" });
                }

                Category category = _unitOfWork.Category.Get(u => u.IdCategory == id);
                if (category == null)
                {
                    return BadRequest(new { message = "Category Id not found" });
                }

                _unitOfWork.Category.Remove(category);
                _unitOfWork.Save();
                return Ok(new { message = "Category deleted successfully" });
            }
            catch (DbUpdateException)
            {
                return BadRequest(new { message = "Unable to delete data because it is being used in another table" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        #region API Calls
        [HttpGet]

        public IActionResult GetAllCategory()
        {
            List<Category>? categoryList = _unitOfWork.Category.GetAll().ToList();
            return Json(new { data = categoryList });
        }
       
        #endregion

    }
}
