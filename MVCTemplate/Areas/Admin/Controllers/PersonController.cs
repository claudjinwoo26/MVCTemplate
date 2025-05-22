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
    public class PersonController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private IUnitOfWork _unitOfWork;

        public PersonController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Create(Person person)
        {
            try
            {
                Models.Person? personCheck = _unitOfWork.Person.CheckIfUnique(person.Name);
                if (personCheck != null)
                {
                    ModelState.AddModelError("Name", "Category already exists");
                }

                if (ModelState.IsValid)
                {
                    _unitOfWork.Person.Add(person);
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
            catch (Exception) //exception to personrepository

            {
                return BadRequest(new { message = "An unexpected error occurred" });
            }
        }
        [HttpPut]
        public IActionResult Update(Person obj)
        {
            try
            {
                obj.GenerateUpdatedAt();
                Person? person = _unitOfWork.Person.ContinueIfNoChangeOnUpdate(obj.Name, obj.Id);

                if (person != null)
                {
                    ModelState.AddModelError("Name", "Person Name Already exists");
                }
                if (ModelState.IsValid)
                {
                    _unitOfWork.Person.Update(obj);
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
                    return BadRequest(new { message = "Person Id not found" });
                }

                Person person = _unitOfWork.Person.Get(u => u.Id == id);
                if (person == null)
                {
                    return BadRequest(new { message = "Person Id not found" });
                }

                _unitOfWork.Person.Remove(person);
                _unitOfWork.Save();
                return Ok(new { message = "Person deleted successfully" });
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

        public IActionResult GetAllPersons()
        {
            List<Person>? personList = _unitOfWork.Person.GetAll().ToList(); //not implemented
            return Json(new { data = personList });
        }
       
        #endregion

    }
}
