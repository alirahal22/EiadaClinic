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
using EiadaClinic.Models.Singleton;
using Clinic.Models.ViewModels;
using Clinic.Models;

namespace EiadaClinic.Controllers
{
    
    public class DoctorsController : Controller
    {
        private ActiveUser _activeUser;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EiadaUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public DoctorsController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<EiadaUser> userManager, ActiveUser activeUser)
        {
            _activeUser = activeUser;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = _userManager.FindByNameAsync(_activeUser.UserName);


            List<Patient> patients = await _context.PatientDoctors
                .Where(pd => pd.Doctor.User.UserName.Equals(_activeUser.UserName))
                .Select(pd => pd.Patient)
                .Include(pd => pd.User)
                .ToListAsync();

            List<Appointment> appointments = await _context.Appointments
                .Where(a => a.Doctor.User.UserName.Equals(_activeUser.UserName))
                .ToListAsync();
            return View(new DoctorViewModel() { Doctor = new Doctor(), Patients = patients, Appointments = appointments });
        }

        public async Task<IActionResult> Patient(string id)
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

        //Create Patient
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id");
            return View("Create");
        }

        //Create Patient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PatientCreationBindingModel patientCreationBindingModel)
        {
            //Create 'Patient' Role if it doesn't exist
            string RoleString = "Patient";
            var role = await _roleManager.RoleExistsAsync(RoleString);
            if (!role)
                await _roleManager.CreateAsync(new IdentityRole(RoleString));

            Patient patient = null;
            //Validate Model
            if (ModelState.IsValid)
            {
                var user = CreateUser(patientCreationBindingModel);
                //create user and assign role if successful
                var result = await _userManager.CreateAsync(user, patientCreationBindingModel.Password);
                if (result.Succeeded)
                {
                    //Fill role related attributes
                    patient = new Patient()
                    {
                        BloodType = patientCreationBindingModel.BloodType,
                        User = user
                    };
                    _context.Add(patient);
                    _context.SaveChanges();


                    //var activeUser = _userManager.FindByNameAsync(_activeUser.UserName);
                    Doctor doctor = _context.Doctors.Where(d => d.Id.Equals(_activeUser.Id)).Single();
                    PatientDoctor pd = new PatientDoctor() { Patient = patient, Doctor = doctor };
                    _context.Add(pd);
                    await _context.SaveChangesAsync();
                    await _userManager.AddToRoleAsync(user, RoleString);
                }

                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                
                return RedirectToAction("Create", patientCreationBindingModel);
            }
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", patient.Id);
            return View("Create", patient);
        }
        
        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", patient.Id);
            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PatientCreationBindingModel patientCreationBindingModel)
        {
            if (id != patientCreationBindingModel.Id)
            {
                return NotFound();
            }
            Patient patient = null;
            if (ModelState.IsValid)
            {
                try
                {
                    EiadaUser user = CreateUser(patientCreationBindingModel);
                    user.Id = patientCreationBindingModel.Id;
                    patient = new Patient() { Id = patientCreationBindingModel.Id, BloodType = patientCreationBindingModel.BloodType };
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
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
            ViewData["UserId"] = new SelectList(_context.Set<EiadaUser>(), "Id", "Id", patient.Id);
            return View(patientCreationBindingModel);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(string id)
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

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(string id)
        {
            return _context.Patients.Any(e => e.Id == id);
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
    }
}
