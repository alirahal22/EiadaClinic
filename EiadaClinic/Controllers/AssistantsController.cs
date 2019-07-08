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
using EiadaClinic.Models.ViewModels;
using Microsoft.Extensions.Logging;

namespace EiadaClinic.Controllers
{
    public class AssistantsController : Controller
    {
        private ActiveUser _activeUser;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<EiadaUser> _userManager;
        private readonly SignInManager<EiadaUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AssistantsController> _logger;

        string[] gender = new string[2] { "Male", "Female" };
        string[] bloodtypes = new string[8] { "A+", "B+", "O+", "AB+", "A-", "B-", "O-", "AB-" };
        string[] times = new string[24] { "1:00 AM", "2:00 AM", "3:00 AM", "4:00 AM", "5:00 AM", "6:00 AM", "7:00 AM", "8:00 AM", "9:00 AM", "10:00 AM", "11:00 AM", "12:00 AM",
            "1:00 PM", "2:00 PM", "3:00 PM", "4:00 PM", "5:00 PM", "6:00 PM", "7:00 PM", "8:00 PM", "9:00 PM", "10:00 PM", "11:00 PM", "12:00 PM" };
        public AssistantsController(ApplicationDbContext context, 
            SignInManager<EiadaUser> signInManager, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<EiadaUser> userManager, 
            ActiveUser activeUser,
            ILogger<AssistantsController> logger)
        {
            _activeUser = activeUser;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            ViewBag.Action = "Home";
            var assistant = await _context.Assistants
                .Where(a => a.Id.Equals(_activeUser.Id))
                .Include(a => a.User)
                .Include(a => a.Doctor)
                .SingleAsync();
            List<Patient> patients = await _context.PatientDoctors
                .Where(pd => assistant.Doctor.Id.Equals(pd.Doctor.Id))
                .Select(pd => pd.Patient)
                .Include(p => p.User)
                .ToListAsync();
            return View(patients);
        }

        public async Task<IActionResult> Appointments()
        {
            ViewBag.Action = "Appointments";
            

            var assistant = await _context.Assistants
                .Where(a => a.Id.Equals(_activeUser.Id))
                .Include(a => a.User)
                .Include(a => a.Doctor)
                .SingleAsync();

            List<Appointment> appointments = await _context.Appointments
                .Where(a => a.Doctor.Id.Equals(assistant.DoctorId))
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .ToListAsync();
            return View(appointments);
        }

        public async Task<IActionResult> Patient(string id)
        {
            ViewBag.Action = "Home";

            var assistant = await _context.Assistants
                .Where(a => a.Id.Equals(_activeUser.Id))
                .Include(a => a.User)
                .Include(a => a.Doctor)
                .SingleAsync();
            var patientViewModel = new PatientViewModel()
            {
                Patient = await _context.Patients
                    .Where(p => p.Id.Equals(id))
                    .Include(p => p.User)
                    .Include(p => p.Consultations)
                    .Include(p => p.Appointments)
                    .SingleAsync(),
                Appointments = await _context.Appointments
                    .Where(a => a.Patient.Id.Equals(id) && a.Doctor.Id.Equals(assistant.DoctorId))
                    .ToListAsync()
            };
            return View(patientViewModel);
        }

        //Create Patient
        public IActionResult CreatePatient()
        {
            ViewBag.Action = "Home";
            ViewBag.Gender = gender.Select(g => new SelectListItem { Text = g, Value = g });
            ViewBag.BloodType = bloodtypes.Select(g => new SelectListItem { Text = g, Value = g });
            return View();
        }

        //Create Patient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient(PatientCreationBindingModel patientCreationBindingModel)
        {
            ViewBag.Gender = gender.Select(g => new SelectListItem { Text = g, Value = g });
            ViewBag.BloodType = bloodtypes.Select(g => new SelectListItem { Text = g, Value = g });
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

                    var assistant = await _context.Assistants
                    .Where(a => a.Id.Equals(_activeUser.Id))
                    .Include(a => a.User)
                    .Include(a => a.Doctor)
                    .SingleAsync();
                    Doctor doctor = _context.Doctors.Where(d => d.Id.Equals(assistant.Id)).Single();
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
            return View("Create", patient);
        }

        public async Task<IActionResult> EditPatient(string id)
        {
            ViewBag.Action = "Home";

            ViewBag.Gender = gender.Select(g => new SelectListItem { Text = g, Value = g });
            ViewBag.BloodType = bloodtypes.Select(g => new SelectListItem { Text = g, Value = g });

            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Where(p => p.Id.Equals(id))
                .Include(p => p.User)
                .SingleAsync();
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(string id, Patient patient)
        {

            #region DontOpen
            EiadaUser eiadaUser = await _userManager.FindByIdAsync(patient.User.Id);
            eiadaUser.Email = patient.User.Email;
            eiadaUser.FirstName = patient.User.FirstName;
            eiadaUser.MiddleName = patient.User.MiddleName;
            eiadaUser.LastName = patient.User.LastName;
            eiadaUser.UserName = patient.User.UserName;
            eiadaUser.PhoneNumber = patient.User.PhoneNumber;
            #endregion

            if (id != patient.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _context.Update(patient);
                await _userManager.UpdateAsync(eiadaUser);
                await _context.SaveChangesAsync();
                return Redirect("~/Assistants/Patient/" + patient.Id);
            }
            ViewBag.Gender = gender.Select(g => new SelectListItem { Text = g, Value = g });
            ViewBag.BloodType = bloodtypes.Select(g => new SelectListItem { Text = g, Value = g });
            return View(patient);
        }

        public async Task<IActionResult> CreateAppointment(string id)
        {
            ViewBag.Time = times.Select(g => new SelectListItem { Text = g, Value = g });
            ViewBag.Action = "Appointments";
            ViewBag.PatientId = id;
            ViewBag.DoctorId = await _context.Assistants
                .Where(a => a.Id.Equals(_activeUser.Id))
                .Select(a => a.DoctorId)
                .SingleAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            var app = new Appointment()
            {
                Date = appointment.Date,
                Doctor = await _context.Doctors.FindAsync(appointment.Doctor.Id),
                Patient = await _context.Patients.FindAsync(appointment.Patient.Id)


            };
            _context.Appointments.Add(app);
            await _context.SaveChangesAsync();
            return Redirect("~/Assistants/Patient/" + appointment.Patient.Id);
        }

        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.Where(a => a.Id == id).Include(a => a.Patient).SingleAsync();
            _context.Remove(appointment);
            _context.SaveChanges();
            return Redirect("~/Assistants/Patient/" + appointment.Patient.Id);

        }

        public async Task<IActionResult> EditAppointment(int id)
        {
            ViewBag.Action = "Appointments";
            ViewBag.Time = times.Select(g => new SelectListItem { Text = g, Value = g }); 
            var appointment = await _context.Appointments
                .Where(a => a.Id == id)
                .Include(a => a.Patient)
                .SingleAsync();
            return View(appointment);
        }
        [HttpPost]
        public async Task<IActionResult> EditAppointment(Appointment appointment)
        {
            _context.Update(appointment);
            await _context.SaveChangesAsync();
            return Redirect("~/Assistants/Patient/" + appointment.Patient.Id);
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
                Address = userCreationBindingModel.Address,
                PasswordHash = userCreationBindingModel.Password,
                Email = userCreationBindingModel.Email,
                Gender = userCreationBindingModel.Gender,
                Birthday = userCreationBindingModel.Birthday,
                PhoneNumber = userCreationBindingModel.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}
