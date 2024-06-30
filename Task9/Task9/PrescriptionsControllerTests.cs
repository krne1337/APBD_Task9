using Microsoft.AspNetCore.Mvc;
using Task9.Controllers;
using Task9.Data;
using Task9.DTO;
using Task9.Models;

namespace Task9
{
    public class PrescriptionsControllerTests
    {
        private readonly DbContextOptions<AppDbContext> _contextOptions;

        public PrescriptionsControllerTests()
        {
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;
        }

        [Fact]
        public async Task AddPrescription_ValidRequest_ReturnsOkResult()
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
            context.SaveChanges();

            var prescriptionRequestDto = new PrescriptionRequestDto
            {
                Patient = patientDto,
                Medicaments = new List<MedicamentDto> { medicamentDto },
                Date = new DateTime(2022, 1, 1),
                DueDate = new DateTime(2022, 2, 1)
            };

            var result = await controller.AddPrescription(prescriptionRequestDto);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task AddPrescription_InvalidMedicament_ReturnsBadRequest()
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
                IdMedicament = 999, // Invalid Id
                Dose = 10,
                Details = "Take daily"
            };

            var prescriptionRequestDto = new PrescriptionRequestDto
            {
                Patient = patientDto,
                Medicaments = new List<MedicamentDto> { medicamentDto },
                Date = new DateTime(2022, 1, 1),
                DueDate = new DateTime(2022, 2, 1)
            };

            var result = await controller.AddPrescription(prescriptionRequestDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task AddPrescription_TooManyMedications_ReturnsBadRequest()
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

            var medicaments = new List<MedicamentDto>();
            for (int i = 1; i <= 11; i++)
            {
                medicaments.Add(new MedicamentDto
                {
                    IdMedicament = i,
                    Dose = 10,
                    Details = "Take daily"
                });

                context.Medicaments.Add(new Medicament
                {
                    IdMedicament = i,
                    Name = $"Med {i}",
                    Description = $"Desc {i}",
                    Type = "OTC"
                });
            }
            context.SaveChanges();

            var prescriptionRequestDto = new PrescriptionRequestDto
            {
                Patient = patientDto,
                Medicaments = medicaments,
                Date = new DateTime(2022, 1, 1),
                DueDate = new DateTime(2022, 2, 1)
            };

            var result = await controller.AddPrescription(prescriptionRequestDto);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }

}
