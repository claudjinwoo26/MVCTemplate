using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCTemplate.Util;
using MVCTemplate.DataAccess.Repository.IRepository;
using MVCTemplate.Models;
using Microsoft.AspNetCore.Mvc.Abstractions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;

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

        private List<Product> GetProducts()
        {
            return _unitOfWork.Product.ToList();
            {
        };
        }

        public async Task<ActionResult> ExportToExcel()
        {
            var products = GetProducts(); // NULL SO EXCEL ONLY HAS HEADER

            // Example: List of data you want to export
            var data = new List<Product>
        {
        };

            // Create a new Excel workbook
            using (var workbook = new XLWorkbook())
            {
                // Add a worksheet to the workbook
                var worksheet = workbook.AddWorksheet("Sheet 1");

                // Add headers
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Description";
                worksheet.Cell(1, 4).Value = "Quantity";

                // Add data
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.Id;
                    worksheet.Cell(row, 2).Value = item.Name;
                    worksheet.Cell(row, 3).Value = item.Description;
                    worksheet.Cell(row, 4).Value = item.Quantity;
                    row++;
                }

                // Save the workbook to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    // Set the response headers for file download
                    Response.Headers.Add("Content-Disposition", "attachment; filename=ProductsExport.xlsx");
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    // Write the memory stream content to the response
                    await memoryStream.CopyToAsync(Response.Body);
                    return new EmptyResult(); // End the action
                }
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
                return Ok(new { message = "Product deleted successfully" });
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
            return Json(new {data = productList});

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
