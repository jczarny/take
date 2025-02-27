﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CompanyApi.Models;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : Controller
    {
        private readonly CompanyDbContext _context;

        public EmployeesController(CompanyDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployee()
        {
            return await _context.Employees
            .Select(e => EmployeeToDTO(e))
            .ToListAsync();
        }


        // GET: Employees/Details/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDTO>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return EmployeeToDTO(employee);
        }


        // GET: Employees/Create
        [Route("create", Name = "CreateEmployee")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            ViewData["ManagerId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([Bind("EmployeeId,FirstName,LastName,ManagerId,Salary,Bonus,DepartmentId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", employee.DepartmentId);
            ViewData["ManagerId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", employee.ManagerId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id,
         EmployeeDTO employeeDTO)
        {
            if (id != employeeDTO.EmployeeId)
            {
                return BadRequest();
            }
            _context.Entry(DTOToEmployee(employeeDTO)).State =
            EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }


        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> PostEmployee(EmployeeDTO employeeDTO)
        {
            _context.Employees.Add(DTOToEmployee(employeeDTO));
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetEmployee",
            new { id = employeeDTO.EmployeeId }, employeeDTO);
        }


        // GET: Employees/Delete/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<EmployeeDTO>> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return EmployeeToDTO(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'CompanyDbContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private static EmployeeDTO EmployeeToDTO(Employee employee) =>
             new EmployeeDTO
             {
                 EmployeeId = employee.EmployeeId,
                 FirstName = employee.FirstName,
                 LastName = employee.LastName,
                 ManagerId = employee.ManagerId,
                 Salary = employee.Salary,
                 Bonus = employee.Bonus,
                 DepartmentId = employee.DepartmentId
             };
        private static Employee DTOToEmployee(EmployeeDTO employeeDTO) =>
            new Employee
            {
                EmployeeId = employeeDTO.EmployeeId,
                FirstName = employeeDTO.FirstName,
                LastName = employeeDTO.LastName,
                ManagerId = employeeDTO.ManagerId,
                Salary = employeeDTO.Salary,
                Bonus = employeeDTO.Bonus,
                DepartmentId = employeeDTO.DepartmentId
            };

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmployeeId == id)).GetValueOrDefault();
        }
    }
}
