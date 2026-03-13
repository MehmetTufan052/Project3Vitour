using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Project3Vitour.Entities
{
    [BsonIgnoreExtraElements]
    public class Tour
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TourId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public string CoverImageUrl { get; set; }
        [BsonElement("CoverImage")]
        [BsonIgnoreIfNull]
        public string LegacyCoverImage
        {
            get => CoverImageUrl;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(CoverImageUrl))
                {
                    CoverImageUrl = value;
                }
            }
        }
        public string Badge { get; set; }
        public int DayCount { get; set; }
        public int Capacity { get; set; }
        public decimal Price { get; set; }
        public bool IsStatus { get; set; }

        public string LocationPageImageUrl { get; set; }
        public string LocationPageTitle { get; set; }
        public string LocationPageDescription { get; set; }
        [BsonElement("LocationImageUrl")]
        [BsonIgnoreIfNull]
        public string LegacyLocationImageUrl
        {
            get => LocationPageImageUrl;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(LocationPageImageUrl))
                {
                    LocationPageImageUrl = value;
                }
            }
        }
        [BsonElement("LocationTitle")]
        [BsonIgnoreIfNull]
        public string LegacyLocationTitle
        {
            get => LocationPageTitle;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(LocationPageTitle))
                {
                    LocationPageTitle = value;
                }
            }
        }
        [BsonElement("LocationDescription")]
        [BsonIgnoreIfNull]
        public string LegacyLocationDescription
        {
            get => LocationPageDescription;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(LocationPageDescription))
                {
                    LocationPageDescription = value;
                }
            }
        }
    }
}
