namespace Project3Vitour.Dtos.ReviewDto
{
    public class ResultReviewDto
    {
        public string ReviewId { get; set; }
        public string NameSurname { get; set; }
        public string Detail { get; set; }
        public int Score { get; set; }
        public int ValueForMoneyScore { get; set; }
        public int DestinationScore { get; set; }
        public int AccommodationScore { get; set; }
        public int TransportScore { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool Status { get; set; }
        public string TourId { get; set; }

        public string SentimentLabel { get; set; }
        public double SentimentScore { get; set; }
    }
}
