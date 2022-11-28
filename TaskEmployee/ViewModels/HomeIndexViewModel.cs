using System.ComponentModel.DataAnnotations;
using TaskEmployee.DataAcess.Models;
using TaskEmployee.Models;

namespace TaskEmployee.ViewModels
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Employee> GetAllEmployees { get; set; }

        public UploadedDataInfo UploadedDataInfo { get; set; }
        public PageInfo PageInfo { get; set; }


    }
}
