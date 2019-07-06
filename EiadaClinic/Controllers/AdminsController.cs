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
using Clinic.Models;

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
        
        public async Task<IActionResult> Doctors()
        {
            var applicationDbContext = _context.Doctors.Include(d => d.Assistant).Include(d => d.User);
            return View("./Doctors/Index", await applicationDbContext.ToListAsync());
        }
        
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
        
        public IActionResult CreateDoctor()
        {
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id");
            return View("./Doctors/Create");
        }
        
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
                        Id = user.Id
                    };
                    doctor.User = user;
                    _context.Add(doctor);
                    _context.SaveChanges();

                    await _userManager.AddToRoleAsync(user, RoleString);
                }
                else
                {
                    return Content("Failed to add User");
                }

                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Doctors");
                }
                return Content("Failed to add Doctor");
            }
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id", doctor.AssistantId);
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", doctor.Id);
            return View("./Doctors/Create", doctor);
        }
        
        public async Task<IActionResult> EditDoctor(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.Where(d => d.Id.Equals(id)).Include(d => d.User).SingleAsync();
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id", doctor.AssistantId);
            return View("./Doctors/Edit", doctor.ToDoctorCreationBindingModel());
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor(string id, DoctorCreationBindingModel doctorCreationBindingModel)
        {
            if (id != doctorCreationBindingModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var doctor = _context.Doctors
                        .Where(d => d.Id.Equals(id))
                        .Include(d => d.User)
                        .Single();

                    var user = CreateUser(doctorCreationBindingModel);
                    user.Id = doctor.Id;

                    doctor.Specialty = doctorCreationBindingModel.Specialty;
                    doctor.AssistantId = doctorCreationBindingModel.AssistantId;
                    doctor.OpenTime = doctorCreationBindingModel.OpenTime;
                    doctor.CloseTime = doctorCreationBindingModel.CloseTime;
                    _context.Update(user);
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctorCreationBindingModel.Id))
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
            ViewData["AssistantId"] = new SelectList(_context.Assistants, "Id", "Id", doctorCreationBindingModel.AssistantId);
            return View("./Doctors/Edit", doctorCreationBindingModel);
        }
        
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
        
        [HttpPost, ActionName("DeleteDoctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDoctorConfirmed(string id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            var user = _context.Doctors.Where(d => d.Id.Equals(id)).Include(d => d.User).Single().User;
            _context.Doctors.Remove(doctor);
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Doctors");
        }
        
        private bool DoctorExists(string id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }

        

        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        
        public async Task<IActionResult> Assistants()
        {
            var applicationDbContext = _context.Assistants.Include(a => a.User).Include(a => a.Doctor).ThenInclude(d => d.User);
            return View("./Assistants/Index" ,await applicationDbContext.ToListAsync());
        }
        
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
        
        public IActionResult CreateAssistant()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id");
            return View("./Assistants/Create");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssistant(AssistantCreationBindingModel assistantCreationBindingModel)
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
                var user = CreateUser(assistantCreationBindingModel);
                //create user and assign role if successful
                var result = await _userManager.CreateAsync(user, assistantCreationBindingModel.Password);
                if (result.Succeeded)
                {
                    //Fill role related attributes
                    assistant = new Assistant()
                    {
                        User = user,
                        DoctorId = assistantCreationBindingModel.DoctorId
                    };
                    _context.Add(assistant);
                    _context.SaveChanges();

                    await _userManager.AddToRoleAsync(user, RoleString);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Assistants");
                
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", assistant.DoctorId);
            return View("./Assistants/Create", assistant);
        }
        
        public async Task<IActionResult> EditAssistant(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var assistant = await _context.Assistants
                .Where(a => a.Id.Equals(id))
                .Include(a => a.User)
                .Include(a => a.Doctor)
                .SingleAsync();
            if (assistant == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Include(d => d.User), "Id", "User.UserName", assistant.Doctor.Id);
            return View("./Assistants/Edit", assistant.ToAssistantCreationBindingModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssistant(string id, AssistantCreationBindingModel assistantCreationBindingModel)
        {
            if (id != assistantCreationBindingModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var assistant = await _context.Assistants.Where(a => a.Id.Equals(id)).Include(a => a.User).SingleAsync();
                    MapUser(assistant.User, assistantCreationBindingModel);
                    assistant.DoctorId = assistantCreationBindingModel.DoctorId;
                    _context.Update(assistant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssistantExists(assistantCreationBindingModel.Id))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "Id", "Id", assistantCreationBindingModel.DoctorId);
            return View("./Assistants/Edit", assistantCreationBindingModel);
        }
        
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
        
        [HttpPost, ActionName("DeleteAssistant")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAssistantConfirmed(string id)
        {
            var assistant = await _context.Assistants.FindAsync(id);
            var user = _context.Assistants.Where(a => a.Id.Equals(id)).Include(a => a.User).Single().User;
            _context.Assistants.Remove(assistant);
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Assistants));
        }

        private bool AssistantExists(string id)
        {
            return _context.Assistants.Any(e => e.Id == id);
        }


        public async Task<IActionResult> InsuranceCompanies()
        {
            var applicationDbContext = _context.InsuranceCompanies.Include(i => i.User);
            return View("./InsuranceCompanies/Index", await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> InsuranceCompany(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceCompany = await _context.InsuranceCompanies
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceCompany == null)
            {
                return NotFound();
            }

            return View("./InsuranceCompanies/Details", insuranceCompany);
        }

        public IActionResult CreateCompany()
        {
            return View("./InsuranceCompanies/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompany(InsuranceCompanyCreationBindingModel insuranceCompanyCreationBindingModel)
        {
            //Create 'InsuranceCompanie' Role if it doesn't exist
            string RoleString = "InsuranceCompanie";
            var role = await _roleManager.RoleExistsAsync(RoleString);
            if (!role)
                await _roleManager.CreateAsync(new IdentityRole(RoleString));

            InsuranceCompany insuranceCompany = null;
            //Validate Model
            if (ModelState.IsValid)
            {
                var user = CreateUser(insuranceCompanyCreationBindingModel);
                //create user and assign role if successful
                var result = await _userManager.CreateAsync(user, insuranceCompanyCreationBindingModel.Password);
                if (result.Succeeded)
                {
                    //Fill role related attributes
                    insuranceCompany = new InsuranceCompany()
                    {
                        Name = insuranceCompanyCreationBindingModel.Name,
                        Fax = insuranceCompanyCreationBindingModel.Fax
                    };
                    insuranceCompany.User = user;
                    _context.Add(insuranceCompany);
                    _context.SaveChanges();

                    await _userManager.AddToRoleAsync(user, RoleString);
                }
                else
                {
                    return Content("Failed to add User");
                }

                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(InsuranceCompanies));
                }
                return Content("Failed to add Company");
            }
            return View("./InsuranceCompanies/Create", insuranceCompanyCreationBindingModel);
        }

        public async Task<IActionResult> EditCompany(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceCompany = await _context.InsuranceCompanies
                .Where(i => i.Id.Equals(id))
                .Include(i => i.User)
                .SingleAsync();
            if (insuranceCompany == null)
            {
                return NotFound();
            }
            return View("./InsuranceCompanies/Edit", insuranceCompany.ToInsuranceCompanyCreationBindingModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCompany(string id, InsuranceCompanyCreationBindingModel insuranceCompanyCreationBindingModel)
        {
            if (id != insuranceCompanyCreationBindingModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var insuranceCompany = await _context.InsuranceCompanies
                        .Where(i => i.Id.Equals(id))
                        .Include(i => i.User)
                        .SingleAsync();
                    MapUser(insuranceCompany.User, insuranceCompanyCreationBindingModel);
                    insuranceCompany.Name = insuranceCompanyCreationBindingModel.Name;
                    _context.Update(insuranceCompany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssistantExists(insuranceCompanyCreationBindingModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(InsuranceCompanies));
            }
            return View("./InsuranceCompanies/Edit", insuranceCompanyCreationBindingModel);
        }

        public async Task<IActionResult> DeleteCompany(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insuranceCompany = await _context.InsuranceCompanies
                .Include(i => i.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (insuranceCompany == null)
            {
                return NotFound();
            }

            return View("./InsuranceCompanies/Delete", insuranceCompany);
        }
        
        [HttpPost, ActionName("DeleteCompany")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCompanyConfirmed(string id)
        {
            var insuranceCompany = await _context.InsuranceCompanies.FindAsync(id);
            _context.InsuranceCompanies.Remove(insuranceCompany);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(InsuranceCompanies));
        }


        private bool InsuranceCompanyExists(string id)
        {
            return _context.InsuranceCompanies.Any(e => e.Id == id);
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
                Address = userCreationBindingModel.Address,
                Birthday = userCreationBindingModel.Birthday,
                PhoneNumber = userCreationBindingModel.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }

        private void MapUser(EiadaUser user, UserCreationBindingModel userCreationBindingModel)
        {
            user.FirstName = userCreationBindingModel.FirstName;
            user.MiddleName = userCreationBindingModel.MiddleName;
            user.LastName = userCreationBindingModel.LastName;
            user.UserName = userCreationBindingModel.UserName;
            user.PasswordHash = userCreationBindingModel.Password;
            user.Email = userCreationBindingModel.Email;
            user.Gender = userCreationBindingModel.Gender;
            user.Address = userCreationBindingModel.Address;
            user.Birthday = userCreationBindingModel.Birthday;
            user.PhoneNumber = userCreationBindingModel.PhoneNumber;
        }
    }
}
