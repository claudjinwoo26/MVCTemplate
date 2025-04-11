
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCTemplate.Util;
using MVCTemplate.DataAccess.Repository.IRepository;
using MVCTemplate.Models;
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

        public IActionResult Create(Models.Product product) 
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

        #region API Calls
        [HttpGet]

        public IActionResult GetAllProducts()
        {
            List<Product>? productList = _unitOfWork.Product.GetAll().ToList();
            return Json(new { data = productList });
        }
        #endregion
    }
}
