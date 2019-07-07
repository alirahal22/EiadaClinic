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
using Clinic.Models;

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
            _context = context;
            _activeUser = activeUser;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<IActionResult> Index()
        { 
            List<Patient> patients = await _context.PatientDoctors
                .Where(pd => pd.Doctor.Assistant.User.UserName.Equals(_activeUser.UserName))
                .Select(pd => pd.Patient)
                .Include(pd => pd.User)
                .ToListAsync();
            return View(patients);
        }

        public async Task<IActionResult> Appointments()
        {
            List<Appointment> appointments = await _context.Appointments
                .Where(a => a.Doctor.Assistant.User.UserName.Equals(_activeUser.UserName))
                .ToListAsync();
            return View(appointments);

        }

    








    }
}
