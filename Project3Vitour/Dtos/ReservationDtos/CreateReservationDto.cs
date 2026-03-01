using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Project3Vitour.Dtos.ReservationDtos
{
    public class CreateReservationDto
    {
        public string ReservationId { get; set; }
        public string NameSurname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime ReservationDate { get; set; }
        public int PersonCount { get; set; }
        public decimal TotalPrice { get; set; } 
        public string ReservationCode { get; set; }

        public string TourId { get; set; }
    }
}
