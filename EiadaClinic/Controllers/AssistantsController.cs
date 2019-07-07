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
using EiadaClinic.Models.Singleton;
using Microsoft.AspNetCore.Identity;

namespace EiadaClinic.Controllers
{
    public class AssistantsController : Controller
    {
        private ActiveUser _activeUser;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EiadaUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AssistantsController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<EiadaUser> userManager, ActiveUser activeUser)
        {
            _activeUser = activeUser;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<IActionResult> Index()
        {
            var assistant = await _context.Assistants
                .Where(a => a.Id.Equals(_activeUser.Id))
                .Include(a => a.User)
                .Include(a => a.Doctor)
                .SingleAsync();
            List<Patient> patients = await _context.PatientDoctors
                .Select(pd => pd.Patient)
                .Include(p => p.User)
                .ToListAsync();
            return View(patients);
        }

        public async Task<IActionResult> Appointments()
        {
            List<Appointment> appointments = await _context.Appointments
                .Where(a => a.Doctor.AssistantId.Equals(_activeUser.Id))
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .ToListAsync();
            return View(appointments);
        }

        public async Task<IActionResult> Patient(string id)
        {
            return View(await _context.Patients
                .Where(p => p.Id.Equals(id))
                .Include(p => p.Consultations)
                .Include(p => p.Appointments)
                .SingleAsync());
        }

        //Create Patient
        public IActionResult CreatePatient()
        {
            return View();
        }

        //Create Patient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient(PatientCreationBindingModel patientCreationBindingModel)
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
                    Doctor doctor = _context.Doctors.Where(d => d.AssistantId.Equals(_activeUser.Id)).Single();
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

        public async Task<IActionResult> EditPatient(string id)
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
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(string id, PatientCreationBindingModel patientCreationBindingModel)
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
