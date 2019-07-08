using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EiadaClinic.Data;
using EiadaClinic.Models;
using EiadaClinic.Models.Singleton;
using Microsoft.AspNetCore.Identity;
using EiadaClinic.Models.ViewModels;

namespace EiadaClinic.Controllers
{
    public class PatientsController : Controller
    {
        private ActiveUser _activeUser;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EiadaUser> _userManager;
        private readonly SignInManager<EiadaUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PatientsController(ApplicationDbContext context, SignInManager<EiadaUser> signInManager, UserManager<EiadaUser> userManager, RoleManager<IdentityRole> roleManager, ActiveUser activeUser)
        {
            _context = context;
            _activeUser = activeUser;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Action = "Home";
            List<Doctor> doctors = await _context.PatientDoctors
                .Where(pd => pd.Patient.User.UserName.Equals(_activeUser.UserName))
                .Select(pd => pd.Doctor)
                .Include(pd => pd.User)
                .ToListAsync();
            List<Appointment> appointments = await _context.Appointments
                .Where(p => p.Patient.User.UserName.Equals(_activeUser.UserName))
                .ToListAsync();
            PatientHomeViewModel patientHomeViewModel = new PatientHomeViewModel() {Doctors = doctors, Appointments = appointments };
            return View(patientHomeViewModel);
        }

        public async Task<IActionResult> Doctor(string id)
        {
            ViewBag.Action = "Home";
            var consultions = await _context.Consultations
                .Where(c => c.DoctorId == id && c.PatientId == _activeUser.Id)
                .ToListAsync();
            return View(consultions);
        }

        public async Task<IActionResult> Search(string searchString)
        {
            ViewBag.Action = "Home";
            ViewData["CurrentFilter"] = searchString;

            List<Doctor> doctors = await _context.PatientDoctors
                .Where(pd => pd.Patient.Id.Equals(_activeUser.Id) 
                    && (pd.Doctor.User.FirstName.Contains(searchString) 
                        || pd.Doctor.User.LastName.Contains(searchString) 
                        || pd.Doctor.User.LastName.Contains(searchString)))
                .Select(pd => pd.Doctor)
                .Include(pd => pd.User)
                .ToListAsync();
            List<Appointment> appointments = await _context.Appointments
                .Where(p => p.Patient.User.UserName.Equals(_activeUser.UserName))
                .ToListAsync();
            PatientHomeViewModel patientHomeViewModel = new PatientHomeViewModel() { Doctors = doctors, Appointments = appointments };
            return View("Index", patientHomeViewModel);

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
            message.sender = (EiadaUser)await _context.Users.FindAsync(message.sender.Id);
            _context.Add(message);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}
