
using Microsoft.AspNetCore.Http;
using TaskEmployee.DataAcess.Models;

namespace TaskEmployee.Models
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int id);

        IEnumerable<Employee> GetAllEmployees();

        Employee Create(Employee employee);

        Employee Update(Employee employee);
        Employee Delete(int id);

        Task<UploadedDataInfo> ImportFromExcel(IFormFile file);
        Task<UploadedDataInfo> ImportFromCsv(IFormFile file);

        IEnumerable<Employee> Sorting(IEnumerable<Employee> getAllEmployees, string sortOrder);
        IEnumerable<Employee> Serching(IEnumerable<Employee> getAllEmployees, Employee search);
    }
}
