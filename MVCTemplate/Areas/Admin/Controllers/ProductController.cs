using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCTemplate.Util;
using MVCTemplate.DataAccess.Repository.IRepository;
using MVCTemplate.Models;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace MVCTemplate.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{Roles.Admin}, {Roles.User}")]
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            try
            {
                Models.Product? productCheck = _unitOfWork.Product.CheckIfUnique(product.Name);
                if (productCheck != null)
                {
                    ModelState.AddModelError("Name", "Product already exists");
                }

                if (ModelState.IsValid)
                {
                    _unitOfWork.Product.Add(product);
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
        public IActionResult Update(Product obj)
        {
            try
            {
                obj.GenerateUpdatedAt();
                Product? product = _unitOfWork.Product.ContinueIfNoChangeOnUpdate(obj.Name, obj.Id);

                if (product != null)
                {
                    ModelState.AddModelError("Name", "Product Name Already exists");
                }
                if (ModelState.IsValid)
                {
                    _unitOfWork.Product.Update(obj);
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
                    return BadRequest(new { message = "Product Id not found" });
                }

                Product product = _unitOfWork.Product.Get(u => u.Id == id);
                if (product == null)
                {
                    return BadRequest(new { message = "Product Id not found" });
                }

                _unitOfWork.Product.Remove(product);
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
        public IActionResult GetAllProducts()
        {
            List<Product>? productList = _unitOfWork.Product.GetAll().ToList();
            return Json(new { data = productList });
        }

        /*
        // Step 1: Add DELETE API Method for Product Deletion
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                // Step 2: Retrieve the product to be deleted
                var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id); // product and iproduct repositories had to be updated

                // Step 3: Check if the product exists
                if (product == null)
                {
                    return Json(new { success = false, message = "Product not found" });
                }

                // Step 4: Delete the product
                _unitOfWork.Product.Remove(product);
                _unitOfWork.Save();

                // Step 5: Return success response
                return Json(new { success = true, message = "Product deleted successfully" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An unexpected error occurred while deleting the product" });
            }
        } OLD DELETE
        */

        #endregion
    }
}
