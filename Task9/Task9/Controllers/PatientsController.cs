using Microsoft.AspNetCore.Mvc;
using Task9.Data;
using Task9.DTO;

namespace Task9.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{idPatient}")]
        public async Task<IActionResult> GetPatient(int idPatient)
        {
            var patient = await _context.Patients
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Prescription_Medicaments)
                        .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Doctor)
                .FirstOrDefaultAsync(p => p.IdPatient == idPatient);

            if (patient == null)
            {
                return NotFound();
            }

            var response = new PatientResponseDto
            {
                IdPatient = patient.IdPatient,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                BirthDate = patient.BirthDate,
                Prescriptions = patient.Prescriptions
                    .OrderBy(pr => pr.DueDate)
                    .Select(pr => new PrescriptionResponseDto
                    {
                        IdPrescription = pr.IdPrescription,
                        Date = pr.Date,
                        DueDate = pr.DueDate,
                        Medicaments = pr.Prescription_Medicaments.Select(pm => new MedicamentResponseDto
                        {
                            IdMedicament = pm.Medicament.IdMedicament,
                            Name = pm.Medicament.Name,
                            Dose = pm.Dose,
                            Details = pm.Details
                        }).ToList(),
                        Doctor = new DoctorDto
                        {
                            IdDoctor = pr.Doctor.IdDoctor,
                            FirstName = pr.Doctor.FirstName,
                            LastName = pr.Doctor.LastName
                        }
                    }).ToList()
            };

            return Ok(response);
        }
    }
}