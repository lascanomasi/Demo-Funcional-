namespace VotaYa.Models
{
    public class Pregunta
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public bool Activa { get; set; } = false;
        public DateTime CreadaEn { get; set; } = DateTime.Now;

        public List<Opcion> Opciones { get; set; } = new();
    }
}
