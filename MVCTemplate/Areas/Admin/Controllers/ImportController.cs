using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCtemplate.DataAccess.Data;
using MVCTemplate.DataAccess.Repository.IRepository;
using MVCTemplate.Models;
using MVCTemplate.Util;
using System.Diagnostics;
using System.Text;

namespace MVCTemplate.Areas.Admin.Controllers
{
    [Authorize(Roles = $"{Roles.Admin}")]
    [Area("Admin")]

    public class ImportController : Controller
    {


        private readonly ILogger<ImportController> _logger;
        private readonly ApplicationDbContext _context;

        public ImportController(ILogger<ImportController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult UploadExcel()
        {
            return View(); 
        }

        public IActionResult UploadExcelProduct()
        {
            //return View();
            return View("UploadExcel");
        } // for product

        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (file != null && file.Length > 0)
            {
                var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\"; // has to be on the root folder

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Choose one of either 1 or 2:

                        // 1. Use the reader methods
                        do
                        {
                            bool isHeaderSkipped = false;

                            while (reader.Read())
                            {
                                if (!isHeaderSkipped)
                                {
                                    isHeaderSkipped = true;
                                    continue;
                                }
                                Package p = new Package();
                                p.Name = reader.GetValue(1).ToString(); // skips the ID column
                                p.Description = reader.GetValue(2).ToString();
                                p.Priority = Convert.ToInt32(reader.GetValue(3).ToString());

                                _context.Add(p);
                                await _context.SaveChangesAsync();
                                // does not check for unique name
                            }
                        } while (reader.NextResult());

                        ViewBag.Message = "success";
                    }
                }
            }
            else ViewBag.Message = "empty";
            return View();

            // below is for product

        }

        [HttpPost]
        public async Task<IActionResult> UploadExcelProduct(IFormFile file)
        {
           
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (file != null && file.Length > 0)
            {
                var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\"; // has to be on the root folder

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // Choose one of either 1 or 2:

                        // 1. Use the reader methods
                        do
                        {
                            bool isHeaderSkipped = false;

                            while (reader.Read())
                            {
                                if (!isHeaderSkipped)
                                {
                                    isHeaderSkipped = true;
                                    continue;
                                }
                                Product pr = new Product
                                {
                                    Name = reader.GetValue(1).ToString(), // skips the ID column
                                    Description = reader.GetValue(2).ToString(),
                                    Quantity = Convert.ToInt32(reader.GetValue(3).ToString())
                                };

                                _context.Add(pr);
                                await _context.SaveChangesAsync();
                                // does not check for unique name
                            }
                        } while (reader.NextResult());

                        //ViewBag.Message = "success";
                    }
                }
            }
            else return BadRequest("Something went wrong!");

            return View("UploadExcel");
            //for product

        }
    }
}
