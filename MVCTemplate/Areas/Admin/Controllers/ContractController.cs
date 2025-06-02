using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCTemplate.Util;
using MVCTemplate.DataAccess.Repository.IRepository;
using MVCTemplate.Models;
using MVCtemplate.DataAccess.Data;
using ClosedXML.Excel;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVCTemplate.ViewModels;

namespace MVCTemplate.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{Roles.Admin}, {Roles.User}")]
    [Area("Admin")]
    public class ContractController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public ContractController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public IActionResult Index()
        {
            //var persons = _unitOfWork.Person.GetAll();

            var excludedPersonIds = _unitOfWork.Contract.GetAll() // for the create
            .Where(c => c.Validity > DateTime.Now)
            .Select(c => c.PersonId)
            .Distinct()
            .ToList();

            var persons = _unitOfWork.Person.GetAll()
                .Where(p => !excludedPersonIds.Contains(p.Id))
                .ToList();

            var viewModel = new ContractVM
            {
                Contract = new Contract(),
                PersonList = persons.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ContractVM model)
        {
            // Check if selected Person exists
            if (!_unitOfWork.Person.Exists(model.Contract.PersonId))
            {
                ModelState.AddModelError("Contract.PersonId", "Selected person does not exist.");
            }

            // Optional: Check if contract name is unique
            var existingContract = _unitOfWork.Contract.CheckIfUnique(model.Contract.Name);
            if (existingContract != null)
            {
                ModelState.AddModelError("Contract.Name", "Contract with this name already exists.");
            }

            // ✅ Check if person already has a future-valid contract
            var hasFutureContract = _unitOfWork.Contract.GetFirstOrDefault(c =>
                c.PersonId == model.Contract.PersonId && c.Validity > DateTime.Now);

            if (hasFutureContract != null)
            {
                ModelState.AddModelError("Contract.PersonId", "This person already has a future-valid contract.");
            }

            if (ModelState.IsValid)
            {
                model.Contract.CreatedAt = DateTime.Now;
                _unitOfWork.Contract.Add(model.Contract);
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }

            // If validation fails, repopulate PersonList and return view
            var persons = _unitOfWork.Person.GetAll();
            model.PersonList = persons.Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            });

            return View(model);
        }


        private List<Contract> GetContracts()
        {
            return _unitOfWork.Contract.ToList();
        }

        public async Task<ActionResult> ExportToExcel()
        {
            var contracts = GetContracts();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.AddWorksheet("Sheet 1");

                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Description";

                int row = 2;
                foreach (var item in contracts)
                {
                    worksheet.Cell(row, 1).Value = item.Id;
                    worksheet.Cell(row, 2).Value = item.Name;
                    worksheet.Cell(row, 3).Value = item.Validity;
                    row++;
                }

                using (var memoryStream = new MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    Response.Headers.Add("Content-Disposition", "attachment; filename=ContractsExport.xlsx");
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    await memoryStream.CopyToAsync(Response.Body);
                    return new EmptyResult();
                }
            }
        }

        [HttpPost]
        public IActionResult GetContractsData()
        {
            var names = _context.Contracts.Select(p => p.Name).ToList();
            var quantities = _context.Contracts.Select(p => p.Validity).ToList();

            return Json(new List<object> { names, quantities });
        }

        [HttpPost] // used to be [HttpPut]
        public IActionResult Update(Contract obj)
        {
            try
            {
                obj.GenerateUpdatedAt();
                Contract? contract = _unitOfWork.Contract.ContinueIfNoChangeOnUpdate(obj.Name, obj.Id);

                if (contract != null)
                {
                    ModelState.AddModelError("Name", "Contract Name Already exists");
                }

                if (ModelState.IsValid)
                {
                    _unitOfWork.Contract.Update(obj);
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
                    return BadRequest(new { message = "Contract Id not found" });
                }

                var contract = _unitOfWork.Contract.Get(u => u.Id == id);
                if (contract == null)
                {
                    return BadRequest(new { message = "Contract Id not found" });
                }

                _unitOfWork.Contract.Remove(contract);
                _unitOfWork.Save();
                return Ok(new { message = "Contract deleted successfully" });
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
        public IActionResult GetAllContracts()
        {
            var contractList = _unitOfWork.Contract.GetAll().ToList();
            return Json(new { data = contractList });
        }

        #endregion
    }
}
