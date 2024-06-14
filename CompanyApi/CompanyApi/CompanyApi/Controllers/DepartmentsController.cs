using System;
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
    public class DepartmentsController : Controller
    {
        private readonly CompanyDbContext _context;

        public DepartmentsController(CompanyDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetDepartments()
        {
            return await _context.Departments
                .Select(e => DepartmentToDTO(e))
                .ToListAsync();
        }

        // GET: Departments/Details/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDTO>> GetDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return DepartmentToDTO(department);
        }

        // GET: Departments/Create
        [Route("create", Name = "CreateDepartment")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<ActionResult<DepartmentDTO>> PostDepartment(DepartmentDTO departmentDTO)
        {
            _context.Departments.Add(DTOToDepartment(departmentDTO));
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDepartment", new { id = departmentDTO.DepartmentId }, departmentDTO);
        }

        // GET: Departments/Edit/5
        [HttpGet]
        [Route("edit/{id}", Name = "EditDepartment")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, DepartmentDTO departmentDTO)
        {
            if (id != departmentDTO.DepartmentId)
            {
                return BadRequest();
            }

            _context.Entry(DTOToDepartment(departmentDTO)).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
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

        // GET: Departments/Delete/5
        [HttpGet]
        [Route("delete/{id}", Name = "DeleteDepartment")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Departments == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static DepartmentDTO DepartmentToDTO(Department department) => new DepartmentDTO
        {
            DepartmentId = department.DepartmentId,
            Name = department.Name
        };
        private static Department DTOToDepartment(DepartmentDTO departmentDTO) => new Department
        {
            DepartmentId = departmentDTO.DepartmentId,
            Name = departmentDTO.Name
        };
        private bool DepartmentExists(int id)
        {
          return (_context.Departments?.Any(e => e.DepartmentId == id)).GetValueOrDefault();
        }
    }
}
