using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Project3Vitour.Entities
{
    [BsonIgnoreExtraElements]
    public class TourPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TourPlanId { get; set; }
        public int NumberOfDay { get; set; }
        public string DayTitle { get; set; }
        public string Description { get; set; }

        public string TourId { get; set; }
        public string TourName { get; set; }
    }
}
