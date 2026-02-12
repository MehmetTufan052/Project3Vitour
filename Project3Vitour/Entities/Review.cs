using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Project3Vitour.Entities
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ReviewId { get; set; }
        public string NameSurname { get; set; }
        public string Detail { get; set; }
        public int Score { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool Status { get; set; }

        public string TourId { get; set; }
    }
}
