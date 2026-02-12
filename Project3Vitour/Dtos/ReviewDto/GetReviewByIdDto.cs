namespace Project3Vitour.Dtos.ReviewDto
{
    public class GetReviewByIdDto
    {
        public string ReviewId { get; set; }
        public string NameSurname { get; set; }
        public string Detail { get; set; }
        public int Score { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool Status { get; set; }
        public string TourId { get; set; }
    }
}
