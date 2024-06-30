using Microsoft.AspNetCore.Mvc;
using Task9.Controllers;
using Task9.Data;
using Task9.DTO;
using Task9.Models;

namespace Task9
{
    public class PatientsControllerTests
    {
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public PatientsControllerTests()
        {
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
        }

        [Fact]
        public async Task GetPatient_ValidId_ReturnsPatientData()
        {
            using var context = new AppDbContext(_contextOptions);
            var controller = new PatientsController(context);

            var patient = new Patient
            {
                IdPatient = 1,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1980, 1, 1)
            };

            var doctor = new Doctor
            {
                IdDoctor = 1,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com"
            };

            var medicament = new Medicament
            {
                IdMedicament = 1,
                Name = "Aspirin",
                Description = "Pain reliever",
                Type = "OTC"
            };

            var prescription = new Prescription
            {
                IdPrescription = 1,
                Date = new DateTime(2022, 1, 1),
                DueDate = new DateTime(2022, 2, 1),
                Doctor = doctor,
                Patient = patient
            };

            var prescriptionMedicament = new Prescription_Medicament
            {
                Prescription = prescription,
                Medicament = medicament,
                Dose = 10,
                Details = "Take daily"
            };

            prescription.Prescription_Medicaments.Add(prescriptionMedicament);
            context.Patients.Add(patient);
            context.Doctors.Add(doctor);
            context.Medicaments.Add(medicament);
            context.Prescriptions.Add(prescription);
            context.Prescription_Medicaments.Add(prescriptionMedicament);
            await context.SaveChangesAsync();

            var result = await controller.GetPatient(1);
            var okResult = result as OkObjectResult;
            var patientData = okResult.Value as PatientResponseDto;

            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(1, patientData.IdPatient);
            Assert.Single(patientData.Prescriptions);
            Assert.Equal(1, patientData.Prescriptions.First().Medicaments.Count);
        }

        [Fact]
        public async Task GetPatient_InvalidId_ReturnsNotFound()
        {
            using var context = new AppDbContext(_contextOptions);
            var controller = new PatientsController(context);

            var result = await controller.GetPatient(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }

}
