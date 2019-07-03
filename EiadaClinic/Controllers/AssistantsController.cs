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
    public class AssistantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssistantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Assistants
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Assistants.Include(a => a.Doctor);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Assistants/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistant = await _context.Assistants
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assistant == null)
            {
                return NotFound();
            }

            return View(assistant);
        }

        // GET: Assistants/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id");
            return View();
        }

        // POST: Assistants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DoctorId")] Assistant assistant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assistant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", assistant.DoctorId);
            return View(assistant);
        }

        // GET: Assistants/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistant = await _context.Assistants.FindAsync(id);
            if (assistant == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", assistant.DoctorId);
            return View(assistant);
        }

        // POST: Assistants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,DoctorId")] Assistant assistant)
        {
            if (id != assistant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assistant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssistantExists(assistant.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", assistant.DoctorId);
            return View(assistant);
        }

        // GET: Assistants/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assistant = await _context.Assistants
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assistant == null)
            {
                return NotFound();
            }

            return View(assistant);
        }

        // POST: Assistants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var assistant = await _context.Assistants.FindAsync(id);
            _context.Assistants.Remove(assistant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssistantExists(string id)
        {
            return _context.Assistants.Any(e => e.Id == id);
        }
    }
}
