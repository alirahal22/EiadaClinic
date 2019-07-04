using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EiadaClinic.Data;
using EiadaClinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using EiadaClinic.Models.BindingModels;

namespace EiadaClinic.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EiadaUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminsController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<EiadaUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Admins.Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }

        //////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////

        // GET: Doctors
        public async Task<IActionResult> Doctors()
        {
            var applicationDbContext = _context.Doctors.Include(d => d.Assistant).Include(d => d.User);
            return View("./Doctors/Index", await applicationDbContext.ToListAsync());
        }

        // GET: Doctors/Details/5
        
        public async Task<IActionResult> Doctor(string id)
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

            return View("./Doctors/Details", doctor);
        }

        // GET: Doctors/Create
        public IActionResult CreateDoctor()
        {
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id");
            return View("./Doctors/Create");
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(DoctorCreationBindingModel doctorCreationBindingModel)
        {
            //Create 'Doctor' Role if it doesn't exist
            string RoleString = "Doctor";
            var role = await _roleManager.RoleExistsAsync(RoleString);
            if (!role)
                await _roleManager.CreateAsync(new IdentityRole(RoleString));

            Doctor doctor = null;
            //Validate Model
            if (ModelState.IsValid)
            {
                var user = CreateUser(doctorCreationBindingModel);
                //create user and assign role if successful
                var result = await _userManager.CreateAsync(user, doctorCreationBindingModel.Password);
                if (result.Succeeded)
                {
                    //Fill role related attributes
                    doctor = new Doctor()
                    {
                        Specialty = doctorCreationBindingModel.Specialty,
                        OpenTime = doctorCreationBindingModel.OpenTime,
                        CloseTime = doctorCreationBindingModel.CloseTime,
                        UserId = user.Id
                    };
                    doctor.User = user;
                    _context.Add(doctor);
                    _context.SaveChanges();

                    await _userManager.AddToRoleAsync(user, RoleString);
                }

                if (result.Succeeded)
                {
                    return RedirectToAction("Doctors");
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Doctors");
            }
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id", doctor.AssistantId);
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", doctor.UserId);
            return View("./Doctors/Create", doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> EditDoctor(string id)
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
            return View("./Doctors/Edit", doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor(string id, [Bind("Id,UserId,Specialty,OpenTime,CloseTime,AssistantId")] Doctor doctor)
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
                return RedirectToAction("Doctors");
            }
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id", doctor.AssistantId);
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", doctor.UserId);
            return View("./Doctors/Edit", doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> DeleteDoctor(string id)
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

            return View("./Doctors/Delete", doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction("Doctors");
        }


        private bool DoctorExists(string id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }

        private EiadaUser CreateUser(UserCreationBindingModel userCreationBindingModel)
        {
            return new EiadaUser()
            {
                FirstName = userCreationBindingModel.FirstName,
                MiddleName = userCreationBindingModel.MiddleName,
                LastName = userCreationBindingModel.LastName,
                UserName = userCreationBindingModel.UserName,
                PasswordHash = userCreationBindingModel.Password,
                Email = userCreationBindingModel.Email,
                Gender = userCreationBindingModel.Gender,
                Birthday = userCreationBindingModel.Birthday,
                PhoneNumber = userCreationBindingModel.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }

        //////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////

        // GET: Assistants
        public async Task<IActionResult> Assistants()
        {
            var applicationDbContext = _context.Assistants.Include(a => a.Doctor);
            return View("./Assistants/Index" ,await applicationDbContext.ToListAsync());
        }

        // GET: Assistants/Details/5
        public async Task<IActionResult> Assistant(string id)
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

            return View("./Assistants/Details", assistant);
        }

        // GET: Assistants/Create
        public IActionResult CreateAssistant()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id");
            return View("./Assistants/Create");
        }

        // POST: Assistants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssistant(UserCreationBindingModel userCreationBindingModel)
        {
            //Create 'Doctor' Role if it doesn't exist
            string RoleString = "Assistant";
            var role = await _roleManager.RoleExistsAsync(RoleString);
            if (!role)
                await _roleManager.CreateAsync(new IdentityRole(RoleString));
            Assistant assistant = null;
            //Validate Model
            if (ModelState.IsValid)
            {
                var user = CreateUser(userCreationBindingModel);
                //create user and assign role if successful
                var result = await _userManager.CreateAsync(user, userCreationBindingModel.Password);
                if (result.Succeeded)
                {
                    //Fill role related attributes
                    assistant = new Assistant()
                    {
                        User = user
                    };
                    _context.Add(assistant);
                    _context.SaveChanges();

                    await _userManager.AddToRoleAsync(user, RoleString);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Assistants");
                
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "UserName", assistant.DoctorId);
            return View("./Assistants/Create", assistant);
        }

        // GET: Assistants/Edit/5
        public async Task<IActionResult> EditAssistant(string id)
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "UserName", assistant.DoctorId);
            return View("./Assistants/Edit", assistant);
        }

        // POST: Assistants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssistant(string id, [Bind("Id,DoctorId")] Assistant assistant)
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
                return RedirectToAction("Assistants");
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "UserName", assistant.DoctorId);
            return View("./Assistants/Edit", assistant);
        }

        // GET: Assistants/Delete/5
        public async Task<IActionResult> DeleteAssistant(string id)
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
            return View("./Assistants/Delete", assistant);
        }

        // POST: Assistants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsConfirmed(string id)
        {
            var assistant = await _context.Assistants.FindAsync(id);
            _context.Assistants.Remove(assistant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Assistants));
        }

        private bool AssistantExists(string id)
        {
            return _context.Assistants.Any(e => e.Id == id);
        }

    }
}
