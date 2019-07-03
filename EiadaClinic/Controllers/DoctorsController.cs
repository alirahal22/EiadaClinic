using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EiadaClinic.Data;
using EiadaClinic.Models;
using EiadaClinic.Models.BindingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EiadaClinic.Controllers
{
    //[Authorize(Roles = "Doctor")]
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EiadaUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        

        

        public DoctorsController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<EiadaUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Doctors
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Doctors.Include(d => d.Assistant).Include(d => d.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Assistant)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id");
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorCreationBindingModel doctorCreationBindingModel)
        {
            if (ModelState.IsValid)
            {
                var user = new EiadaUser()
                {
                    FirstName = doctorCreationBindingModel.FirstName,
                    MiddleName = doctorCreationBindingModel.MiddleName,
                    LastName = doctorCreationBindingModel.LastName,
                    UserName = doctorCreationBindingModel.UserName,
                    PasswordHash = doctorCreationBindingModel.PasswordHash,
                    Email = doctorCreationBindingModel.Email,
                    Gender = doctorCreationBindingModel.Gender,
                    Birthday = doctorCreationBindingModel.Birthday,
                    PhoneNumber = doctorCreationBindingModel.PhoneNumber
                    
                };
                //_context.Add(user);
                //_context.SaveChanges();
                user.SecurityStamp = "ol,ikmujnyhbtgvrfc";
                var result = await _userManager.CreateAsync(user, doctorCreationBindingModel.PasswordHash);
                var role = await _roleManager.RoleExistsAsync("Doctor");
                //if (!role)
                //    await _roleManager.CreateAsync(new IdentityRole("Doctor"));
                //await _userManager.AddToRoleAsync(user, "Doctor");
                var doctor = new Doctor()
                {
                    Specialty = doctorCreationBindingModel.Specialty,
                    OpenTime = doctorCreationBindingModel.OpenTime,
                    CloseTime = doctorCreationBindingModel.CloseTime,
                    UserId = user.Id
                };
                doctor.User = user;
                _context.Add(doctor);
                _context.SaveChanges();
                
                if (result.Succeeded)
                {
                    //return LocalRedirect(returnUrl);
                    return RedirectToAction("Index", "Doctors");
                }


                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id", doctor.AssistantId);
            //ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", doctor.UserId);
            //return View(doctor);
            return View();
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id", doctor.AssistantId);
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", doctor.UserId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserId,Specialty,OpenTime,CloseTime,AssistantId")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
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
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id", doctor.AssistantId);
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", doctor.UserId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Assistant)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(string id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
