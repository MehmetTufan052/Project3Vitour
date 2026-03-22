using System.ComponentModel.DataAnnotations;

namespace Project3Vitour.Dtos.ReservationDtos
{
    public class CreateReservationDto
    {
        public string ReservationId { get; set; }

        [Required(ErrorMessage = "Ad soyad zorunludur.")]
        [RegularExpression(@"^\s*\S+.*\s+\S+.*$", ErrorMessage = "Lütfen ad ve soyad giriniz.")]
        public string NameSurname { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur.")]
        [RegularExpression(@"^[+\d][\d\s\-()]{7,16}$", ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şehir zorunludur.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Ülke zorunludur.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Rezervasyon tarihi zorunludur.")]
        public DateTime ReservationDate { get; set; }

        [Range(1, 50, ErrorMessage = "Kişi sayısı 1 ile 50 arasında olmalıdır.")]
        public int PersonCount { get; set; }

        public decimal TotalPrice { get; set; }
        public string ReservationCode { get; set; }

        [Required(ErrorMessage = "Tur seçimi zorunludur.")]
        public string TourId { get; set; }
    }
}
