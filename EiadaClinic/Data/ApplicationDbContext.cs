using System;
using System.Collections.Generic;
using System.Text;
using Clinic.Models;
using EiadaClinic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EiadaClinic.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Consultation> Consultations { get; set; }
        public DbSet<InsuranceCompany> InsuranceCompanies{ get; set; }
        public DbSet<PatientDoctor> PatientDoctors{ get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
