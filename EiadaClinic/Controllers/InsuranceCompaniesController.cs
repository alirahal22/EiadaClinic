﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Clinic.Models;
using EiadaClinic.Data;
using EiadaClinic.Models;
using EiadaClinic.Models.Singleton;

namespace EiadaClinic.Controllers
{
    public class InsuranceCompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ActiveUser _activeUser;

        public InsuranceCompaniesController(ApplicationDbContext context, ActiveUser activeUser)
        {
            _context = context;
            _activeUser = activeUser;
        }

        // GET: InsuranceCompanies
        public async Task<IActionResult> Index()
        {
            //get all patients of logged in company
            var patientIds = _context.Patients
                .Where(p => p.InsuranceCompany.Id.Equals(_activeUser.Id))
                .Select(p => p.Id)
                .ToList();
            var consultations = _context.Consultations
                .Where(c => patientIds.Contains(c.Patient.Id))
                .Include(c => c.Patient);
            return View(consultations);
        }
        
        

        

        
    }
}
