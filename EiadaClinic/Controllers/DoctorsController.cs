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
        private readonly SignInManager<EiadaUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public DoctorsController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<EiadaUser> userManager, ActiveUser activeUser, SignInManager<EiadaUser> signInManager)
        {
            _activeUser = activeUser;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Action = "Home";
            List<Patient> patients = await _context.PatientDoctors
                .Where(pd => pd.Doctor.User.UserName.Equals(_activeUser.UserName))
                .Select(pd => pd.Patient)
                .Include(pd => pd.User)
                .ToListAsync();
            List<Appointment> appointments = await _context.Appointments
                .Where(a => a.Doctor.User.UserName.Equals(_activeUser.UserName))
                .Include(a => a.Patient)
                .ToListAsync();
            return View(new DoctorViewModel() { Doctor = new Doctor(), Patients = patients, Appointments = appointments });
        }

        public async Task<IActionResult> Patient(string id)
        {
            ViewBag.Action = "Home";
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
            var consultations = await _context.Consultations
                .Where(c => c.Patient.Id == id && c.Doctor.Id == _activeUser.Id)
                .Include(c => c.Patient)
                .ToListAsync();
            return View(new DoctorPatientViewModel() { Patient = patient, Consultations = consultations });
        }

        public IActionResult CreateConsultation(string id) //id of patient
        {
            ViewBag.Action = "Home";
            ViewData["DoctorId"] = _activeUser.Id;
            ViewData["PatientId"] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateConsultation(Consultation consultation)
        {
            consultation.Patient = await _context.Patients.Where(p => p.Id.Equals(consultation.Patient.Id)).SingleAsync();
            consultation.Doctor = await _context.Doctors.Where(p => p.Id.Equals(consultation.Doctor.Id)).SingleAsync();
            _context.Add(consultation);
            await _context.SaveChangesAsync();
            return Redirect("~/Doctors/Patient/" + consultation.Patient.Id);
        }

        public async Task<IActionResult> EditConsultation(int id)
        {
            ViewBag.Action = "Home";
            var consulation = await _context.Consultations
                .Where(c => c.Id == id)
                .Include(c => c.Patient)
                .SingleAsync();
            ViewData["DoctorId"] = _activeUser.Id;
            ViewData["PatientId"] = consulation.Patient.Id;
            return View(consulation);
        }

        [HttpPost]
        public async Task<IActionResult> EditConsultation(Consultation consultation)
        {
            if (ModelState.IsValid)
            {
                consultation.Patient = await _context.Patients.Where(p => p.Id.Equals(consultation.Patient.Id)).SingleAsync();
                consultation.Doctor = await _context.Doctors.Where(p => p.Id.Equals(consultation.Doctor.Id)).SingleAsync();
                _context.Update(consultation);
                await _context.SaveChangesAsync();
                return Redirect("~/Doctors/Patient/" + consultation.Patient.Id);
            }
            return View(consultation);  
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult Message()
        {
            ViewBag.Action = "Message";
            ViewBag.Id = _activeUser.Id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            message.sender = (EiadaUser) await _context.Users.FindAsync(message.sender.Id);
            _context.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
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
