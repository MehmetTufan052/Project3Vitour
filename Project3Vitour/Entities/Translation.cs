using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Project3Vitour.Entities
{
    public class Translation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TranslationId { get; set; } = string.Empty;

        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string FieldName { get; set; } = string.Empty;
        public string LanguageCode { get; set; } = string.Empty;
        public string SourceLanguageCode { get; set; } = "tr";
        public string Value { get; set; } = string.Empty;
        public bool IsHtml { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
