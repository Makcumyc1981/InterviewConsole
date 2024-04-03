using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeeService.Models
{

    public class GetEmployeeByIdResponse
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public Employee Employee { get; set; } = new Employee();
    }
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ManagerId { get; set; }
        public List<Employee> Employees { get; set;} = new List<Employee>();
    }
}
