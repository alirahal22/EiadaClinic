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
using EiadaClinic.Models.ViewModels;

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

        public IActionResult CreateConsultation(string id) //id of patient
        {
            ViewData["PatientId"] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateConsultation(Consultation consultation, string PatientId)
        {
            consultation.Patient = await _context.Patients.Where(p => p.Id.Equals(PatientId)).SingleAsync();
            consultation.Doctor = await _context.Doctors.Where(d => d.Id.Equals(_activeUser.Id)).SingleAsync();

            _context.Add(consultation);
            await _context.SaveChangesAsync();
            return View();
        }

        public async Task<IActionResult> EditConsultation(string id)
        {
            return View( await _context.Consultations
                .Where(c => c.Id.Equals(id))
                .SingleAsync());
        }

        [HttpPost]
        public async Task<IActionResult> EditConsultation(Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                _context.Update(consultation);
                await _context.SaveChangesAsync();
                return View();
            }
            return View(consultation);  
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
