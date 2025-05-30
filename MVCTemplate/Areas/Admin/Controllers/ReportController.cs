using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCtemplate.DataAccess.Data;
using MVCTemplate.Models;
using MVCTemplate.Util;
using MVCTemplate.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Drawing;
using DocumentFormat.OpenXml.InkML;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Previewer;

namespace MVCTemplate.Controllers
{
    [Authorize(Roles = $"{Roles.Admin}, {Roles.User}")]
    [Area("Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ReportController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPdf()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var reports = await _context.Reports.ToListAsync();
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/reports");

            byte[] pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);

                    // Add footer with report generation date
                    page.Footer()
                        .AlignRight()
                        .Text($"Report generated on {DateTime.Now:MM-dd-yyyy}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Medium);


                    page.Content().Table(table =>
                    {
                        // Define columns count and relative widths
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);  // Title column
                            columns.RelativeColumn(5);  // Description column
                            columns.RelativeColumn(3);  // Image column
                        });

                        // Header row styling
                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Title").Bold();
                            header.Cell().Element(CellStyle).Text("Description").Bold();
                            header.Cell().Element(CellStyle).Text("Image").Bold();
                        });

                        // Data rows
                        foreach (var report in reports)
                        {
                            table.Cell().Element(CellStyle).Text(report.Title ?? "");
                            table.Cell().Element(CellStyle).Text(report.Description ?? "");

                            if (!string.IsNullOrEmpty(report.ImageName))
                            {
                                var imagePath = Path.Combine(filePath, report.ImageName);
                                if (System.IO.File.Exists(imagePath))
                                {
                                    table.Cell().Element(CellStyle).Image(imagePath, ImageScaling.FitArea);
                                }
                                else
                                {
                                    table.Cell().Element(CellStyle).Text("[Image not found]");
                                }
                            }
                            else
                            {
                                table.Cell().Element(CellStyle).Text("[No image]");
                            }
                        }
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "Reports.pdf");

            // Local function for cell styling
            IContainer CellStyle(IContainer container) =>
                container
                    .Border(1)
                    .BorderColor(Colors.Grey.Medium)
                    .Padding(5)
                    .AlignMiddle()
                    .AlignCenter();
        }




        [HttpGet]
        public async Task<IActionResult> ExportToExcel() // all data are fetched
        {
            ExcelPackage.License.SetNonCommercialPersonal("My Name");

            var reports = await _context.Reports.ToListAsync();
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/reports");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reports");

                // Header row
                worksheet.Cells[1, 1].Value = "Title";
                worksheet.Cells[1, 2].Value = "Description";
                worksheet.Cells[1, 3].Value = "Image";

                int row = 2;
                foreach (var report in reports)
                {
                    worksheet.Cells[row, 1].Value = report.Title;
                    worksheet.Cells[row, 2].Value = report.Description;

                    if (!string.IsNullOrEmpty(report.ImageName))
                    {
                        var imagePath = Path.Combine(filePath, report.ImageName);
                        if (System.IO.File.Exists(imagePath))
                        {
                            using (var imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                var excelImage = worksheet.Drawings.AddPicture($"img_{row}", imageStream);

                                // Anchor to the cell C{row}
                                excelImage.SetPosition(row - 1, 5, 2, 5); // (rowIndex, offsetY, colIndex, offsetX)
                                excelImage.SetSize(80, 80);
                                excelImage.EditAs = eEditAs.OneCell; // Anchor image to cell

                                // Lock image and maintain aspect ratio
                                excelImage.Locked = true;
                                excelImage.LockAspectRatio = true;

                                // Adjust row height and column width to fit image
                                worksheet.Row(row).Height = 60;
                                worksheet.Column(3).Width = (80 - 5) / 7.0; // Approx 10.7 for image to show well
                            }
                        }
                    }

                    row++;
                }

                // Formatting headers and columns
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 50;
                worksheet.Row(1).Style.Font.Bold = true;

                // Add auto filter
                worksheet.Cells["A1:B1"].AutoFilter = true;

                // Lock all cells
                worksheet.Cells.Style.Locked = true;

                // Protect worksheet - disable image editing
                worksheet.Protection.SetPassword("YourPassword"); // Change this
                worksheet.Protection.AllowSelectLockedCells = true;
                worksheet.Protection.AllowSelectUnlockedCells = true;
                worksheet.Protection.AllowAutoFilter = true;
                worksheet.Protection.AllowEditObject = false;
                worksheet.Protection.AllowEditScenarios = false;
                worksheet.Protection.IsProtected = true;

                // Protect workbook structure
                package.Workbook.Protection.SetPassword("YourPassword");
                package.Workbook.Protection.LockStructure = true;
                package.Workbook.Protection.LockWindows = true;

                // Save and return the Excel file
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"Reports.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportFilteredToExcel(string? titleFilter)
        {
            ExcelPackage.License.SetNonCommercialPersonal("My Name");

            var reportsQuery = _context.Reports.AsQueryable();

            if (!string.IsNullOrWhiteSpace(titleFilter))
            {
                reportsQuery = reportsQuery.Where(r => r.Title != null && r.Title.Contains(titleFilter));
            }

            var reports = await reportsQuery.ToListAsync();
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/reports");

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Filtered Reports");

                // Header row
                worksheet.Cells[1, 1].Value = "Title";
                worksheet.Cells[1, 2].Value = "Description";
                worksheet.Cells[1, 3].Value = "Image";

                int row = 2;
                foreach (var report in reports)
                {
                    worksheet.Cells[row, 1].Value = report.Title;
                    worksheet.Cells[row, 2].Value = report.Description;

                    if (!string.IsNullOrEmpty(report.ImageName))
                    {
                        var imagePath = Path.Combine(filePath, report.ImageName);
                        if (System.IO.File.Exists(imagePath))
                        {
                            using (var imageStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                            {
                                var excelImage = worksheet.Drawings.AddPicture($"img_{row}", imageStream);

                                excelImage.SetPosition(row - 1, 5, 2, 5);
                                excelImage.SetSize(80, 80);
                                excelImage.EditAs = eEditAs.OneCell;
                                excelImage.Locked = true;
                                excelImage.LockAspectRatio = true;

                                worksheet.Row(row).Height = 60;
                                worksheet.Column(3).Width = (80 - 5) / 7.0;
                            }
                        }
                    }

                    row++;
                }

                // Format headers
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 50;
                worksheet.Row(1).Style.Font.Bold = true;

                worksheet.Cells["A1:B1"].AutoFilter = true;
                worksheet.Cells.Style.Locked = true;

                worksheet.Protection.SetPassword("YourPassword");
                worksheet.Protection.AllowSelectLockedCells = true;
                worksheet.Protection.AllowSelectUnlockedCells = true;
                worksheet.Protection.AllowAutoFilter = true;
                worksheet.Protection.AllowEditObject = false;
                worksheet.Protection.AllowEditScenarios = false;
                worksheet.Protection.IsProtected = true;

                package.Workbook.Protection.SetPassword("YourPassword");
                package.Workbook.Protection.LockStructure = true;
                package.Workbook.Protection.LockWindows = true;

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string fileName = $"FilteredReports.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ExportFilteredToPdf(string? titleFilter) // not all
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var reportsQuery = _context.Reports.AsQueryable();

            if (!string.IsNullOrWhiteSpace(titleFilter))
            {
                reportsQuery = reportsQuery.Where(r => r.Title != null && r.Title.Contains(titleFilter));
            }

            var reports = await reportsQuery.ToListAsync();

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/reports");

            byte[] pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);

                    // Footer with generation date
                    page.Footer()
                        .AlignRight()
                        .Text($"Report generated on {DateTime.Now:MM-dd-yyyy}")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Medium);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(5);
                            columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Title").Bold();
                            header.Cell().Element(CellStyle).Text("Description").Bold();
                            header.Cell().Element(CellStyle).Text("Image").Bold();
                        });

                        foreach (var report in reports)
                        {
                            table.Cell().Element(CellStyle).Text(report.Title ?? "");
                            table.Cell().Element(CellStyle).Text(report.Description ?? "");

                            if (!string.IsNullOrEmpty(report.ImageName))
                            {
                                var imagePath = Path.Combine(filePath, report.ImageName);
                                if (System.IO.File.Exists(imagePath))
                                {
                                    table.Cell().Element(CellStyle).Image(imagePath, ImageScaling.FitArea);
                                }
                                else
                                {
                                    table.Cell().Element(CellStyle).Text("[Image not found]");
                                }
                            }
                            else
                            {
                                table.Cell().Element(CellStyle).Text("[No image]");
                            }
                        }
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "FilteredReports.pdf");

            IContainer CellStyle(IContainer container) =>
                container
                    .Border(1)
                    .BorderColor(Colors.Grey.Medium)
                    .Padding(5)
                    .AlignMiddle()
                    .AlignCenter();
        }



        // GET: /Report/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Report/GetAllReports (for DataTable ajax)
        [HttpGet]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _context.Reports
                .Select(r => new {
                    r.Id,
                    r.Title,
                    r.ImageName,
                    r.Description
                }).ToListAsync();

            return Json(new { data = reports });
        }

        // POST: /Report/Create
        [HttpPost]
        public async Task<IActionResult> Create(ReportVM model)
        {
            if (ModelState.IsValid)
            {
                string fileName = null;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/reports");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var report = new Report
                {
                    Title = model.Title,
                    ImageName = fileName,
                    Description = model.Description,
                    CreatedAt = DateTime.Now
                };

                _context.Reports.Add(report);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Report created successfully." });
            }

            // Collect errors to send back
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { success = false, message = "Validation failed", errors });
        }

        // POST: /Report/Update             // AGGRESSIVE WAY TO ALLOW imageFile to be empty
        [HttpPost]
        public async Task<IActionResult> Update(ReportVM model)
        {
            // Remove all validation errors related to ImageFile so image is optional during update
            if (ModelState.ContainsKey(nameof(model.ImageFile)))
            {
                ModelState[nameof(model.ImageFile)].Errors.Clear();

                // Also mark the field as valid in ModelState dictionary
                ModelState[nameof(model.ImageFile)].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            }

            if (ModelState.IsValid)
            {
                var report = await _context.Reports.FindAsync(model.Id);
                if (report == null)
                    return NotFound(new { success = false, message = "Report not found." });

                report.Title = model.Title;
                report.Description = model.Description;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(report.ImageName))
                    {
                        var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/reports", report.ImageName);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/reports");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    var newFilePath = Path.Combine(uploadsFolder, newFileName);

                    using (var fileStream = new FileStream(newFilePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    report.ImageName = newFileName;
                }

                report.GenerateUpdatedAt();

                _context.Reports.Update(report);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Report updated successfully." });
            }

            // If invalid, return all validation errors for debugging
            var errors = ModelState
                .Where(kvp => kvp.Value.Errors.Count > 0)
                .Select(kvp => new { Field = kvp.Key, Errors = kvp.Value.Errors.Select(e => e.ErrorMessage).ToList() })
                .ToList();

            return BadRequest(new { success = false, message = "Validation failed", errors });
        }



        // DELETE: /Report/Delete/{id}
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
                return NotFound(new { success = false, message = "Report not found." });

            // Delete image file if exists
            if (!string.IsNullOrEmpty(report.ImageName))
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/reports", report.ImageName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Report deleted successfully." });
        }
    }
}
