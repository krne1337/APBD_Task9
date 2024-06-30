using Microsoft.AspNetCore.Mvc;
using Task9.Data;
using Task9.DTO;
using Task9.Models;

namespace Task9.Controllers
{
    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionRequestDto request)
    {
        if (request.Medicaments.Count > 10)
        {
            return BadRequest("A prescription can include a maximum of 10 medications.");
        }

        if (request.DueDate < request.Date)
        {
            return BadRequest("DueDate cannot be earlier than Date.");
        }

        var patient = await _context.Patients.FindAsync(request.Patient.IdPatient);
        if (patient == null)
        {
            patient = new Patient
            {
                FirstName = request.Patient.FirstName,
                LastName = request.Patient.LastName,
                BirthDate = request.Patient.BirthDate
            };
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }

        var prescription = new Prescription
        {
            Patient = patient,
            Date = request.Date,
            DueDate = request.DueDate,
            IdDoctor = request.IdDoctor
        };

        foreach (var med in request.Medicaments)
        {
            var medicament = await _context.Medicaments.FindAsync(med.IdMedicament);
            if (medicament == null)
            {
                return BadRequest($"Medicament with Id {med.IdMedicament} does not exist.");
            }

            prescription.Prescription_Medicaments.Add(new Prescription_Medicament
            {
                Medicament = medicament,
                Dose = med.Dose,
                Details = med.Details
            });
        }

        _context.Prescriptions.Add(prescription);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { IdPrescription = prescription.IdPrescription });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            return Conflict(new { message = "A concurrency conflict occurred. Please try again." });
        }
    }

}