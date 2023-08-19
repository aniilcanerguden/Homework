using AutoMapper;
using Homework.Models;
using Homework.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Homework.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly IMapper _mapper;
        public EmployeeController(IMapper mapper)
        {
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetListEmployee")]
        public async Task<IActionResult> GetEmployeeList()
        {
            NorthwndContext context = new NorthwndContext();
            var employeeList = context.Employees.Select(a => new EmployeeList()
            {
                FirstName = a.FirstName,
                LastName = a.LastName,
                Position = a.Position,
                EnteredJobDate = a.EnteredJobDate
            }).ToList();

            return Ok(employeeList);
        }

        [HttpGet]
        [Route("GetEmployee/{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            NorthwndContext context = new NorthwndContext();
            var user = context.Employees.FirstOrDefault(x => x.EmployeeId == id);
            var employeeDTO = _mapper.Map<SelectedEmployeeList>(user);
            return Ok(employeeDTO);
        }

        [HttpPost]
        [Route("AddEmployee")]
        public async Task<IActionResult> AddEmployees(EmployeeAdd employeeAdd)
        {
            NorthwndContext context = new NorthwndContext();
            Employee employee = new Employee();
            employee.FirstName = employeeAdd.FirstName;
            employee.LastName = employeeAdd.LastName;
            employee.Position = employeeAdd.Position;
            employee.EnteredJobDate = employeeAdd.EnteredJobDate;
            employee.ReportsTo = employeeAdd.ReportsTo;
            var getByIdEmployee = await context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employee.ReportsTo);
            employee.ReportsToNavigation = getByIdEmployee;

            if (employee != null)
            {
                context.Employees.Add(employee);
                context.SaveChanges();
                return Created("Employee is added successfull", employee);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPut]
        [Route("Update/{id}")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeAdd employeeUpdate, int id)
        {
            NorthwndContext context = new NorthwndContext();
            var updateEmployeeById = await context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == id);

            if (employeeUpdate != null)
            {
                updateEmployeeById.FirstName = employeeUpdate.FirstName;
                updateEmployeeById.LastName = employeeUpdate.LastName;
                updateEmployeeById.Position = employeeUpdate.Position;
                updateEmployeeById.EnteredJobDate = employeeUpdate.EnteredJobDate;
                updateEmployeeById.ReportsTo = employeeUpdate.ReportsTo;
                var getByIdEmployee = await context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == updateEmployeeById.ReportsTo);
                updateEmployeeById.ReportsToNavigation = getByIdEmployee;
                context.Employees.Update(updateEmployeeById);
                context.SaveChanges();
            }
            else
            {
                return NotFound("Id is not found.");
            }
            return Ok(updateEmployeeById);
        }
        [HttpDelete]
        [Route("Delete/{id}")]

        public async Task<IActionResult> DeleteEmployeeById(int id)
        {
            NorthwndContext context = new NorthwndContext();
            var deleteEmployeeById = await context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == id);
            if (deleteEmployeeById != null)
            {
                context.Employees.Remove(deleteEmployeeById);
                context.SaveChanges();
                return NoContent();
            }
            else
            {
                return NotFound("User is not Found.");
            }
        }

        [HttpGet]
        [Route("GetOrders/{id}")]
        public async Task<IActionResult> GetOrdersByEmployee(int id)
        {
            NorthwndContext context = new NorthwndContext();
            var ordersWithCustomers = from employee in context.Employees
                                      where employee.EmployeeId == id
                                      from order in employee.Orders
                                      join customer in context.Customers
                                          on order.CustomerId equals customer.CustomerId
                                      select new EmployeeListOrders
                                      {
                                          Siparis_No = order.OrderId,
                                          Musteri_Adi = customer.CompanyName,
                                          Siparis_Tarihi = order.OrderDate,
                                          Toplam_Tutar = 300     //Ayrıca bir table da coloumn olarak olmadıgından değeri 300 verdim.
                                      };

            if (ordersWithCustomers == null)
            {
                return NotFound();
            }

            return Ok(ordersWithCustomers.ToList());
        }
    }
}
