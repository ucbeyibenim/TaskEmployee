using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TaskEmployee.DataAcess.Models;
using TaskEmployee.Models;
using TaskEmployee.ViewModels;
using PagedList;
using System.Linq;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;

namespace TaskEmployee.Controllers
{
    public class HomeController : Controller
    {
        private const int PAGE_SIZE = 5;

        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }





        public async Task<IActionResult> Index(UploadedDataInfo uploading, string sortOrder, Employee searchString, int? page)
        {

            ViewData["PayrollNumberSortParm"] = sortOrder == "payroll_number" ? "payroll_number_desc" : "payroll_number";
            ViewData["ForenamesSortParm"] = sortOrder == "forenames" ? "forenames_desc" : "forenames";
            ViewData["SurnameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "surname_desc" : "";
            ViewData["BirthDateSortParm"] = sortOrder == "birth_date" ? "birth_date_desc" : "birth_date";
            ViewData["TelephoneSortParm"] = sortOrder == "telephone" ? "telephone_desc" : "telephone";
            ViewData["MobileSortParm"] = sortOrder == "mobile" ? "mobile_desc" : "mobile";
            ViewData["AddressSortParm"] = sortOrder == "address" ? "address_desc" : "address";
            ViewData["Address2SortParm"] = sortOrder == "address2" ? "address2_desc" : "address2";
            ViewData["PostcodeSortParm"] = sortOrder == "postcode" ? "postcode_desc" : "postcode";
            ViewData["EMailHomeSortParm"] = sortOrder == "email_home" ? "email_home_desc" : "email_home";
            ViewData["StartDateSortParm"] = sortOrder == "start_date" ? "start_date_desc" : "start_date";


            IEnumerable<Employee> getAllEmployees = _employeeRepository.GetAllEmployees();
            
            if (string.IsNullOrEmpty(page.ToString()))
            {
                page = 1;
            }

            ViewBag.Payroll_Number = searchString.Payroll_Number;
            ViewBag.Forenames =searchString.Forenames;
            ViewBag.Surname = searchString.Surname;
            ViewBag.Date_of_Birth = searchString.Date_of_Birth;
            ViewBag.Telephone = searchString.Telephone;
            ViewBag.Mobile = searchString.Mobile;
            ViewBag.Address = searchString.Address;
            ViewBag.Address_2 = searchString.Address_2;
            ViewBag.Postcode = searchString.Postcode;
            ViewBag.EMail_Home = searchString.EMail_Home;
            ViewBag.Start_Date = searchString.Start_Date;

            getAllEmployees = _employeeRepository.Serching(getAllEmployees, searchString);
          
            getAllEmployees = _employeeRepository.Sorting(getAllEmployees, sortOrder);

            
            int pageNumber = (page ?? 1);

            HomeIndexViewModel viewModel = new HomeIndexViewModel()
            {
                GetAllEmployees = getAllEmployees.ToPagedList(pageNumber, PAGE_SIZE),
                UploadedDataInfo = uploading,
                PageInfo = new PageInfo()
                {
                    CurrentPage = pageNumber,
                    ItemsPerPage= PAGE_SIZE,
                    TotalItems = getAllEmployees.Count()
                }

            };


            return View(viewModel);

        }






        public ViewResult Details(int id)
        {

            Employee employee = _employeeRepository.GetEmployee(id);

            if (employee != null)
            {
                HomeDetailsViewModel viewModel = new HomeDetailsViewModel()
                {
                    GetEmployee = employee,
                    Title = "Employee Details"
                };
                return View(viewModel);

            }
            else
            {
                return NotFoundView(id);
            }

        }







        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }







        [HttpPost]
        public IActionResult Create(HomeCreateViewModel employee)
        {

            if (ModelState.IsValid)
            {
                Employee newEmployee = new Employee()
                {
                    Payroll_Number = employee.Payroll_Number,
                    Forenames = employee.Forenames,
                    Surname = employee.Surname,
                    Date_of_Birth = employee.Date_of_Birth,
                    Telephone = employee.Telephone,
                    Mobile = employee.Mobile,
                    Address = employee.Address,
                    Address_2 = employee.Address_2,
                    Postcode = employee.Postcode,
                    EMail_Home = employee.EMail_Home,
                    Start_Date = employee.Start_Date
                };
                var savedEmployee = _employeeRepository.Create(newEmployee);
                return RedirectToAction("details", new { id = savedEmployee.Id });
            }

            return View();

        }








        [HttpGet]
        public ViewResult Edit(int id)
        {

            Employee employee = _employeeRepository.GetEmployee(id);

            if (employee != null)
            {

                HomeEditViewModel viewModel = new HomeEditViewModel()
                {
                    Id = id,
                    Payroll_Number = employee.Payroll_Number,
                    Forenames = employee.Forenames,
                    Surname = employee.Surname,
                    Date_of_Birth = employee.Date_of_Birth,
                    Telephone = employee.Telephone,
                    Mobile = employee.Mobile,
                    Address = employee.Address,
                    Address_2 = employee.Address_2,
                    Postcode = employee.Postcode,
                    EMail_Home = employee.EMail_Home,
                    Start_Date = employee.Start_Date

                };
                return View(viewModel);

            }
            else
            {
                return NotFoundView(id);
            }

        }







        [HttpPost]
        public IActionResult Edit(HomeEditViewModel employee)
        {

            if (ModelState.IsValid)
            {

                Employee newEmployee = new Employee()
                {
                    Id = employee.Id,
                    Payroll_Number = employee.Payroll_Number,
                    Forenames = employee.Forenames,
                    Surname = employee.Surname,
                    Date_of_Birth = employee.Date_of_Birth,
                    Telephone = employee.Telephone,
                    Mobile = employee.Mobile,
                    Address = employee.Address,
                    Address_2 = employee.Address_2,
                    Postcode = employee.Postcode,
                    EMail_Home = employee.EMail_Home,
                    Start_Date = employee.Start_Date
                };

                var savedEmployee = _employeeRepository.Update(newEmployee);

                return RedirectToAction("details", new { id = savedEmployee.Id });

            }

            return View();

        }







        [HttpPost]
        public IActionResult Delete(int id)
        {

            Employee employee = _employeeRepository.GetEmployee(id);

            if (employee != null)
            {
                _employeeRepository.Delete(id);
                return RedirectToAction("Index");
            }
            else
            {
                return NotFoundView(id);
            }

        }







        public IActionResult Import(IFormFile file)
        {

            var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
            UploadedDataInfo uploadedDataInfo = new UploadedDataInfo();

            switch (fileExt)
            {
                case "xlsx":
                    uploadedDataInfo = _employeeRepository.ImportFromExcel(file).Result;
                    break;
                case "csv":
                    uploadedDataInfo = _employeeRepository.ImportFromCsv(file).Result;
                    break;
                default:
                    uploadedDataInfo.IsUploaded = false;
                    uploadedDataInfo.Message = "Supported file formats: csv, xlsx";
                    uploadedDataInfo.Count = 0;
                    break;
            }

            return RedirectToAction("Index", new { IsUploaded = uploadedDataInfo.IsUploaded, Message = uploadedDataInfo.Message, Count = uploadedDataInfo.Count });

        }


        private ViewResult NotFoundView(int id)
        {
            Response.StatusCode = 404;
            return View("EmployeeNotFound", id);
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}