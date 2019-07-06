using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EiadaClinic.Data;
using EiadaClinic.Models;

namespace EiadaClinic.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Patients.Include(p => p.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }


        // GET: Patients/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id");
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", patient.Id);
            return View(patient);
        }
        

        

        

        
    }
}
