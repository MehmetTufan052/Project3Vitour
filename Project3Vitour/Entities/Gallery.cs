using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Project3Vitour.Entities
{
    public class Gallery
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GalleryId { get; set; }
        
        public string ImageUrl { get; set; }
        public string TourId { get; set; }
        public string TourName { get; set; }
    }
}
