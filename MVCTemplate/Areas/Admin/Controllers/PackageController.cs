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
    public class PackageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private IUnitOfWork _unitOfWork;

        public PackageController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Create(Package package)
        {
            try
            {
                Models.Package? packageCheck = _unitOfWork.Package.CheckIfUnique(package.Name);
                if (packageCheck != null)
                {
                    ModelState.AddModelError("Name", "Package already exists");
                }

                if (ModelState.IsValid)
                {
                    _unitOfWork.Package.Add(package);
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
        public IActionResult Update(Package obj)
        {
            try
            {
                obj.GenerateUpdatedAt();
                Package? package = _unitOfWork.Package.ContinueIfNoChangeOnUpdate(obj.Name, obj.Id);

                if (package != null)
                {
                    ModelState.AddModelError("Name", "Package Name Already exists");
                }
                if (ModelState.IsValid)
                {
                    _unitOfWork.Package.Update(obj);
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
                    return BadRequest(new { message = "Package Id not found" });
                }

                Package package = _unitOfWork.Package.Get(u => u.Id == id);
                if (package == null)
                {
                    return BadRequest(new { message = "Package Id not found" });
                }

                _unitOfWork.Package.Remove(package);
                _unitOfWork.Save();
                return Ok(new { message = "Package deleted successfully" });
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

        public IActionResult GetAllPackages()
        {
            List<Package>? packageList = _unitOfWork.Package.GetAll().ToList();
            return Json(new { data = packageList });
        }
       
        #endregion

    }
}
