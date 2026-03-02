namespace Project3Vitour.Settings
{
    public interface IDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string TourCollectionName { get; set; }
        public string CategoryCollectionName { get; set; }
        public string ReviewCollectionName { get; set; }
        public string ReservationCollectionName { get; set; }
        public string GalleryCollectionName { get; set; }
        public string TourPlanCollectionName { get; set; }
    }
}
