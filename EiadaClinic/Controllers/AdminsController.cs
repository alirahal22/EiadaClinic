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
using EiadaClinic.Models.ViewModels;

namespace EiadaClinic.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EiadaUser> _userManager;
        private readonly SignInManager<EiadaUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        string[] genders = new String[2] { "Male", "Female" };
        

        public AdminsController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<EiadaUser> userManager, SignInManager<EiadaUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        public async Task<IActionResult> Index()
        {
            
            return await Doctors();
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
            ViewBag.Action = "Doctors";
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .ToListAsync();
            return View("./Doctors/Index", doctors);
        }
        
        public async Task<IActionResult> Doctor(string id)
        {
            ViewBag.Action = "Doctors";
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
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
            
            ViewBag.Gender = genders.Select(g => new SelectListItem { Text = g, Value = g }); ;
            ViewBag.Action = "Doctors";
            ViewData["AssistantId"] = new SelectList(_context.Assistants.Include(a => a.User).ToList(), "Id", "User.UserName");
            return View("./Doctors/Create");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(DoctorCreationBindingModel doctorCreationBindingModel)
        {
            ViewBag.Gender = genders.Select(g => new SelectListItem { Text = g, Value = g }); ;
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
            return View("./Doctors/Create", doctor);
        }
        
        public async Task<IActionResult> EditDoctor(string id)
        {
            ViewBag.Action = "Doctors";
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.Where(d => d.Id.Equals(id)).Include(d => d.User).SingleAsync();
            if (doctor == null)
            {
                return NotFound();
            }
            ViewBag.Gender = genders.Select(g => new SelectListItem { Text = g, Value = g }); ;
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

                    doctor.Specialty = doctorCreationBindingModel.Specialty;
                    doctor.OpenTime = doctorCreationBindingModel.OpenTime;
                    doctor.CloseTime = doctorCreationBindingModel.CloseTime;
                    MapUser(doctor.User, doctorCreationBindingModel);
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
            return View("./Doctors/Edit", doctorCreationBindingModel);
        }
        
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            ViewBag.Action = "Doctors";
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
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
            var assistantContext = _context.Assistants.Where(a => a.DoctorId.Equals(id));
            if (assistantContext.ToList().Count() > 0)
            {
                var assistant = await assistantContext.SingleAsync();
                assistant.DoctorId = null;
                _context.Update(assistant);
                await _context.SaveChangesAsync();
            }

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
            ViewBag.Action = "Assistants";
            var applicationDbContext = _context.Assistants.Include(a => a.User).Include(a => a.Doctor).ThenInclude(d => d.User);
            return View("./Assistants/Index" ,await applicationDbContext.ToListAsync());
        }
        
        public async Task<IActionResult> Assistant(string id)
        {
            ViewBag.Action = "Assistants";
            if (id == null)
            {
                return NotFound();
            }

            var assistant = await _context.Assistants
                .Include(a => a.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assistant == null)
            {
                return NotFound();
            }

            return View("./Assistants/Details", assistant);
        }
        
        public IActionResult CreateAssistant()
        {
            ViewBag.Action = "Assistants";
            var doctors = _context.Doctors.Include(d => d.User).ToList();
            var doctorsList = doctors.Select(d => new SelectListItem { Text = d.User.UserName, Value = d.Id });
            
            ViewBag.Gender = genders.Select(g => new SelectListItem { Text = g, Value = g }); ;
            ViewBag.DoctorId = doctorsList;
            return View("./Assistants/Create");
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssistant(AssistantCreationBindingModel assistantCreationBindingModel)
        {
            ViewBag.Action = "Assistants";
            //Create 'Assistant' Role if it doesn't exist
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
                   
                    await _context.SaveChangesAsync();
                    
                    
                    await _context.SaveChangesAsync();

                    await _userManager.AddToRoleAsync(user, RoleString);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Assistants");
                
            }
            var doctors = _context.Doctors.Include(d => d.User).ToList();
            ViewBag.Gender = genders.Select(g => new SelectListItem { Text = g, Value = g });
            ViewBag.DoctorId = doctors.Select(d => new SelectListItem { Text = d.User.UserName, Value = d.Id });
            return View("./Assistants/Create", assistant);
        }
        
        public async Task<IActionResult> EditAssistant(string id)
        {
            ViewBag.Action = "Assistants";
            if (id == null)
            {
                return NotFound();
            }
            var assistant = await _context.Assistants
                .Where(a => a.Id.Equals(id))
                .Include(a => a.User)
                .Include(a => a.Doctor)
                    .Include(d => d.User)
                .SingleAsync();
            if (assistant == null)
            {
                return NotFound();
            }
            var doctors = _context.Doctors.Include(d => d.User).ToList();
            ViewBag.Gender = genders.Select(g => new SelectListItem { Text = g, Value = g });
            ViewBag.DoctorId = doctors.Select(d => new SelectListItem { Text = d.User.UserName, Value = d.Id });
            return View("./Assistants/Edit", assistant.ToAssistantCreationBindingModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssistant(string id, AssistantCreationBindingModel assistantCreationBindingModel)
        {
            ViewBag.Action = "Assistants";
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
            var doctors = _context.Doctors.Include(d => d.User).ToList();
            ViewBag.Gender = genders.Select(g => new SelectListItem { Text = g, Value = g });
            ViewBag.DoctorId = doctors.Select(d => new SelectListItem { Text = d.User.UserName, Value = d.Id });
            return View("./Assistants/Edit", assistantCreationBindingModel);
        }
        
        public async Task<IActionResult> DeleteAssistant(string id)
        {
            ViewBag.Action = "Assistants";
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
            ViewBag.Action = "Assistants";
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
            ViewBag.Action = "InsuranceCompanies";
            var applicationDbContext = _context.InsuranceCompanies.Include(i => i.User);
            return View("./InsuranceCompanies/Index", await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> InsuranceCompany(string id)
        {
            ViewBag.Action = "InsuranceCompanies";
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
            ViewBag.Action = "InsuranceCompanies";
            return View("./InsuranceCompanies/Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompany(InsuranceCompanyCreationBindingModel insuranceCompanyCreationBindingModel)
        {
            ViewBag.Action = "InsuranceCompanies";
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
            ViewBag.Action = "InsuranceCompanies";
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
            ViewBag.Action = "InsuranceCompanies";
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
            ViewBag.Action = "InsuranceCompanies";
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
            ViewBag.Action = "InsuranceCompanies";
            var insuranceCompany = await _context.InsuranceCompanies.FindAsync(id);
            _context.InsuranceCompanies.Remove(insuranceCompany);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(InsuranceCompanies));
        }


        private bool InsuranceCompanyExists(string id)
        {
            return _context.InsuranceCompanies.Any(e => e.Id == id);
        }

        ////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////
       
        public async Task<IActionResult> Messages() {
            ViewBag.Action = "Messages";
            int[] priorities = new int[5] { 1, 2, 3, 4, 5 };
            var prioritiesList = priorities.Select(p => new SelectListItem() { Text = p.ToString(), Value = p.ToString() });
            ViewBag.Priority = prioritiesList;
            var messages = await _context.Messages.Include(m => m.sender).ToListAsync();
            var reminders = await _context.Reminders.ToListAsync();
            return View(new MessagesViewModel() { Reminders = reminders, Messages = messages });
        }
        
        public async Task<IActionResult> DeleteMessage(string id)
        {
            ViewBag.Action = "Messages";
            var message = await _context.Messages
                 .FindAsync(id);
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Messages));
        }

        public async Task<IActionResult> AddReminder(MessagesViewModel messagesViewModel)
        {
            await _context.Reminders.AddAsync(messagesViewModel.Reminder);
            await _context.SaveChangesAsync();
            return RedirectToAction("Messages");
            
        }

        public async Task<IActionResult> DeleteReminder(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
            return RedirectToAction("Messages");

        }



        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
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
            user.Email = userCreationBindingModel.Email;
            user.Gender = userCreationBindingModel.Gender;
            user.Address = userCreationBindingModel.Address;
            user.Birthday = userCreationBindingModel.Birthday;
            user.PhoneNumber = userCreationBindingModel.PhoneNumber;
        }
    }
}
