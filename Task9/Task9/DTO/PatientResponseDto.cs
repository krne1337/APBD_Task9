namespace Task9.DTO
{
    public class PatientResponseDto
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public List<PrescriptionResponseDto> Prescriptions { get; set; }
    }

    public class PrescriptionResponseDto
    {
        public int IdPrescription { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public List<MedicamentResponseDto> Medicaments { get; set; }
        public DoctorDto Doctor { get; set; }
    }

    public class MedicamentResponseDto
    {
        public int IdMedicament { get; set; }
        public string Name { get; set; }
        public int Dose { get; set; }
        public string Details { get; set; }
    }

    public class DoctorDto
    {
        public int IdDoctor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}
