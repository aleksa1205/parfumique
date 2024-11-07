namespace FragranceRecommendation.DTOs;

public class UpdateParfemDTO
{
        public string Naziv { get; set; }
        public int GodinaIzlaska { get; set; } = DateTime.Now.Year;
        public char Pol { get; set; } = 'U';
}