namespace VotaYa.Models
{
    public class Opcion
    {
        public int Id { get; set; }
        public string Texto { get; set; } = string.Empty;
        public int Votos { get; set; } = 0;

        public int PreguntaId { get; set; }
        public Pregunta? Pregunta { get; set; }
    }
}
