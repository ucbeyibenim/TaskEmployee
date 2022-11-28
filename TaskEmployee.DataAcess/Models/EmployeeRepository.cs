using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskEmployee.Models;
using static System.Formats.Asn1.AsnWriter;

namespace TaskEmployee.DataAcess.Models
{
    public class EmployeeRepository : IEmployeeRepository
    {
        
        private readonly AppDbContext dbContext;

        
        
        public EmployeeRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        
        
        public Employee Create(Employee employee)
        {
            dbContext.Employees.Add(employee);
            dbContext.SaveChanges();
            return employee;
        }

        
        
        public Employee Delete(int id)
        {
            var employee = dbContext.Employees.Find(id);
            if(employee != null)
            {
                dbContext.Employees.Remove(employee);
                dbContext.SaveChanges();
            }
            return employee;
        }

        
        
        public IEnumerable<Employee> GetAllEmployees()
        {
            return dbContext.Employees;
        }

        
        
        public Employee GetEmployee(int id)
        {
            return dbContext.Employees.Find(id);
        }

        
        
        public Employee Update(Employee updatedEmployee)
        {
            var employee = dbContext.Employees.Attach(updatedEmployee);
            employee.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            dbContext.SaveChanges();
            return updatedEmployee;
        }







        public async Task<UploadedDataInfo> ImportFromExcel(IFormFile file)
        {
            UploadedDataInfo done = new UploadedDataInfo() 
            {
                Message = "Uploaded successfuly",
                IsUploaded = true,
                Count = 0
            };

            using var transaction = dbContext.Database.BeginTransaction();
            try
            {

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream).ConfigureAwait(false);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.Commercial;
                        // if you are using epplus for noncommercial purposes, see https://polyformproject.org/licenses/noncommercial/1.0.0/
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                        if (worksheet.Rows.IsNullOrEmpty())
                        {
                            throw new Exception("File is empty");
                        }
                        if (worksheet.Columns.Count() != 11)
                        {
                            throw new Exception("Columns are not filled correctly");

                        }
                        var rowCount = worksheet.Dimension.Rows;
                        var colCount = worksheet.Dimension.Columns;


                        for (int row = 2; row <= rowCount; row++)
                        {

                            var cultureInfo = new CultureInfo("uz-UZ");

                            Employee employee = new Employee
                            {
                                Payroll_Number = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                Forenames = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                Surname = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                Date_of_Birth = DateTime.Parse(worksheet.Cells[row, 4].Value.ToString().Trim(), cultureInfo),
                                Telephone = worksheet.Cells[row, 5].Value.ToString().Trim(),
                                Mobile = worksheet.Cells[row, 6].Value.ToString().Trim(),
                                Address = worksheet.Cells[row, 7].Value.ToString().Trim(),
                                Address_2 = worksheet.Cells[row, 8].Value.ToString().Trim(),
                                Postcode = worksheet.Cells[row, 9].Value.ToString().Trim(),
                                EMail_Home = worksheet.Cells[row, 10].Value.ToString().Trim(),
                                Start_Date = DateTime.Parse(worksheet.Cells[row, 11].Value.ToString().Trim(), cultureInfo),

                            };

                            dbContext.Employees.Add(employee);
                            if (dbContext.SaveChanges() <= 0)
                            {
                                throw new Exception("Could not saved");
                            }
                            done.Count = done.Count + 1;


                        }

                    }

                }
            }
            catch (Exception e)
            {
                done.Message = e.Message.ToString();
                done.IsUploaded = false;
                done.Count = 0;
                transaction.Rollback();
            }

            if (done.IsUploaded)
            {
                transaction.Commit();
            }

            return done;
        }







        public async Task<UploadedDataInfo> ImportFromCsv(IFormFile file)
        {
            UploadedDataInfo done = new UploadedDataInfo()
            {
                Message = "Uploaded successfuly",
                IsUploaded = true,
                Count = 0
            };

            using var transaction = dbContext.Database.BeginTransaction();
            try
            {


                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    bool header = true;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line.IsNullOrEmpty())
                        {
                            throw new Exception("Field is empty");
                        }
                        if (header)
                        {
                            header = false;
                            continue;
                        }
                        var values = line.Split(';');
                        var cultureInfo = new CultureInfo("uz-UZ");

                        Employee employee = new Employee
                        {
                            Payroll_Number = values[0],
                            Forenames = values[1],
                            Surname = values[2],
                            Date_of_Birth = DateTime.Parse(values[3], cultureInfo),
                            Telephone = values[4],
                            Mobile = values[5],
                            Address = values[6],
                            Address_2 = values[7],
                            Postcode = values[8],
                            EMail_Home = values[9],
                            Start_Date = DateTime.Parse(values[10], cultureInfo),

                        };
                        dbContext.Employees.Add(employee);
                        if (dbContext.SaveChanges() <= 0)
                        {
                            throw new Exception("Could not saved");
                        }

                        done.Count++;
                    }

                }
            }
            catch (Exception e)
            {
                done.Message = e.Message.ToString();
                done.IsUploaded = false;
                done.Count = 0;
                transaction.Rollback();

            }

            if (done.IsUploaded)
            {
                transaction.Commit();
            }

            return done;
        }




        public IEnumerable<Employee> Sorting(IEnumerable<Employee> getAllEmployees, string sortOrder)
        {

            var sortedViewModel = from s in getAllEmployees
                                  select s;
            switch (sortOrder)
            {
                case "payroll_number":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Payroll_Number);
                    break;
                case "payroll_number_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Payroll_Number);
                    break;
                case "forenames":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Forenames);
                    break;
                case "forenames_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Forenames);
                    break;
                case "surname_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Surname);
                    break;
                case "birth_date":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Date_of_Birth);
                    break;
                case "birth_date_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Date_of_Birth);
                    break;
                case "telephone":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Telephone);
                    break;
                case "telephone_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Telephone);
                    break;
                case "mobile":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Mobile);
                    break;
                case "mobile_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Mobile);
                    break;
                case "address":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Address);
                    break;
                case "address_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Address);
                    break;
                case "address2":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Address_2);
                    break;
                case "address2_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Address_2);
                    break;
                case "postcode":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Postcode);
                    break;
                case "postcode_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Postcode);
                    break;
                case "email_home":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.EMail_Home);
                    break;
                case "email_home_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.EMail_Home);
                    break;
                case "start_date":
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Start_Date);
                    break;
                case "start_date_desc":
                    sortedViewModel = sortedViewModel.OrderByDescending(s => s.Start_Date);
                    break;
                default:
                    sortedViewModel = sortedViewModel.OrderBy(s => s.Surname);
                    break;
            }

            return sortedViewModel;
        }







        public IEnumerable<Employee> Serching(IEnumerable<Employee> getAllEmployees, Employee search)
        {
            var serchedViewModel = from s in getAllEmployees
                                  select s;

            if (search.Payroll_Number != null)
            {
                serchedViewModel = serchedViewModel.Where(s => s.Payroll_Number.ToLower().Contains(search.Payroll_Number.ToLower()));

            }

            if (search.Forenames != null)
            {
                serchedViewModel = serchedViewModel.Where(s => s.Forenames.ToLower().Contains(search.Forenames.ToLower()));
            }

            if (search.Surname != null)
            {
                serchedViewModel = serchedViewModel.Where(s => s.Surname.ToLower().Contains(search.Surname.ToLower()));
            }

            if (search.Telephone != null)
            {
                serchedViewModel = serchedViewModel.Where(s => s.Telephone.ToLower().Contains(search.Telephone.ToLower()));
            }

            if (search.Mobile != null)
            {
                serchedViewModel = serchedViewModel.Where(s => s.Mobile.ToLower().Contains(search.Mobile.ToLower()));
            }
            

            if (search.Address != null)
            {
                serchedViewModel = serchedViewModel.Where(s => s.Address.ToLower().Contains(search.Address.ToLower()));
            }
            

            if (search.Address_2 != null)
            {
                serchedViewModel = serchedViewModel.Where(s => s.Address_2.ToLower().Contains(search.Address_2.ToLower()));
            }
            

            if (search.Postcode != null)
            {
                serchedViewModel = serchedViewModel.Where(s => s.Postcode.ToLower().Contains(search.Postcode.ToLower()));
            }
            

            if (search.EMail_Home != null)
            {
                serchedViewModel = serchedViewModel.Where(s => s.EMail_Home.ToLower().Contains(search.EMail_Home.ToLower()));
            }


            if (search.Date_of_Birth != default(DateTime))
            {
                serchedViewModel = serchedViewModel.Where(s => s.Date_of_Birth.Date == search.Date_of_Birth.Date);
            }

            if (search.Start_Date != default(DateTime))
            {
                serchedViewModel = serchedViewModel.Where(s => s.Start_Date.Date == search.Start_Date.Date);
            }

            return serchedViewModel;
        }







    }
}
