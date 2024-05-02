using ClosedXML.Excel;
using CustomizedDataTableAspNetCore.Contracts;
using CustomizedDataTableAspNetCore.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Font = iTextSharp.text.Font;
namespace CustomizedDataTableAspNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment environment;
        public HomeController(ILogger<HomeController> logger, IUserService userService, IWebHostEnvironment environment)
        {
            _logger = logger;
            _userService = userService;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            ViewBag.Roles = new string[] { "Admin", "Writer", "Subscriber", "Accountant", "Visitor" };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> List(UsersViewModel model)
        {
            try
            {
                int RowsPerPage = 10;
                model.RowsPerPage = RowsPerPage;

                var response = await _userService.GetUsersList(model);

                ViewBag.TotalCount = response.TotalCount;
                ViewBag.TotalPages = (int)Math.Ceiling((double)response.TotalCount / RowsPerPage);
                ViewBag.CurrentPage = model.PageNo;
                ViewBag.StartFrom = model.StartFrom;
                ViewBag.EndTo = ((model.PageNo - 1) * model.RowsPerPage) + model.RowsPerPage;

                ViewBag.OrderBy = model.OrderBy;
                ViewBag.Direction = model.Direction;
                return PartialView(response);

            }
            catch (Exception ex)
            {
                //LOG
            }

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(UsersViewModel model)
        {
            try
            {
                model.RowsPerPage = 10;
                model.ExportAll = 1;
                var response = await _userService.GetUsersList(model);

                using (var workBook = new XLWorkbook())
                {
                    var workSheet = workBook.Worksheets.Add("users");
                    int currentRow = 1;
                    int index = 1;
                    workSheet.Cell(currentRow, 1).Value = "#";
                    workSheet.Cell(currentRow, 2).Value = "Name";
                    workSheet.Cell(currentRow, 3).Value = "Email";
                    workSheet.Cell(currentRow, 4).Value = "Phone";
                    workSheet.Cell(currentRow, 5).Value = "Address";
                    workSheet.Cell(currentRow, 6).Value = "Role";

                    foreach (var user in response._Items)
                    {
                        currentRow++;
                        workSheet.Cell(currentRow, 1).Value = index++;
                        workSheet.Cell(currentRow, 2).Value = user.FirstName + " " + user.LastName;
                        workSheet.Cell(currentRow, 3).Value = user.Email;
                        workSheet.Cell(currentRow, 4).Value = user.Phone;
                        workSheet.Cell(currentRow, 5).Value = user.Address;
                        workSheet.Cell(currentRow, 6).Value = user.Role;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workBook.SaveAs(stream);
                        var content = stream.ToArray();
                        var date = DateTime.Now.ToString();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            date + "_users.xlsx");
                    }
                }

            }
            catch (Exception ex)
            {
                //LOG
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> ExportToPdf(UsersViewModel usersView)
        {
            try
            {
                usersView.RowsPerPage = 10;
                usersView.ExportAll = 1;

                var response = await _userService.GetUsersList(usersView);

                BaseFont baseFont = BaseFont.CreateFont(Path.Combine(environment.WebRootPath, "assets\\fonts\\arial.ttf"),
                    BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                var font = new Font(baseFont, 8);

                DataTable dt = new DataTable();

                dt.Columns.Add("#");
                dt.Columns.Add("Name");
                dt.Columns.Add("Email");
                dt.Columns.Add("Phone");
                dt.Columns.Add("Address");
                dt.Columns.Add("Role");
                int index = 1;
                foreach (var user in response._Items)
                {
                    var dtRow = dt.NewRow();

                    dtRow[0] = index++;
                    dtRow[1] = user.FirstName + " " + user.LastName;
                    dtRow[2] = user.Email;
                    dtRow[3] = user.Phone;
                    dtRow[4] = user.Address;
                    dtRow[5] = user.Role;

                    dt.Rows.Add(dtRow); 
                }

                Document doc = new Document();
                MemoryStream stream = new MemoryStream();

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, stream);

                doc.Open();

                PdfPTable pdfPTable = new PdfPTable(dt.Columns.Count);

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(dt.Columns[i].ColumnName, font));

                    cell.BackgroundColor = new BaseColor(240, 240, 240);
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.FixedHeight = 25f;
                    cell.PaddingTop = 5f;
                    cell.NoWrap = false;
                    cell.Rotation = 0;

                    pdfPTable.AddCell(cell);
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(dt.Rows[i][j].ToString(), font));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.FixedHeight = 25f;
                        cell.PaddingTop = 5f;
                        cell.NoWrap = false;
                        cell.Rotation = 0;

                        pdfPTable.AddCell(cell);
                    }
                }

                doc.Add(pdfPTable);
                doc.Close();
                stream.Close();

                var date = DateTime.Now.ToString();
                return File(stream.ToArray(), "application/pdf", date + "_users.pdf");
            }
            catch (Exception ex)
            {

                //LOG
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
