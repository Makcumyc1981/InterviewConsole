using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using EmployeeService.Models;


namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IEmployeeService
    {
        public GetEmployeeByIdResponse GetEmployeeById(int id)
        {
            int count = 0;
            GetEmployeeByIdResponse resp = new GetEmployeeByIdResponse()
            {
                ErrorCode = 0,
            };
            List<Employee> employees = new List<Employee>();
            try
            {
                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["EmploeeTest"].ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand($"WITH CTE AS (SELECT id, Name, ManagerID FROM Employee WHERE id = {id} " +
                        $"UNION ALL SELECT t.id, t.Name, t.ManagerID FROM Employee t INNER JOIN CTE c ON t.ManagerID = c.id) " +
                        $"SELECT * FROM CTE ORDER BY ManagerID;", con);
                    using (var reader = cmd.ExecuteReader())
                    {
                        var empIdCol = reader.GetOrdinal("Id");
                        var empNameCol = reader.GetOrdinal("Name");
                        var empManagerIdCol = reader.GetOrdinal("ManagerId");
                        while (reader.Read())
                        {
                            employees.Add(new Employee() {
                                Id = (int)reader.GetValue(empIdCol), 
                                Name = reader.GetString(empNameCol), 
                                ManagerId = (reader.GetValue(empManagerIdCol) == DBNull.Value ? 0 : (int)reader.GetValue(empManagerIdCol)) 
                            });
                        }
                        
                        foreach (var empl in employees)
                        {
                            if(count == 0)
                            {
                                resp.Employee = employees.FirstOrDefault(x => x.Id == id);
                                count++;
                            }
                            else
                            {
                                if(resp.Employee.Id == empl.ManagerId)
                                {
                                    resp.Employee.Employees.Add(empl);
                                }
                                else
                                {
                                    resp.Employee.Employees.FirstOrDefault(x => x.Id == empl.ManagerId).Employees.Add(empl);
                                }
                            }
                        }
                    }
                    return resp;
                }
            }
            catch (Exception ex)
            {
                resp.ErrorCode = 1;
                resp.ErrorMessage = ex.Message;
                return resp;
            }
        }

      

        public string EnableEmployee(int id, int enable)
        {
            List<int> enableValues = new List<int> { 0, 1 }; //possible enable values
            if(!enableValues.Contains(enable))
            {
                return $"Error Updating - wrong ENABLE value";
            }
            try
            {
                string sqlUpd = $"UPDATE Employee SET enable={enable} WHERE id={id}";
                using (SqlConnection con = new SqlConnection(WebConfigurationManager.ConnectionStrings["EmploeeTest"].ConnectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sqlUpd, con);
                    int number = cmd.ExecuteNonQuery();
                    return $"Update {number} records";
                }
            }
            catch (Exception ex)
            {
                return $"Error Updating - {ex.Message}";
            }
        }
    }

      
}