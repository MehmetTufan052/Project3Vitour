using Project3Vitour.Dtos.ReviewDto;

namespace Project3Vitour.Models
{
    public class SentimentViewModel
    {
        public SentimentSummaryDto Ozet { get; set; }
        public List<SentimentTrendDto> Trend { get; set; }
        public List<ResultReviewByTourIdDto> Yorumlar { get; set; }
    }
}
