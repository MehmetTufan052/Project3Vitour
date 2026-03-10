namespace Project3Vitour.Dtos.ReviewDto
{
    public class CreateReviewFormDto
    {
        public string NameSurname { get; set; }
        public string Email { get; set; }
        public string Detail { get; set; }

        public int ValueForMoneyScore { get; set; }
        public int DestinationScore { get; set; }
        public int AccommodationScore { get; set; }
        public int TransportScore { get; set; }

        public string TourId { get; set; }
        public bool AcceptTerms { get; set; }
    }
}
