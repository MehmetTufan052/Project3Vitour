namespace Project3Vitour.Dtos.TourDtos
{
    public class ResultTourDto
    {
        public string TourId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public string CoverImageUrl { get; set; }
        public string Badge { get; set; }
        public int DayCount { get; set; }
        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public decimal Price { get; set; }
        public bool IsStatus { get; set; }

        public string LocationPageImageUrl { get; set; }
        public string LocationPageTitle { get; set; }
        public string LocationPageDescription { get; set; }
    }
}
