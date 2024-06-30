using Microsoft.AspNetCore.Mvc;
using Task9.Controllers;
using Task9.Data;
using Task9.DTO;
using Task9.Models;

namespace Task9
{
    public class PrescriptionsControllerConcurrencyTests
    {
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public PrescriptionsControllerConcurrencyTests()
        {
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
        }

        [Fact]
        public async Task AddPrescription_ConcurrencyConflict_ReturnsConflict()
        {
            using var context = new AppDbContext(_contextOptions);
            var controller = new PrescriptionsController(context);

            var patientDto = new PatientDto
            {
                IdPatient = 1,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1980, 1, 1)
            };

            var medicamentDto = new MedicamentDto
            {
                IdMedicament = 1,
                Dose = 10,
                Details = "Take daily"
            };

            context.Medicaments.Add(new Medicament
            {
                IdMedicament = 1,
                Name = "Aspirin",
                Description = "Pain reliever",
                Type = "OTC"
            });
            await context.SaveChangesAsync();

            var prescriptionRequestDto = new PrescriptionRequestDto
            {
                Patient = patientDto,
                Medicaments = new List<MedicamentDto> { medicamentDto },
                Date = new DateTime(2022, 1, 1),
                DueDate = new DateTime(2022, 2, 1)
            };

            using (var concurrentContext = new AppDbContext(_contextOptions))
            {
                var prescription = new Prescription
                {
                    IdPrescription = 1,
                    Date = new DateTime(2022, 1, 1),
                    DueDate = new DateTime(2022, 2, 1),
                    Patient = new Patient
                    {
                        IdPatient = 1,
                        FirstName = "John",
                        LastName = "Doe",
                        BirthDate = new DateTime(1980, 1, 1)
                    },
                    IdDoctor = 1
                };
                concurrentContext.Prescriptions.Add(prescription);
                await concurrentContext.SaveChangesAsync();

                var prescriptionToUpdate = await concurrentContext.Prescriptions.FirstAsync();
                prescriptionToUpdate.DueDate = new DateTime(2022, 3, 1);
                await concurrentContext.SaveChangesAsync();
            }

            var result = await controller.AddPrescription(prescriptionRequestDto);

            Assert.IsType<ConflictObjectResult>(result);
        }
    }

}
