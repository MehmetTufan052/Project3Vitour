namespace Project3Vitour.Dtos.ReviewDto
{
    public class SentimentSummaryDto
    {
        public string TourId { get; set; }
        public int ToplamYorum { get; set; }
        public int OlumluSayi { get; set; }
        public int OlumsuzSayi { get; set; }
        public int NotSayi { get; set; }
        public double OrtalamaGuven { get; set; }


        public double OlumluYuzde => ToplamYorum > 0
            ? Math.Round((double)OlumluSayi / ToplamYorum * 100, 1) : 0;
        public double OlumsuzYuzde => ToplamYorum > 0
            ? Math.Round((double)OlumsuzSayi / ToplamYorum * 100, 1) : 0;
        public double NotYuzde => ToplamYorum > 0
            ? Math.Round((double)NotSayi / ToplamYorum * 100, 1) : 0;
    }
}
