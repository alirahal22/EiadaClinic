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

namespace EiadaClinic.Controllers
{
    public class PatientsController : Controller
    {
        private ActiveUser _activeUser;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EiadaUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PatientsController(ApplicationDbContext context, UserManager<EiadaUser> userManager, RoleManager<IdentityRole> roleManager, ActiveUser activeUser)
        {
            _context = context;
            _activeUser = activeUser;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            List<Doctor> doctors = await _context.PatientDoctors
                .Where(pd => pd.Patient.User.UserName.Equals(_activeUser.UserName))
                .Select(pd => pd.Doctor)
                .Include(pd => pd.User)
                .ToListAsync();

            return View(doctors);

        }


        public async Task<IActionResult> Appointments()
        {
            List<Appointment> appointments = await _context.Appointments
                .Where(p => p.Patient.User.UserName.Equals(_activeUser.UserName))
                .ToListAsync();
            return View(appointments);

        }


        public async Task<IActionResult> Doctor(string doctorid)
        {
            if (doctorid == null)
            {
                return NotFound();
            }

            var patient = await _context.Doctors.FindAsync(doctorid);
            if (patient == null)
            {
                return NotFound();
            }

            List<Consultation> consultations = await _context.Consultations
                .Where(c => c.Doctor.Id.Equals(doctorid) && c.Patient.Id.Equals(_activeUser.Id)).ToListAsync();


            return View(consultations);

        }

        public IActionResult SearchByName(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            List<Doctor> doctors = _context.Doctors.Where(d => d.User.FirstName.Contains(searchString) || d.User.LastName.Contains(searchString))
                .ToList();
            return View(doctors);

        }
    }
}
