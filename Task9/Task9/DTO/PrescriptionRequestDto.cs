namespace Task9.DTO
{
    public class PrescriptionRequestDto
    {
        public PatientDto Patient { get; set; }
        public List<MedicamentDto> Medicaments { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class PatientDto
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class MedicamentDto
    {
        public int IdMedicament { get; set; }
        public int Dose { get; set; }
        public string Details { get; set; }
    }

}
