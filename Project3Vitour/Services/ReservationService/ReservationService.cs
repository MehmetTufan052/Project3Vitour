using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using Project3Vitour.Dtos.ReservationDtos;
using Project3Vitour.Entities;
using Project3Vitour.Settings;
using System.Text.RegularExpressions;

namespace Project3Vitour.Services.ReservationService
{
    public class ReservationService : IReservationService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Reservation> _reservationCollection;
        private readonly IMongoCollection<Tour> _tourCollection;

        public ReservationService(IDatabaseSettings _databaseSettings, IMapper mapper)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _reservationCollection = database.GetCollection<Reservation>(_databaseSettings.ReservationCollectionName);
            _tourCollection = database.GetCollection<Tour>(_databaseSettings.TourCollectionName);
            _mapper = mapper;
        }

        public async Task CreateReservationAsync(CreateReservationDto createReservationDto)
        {
            var value = _mapper.Map<Reservation>(createReservationDto);
            if (string.IsNullOrWhiteSpace(value.ReservationId))
            {
                value.ReservationId = null!;
            }
            await _reservationCollection.InsertOneAsync(value);
        }

        public async Task DeleteReservationAsync(string id)
        {
            await _reservationCollection.DeleteOneAsync(x => x.ReservationId == id);
        }

        public async Task<List<ResultReservationDto>> GetAllReservationAsync()
        {
            var values = await _reservationCollection.Find(x => true).ToListAsync();
            var reservations = _mapper.Map<List<ResultReservationDto>>(values);

            var tourIds = reservations
                .Select(x => x.TourId)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var tours = await _tourCollection
                .Find(x => tourIds.Contains(x.TourId))
                .ToListAsync();

            var tourMap = tours.ToDictionary(x => x.TourId, x => x.Title);

            foreach (var reservation in reservations)
            {
                reservation.TourTitle = reservation.TourId is not null && tourMap.TryGetValue(reservation.TourId, out var title)
                    ? title
                    : reservation.TourId;
            }

            return reservations;
        }

        public async Task<GetReservationByIdDto> GetReservationByIdAsync(string id)
        {
            var value = await _reservationCollection.Find(x => x.ReservationId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetReservationByIdDto>(value);
        }

        public async Task<bool> ExistsReservationForTourAndEmailAsync(string tourId, string email)
        {
            if (string.IsNullOrWhiteSpace(tourId) || string.IsNullOrWhiteSpace(email))
                return false;

            var normalizedEmail = email.Trim();
            var tourFilter = Builders<Reservation>.Filter.Eq(x => x.TourId, tourId);
            var emailFilter = Builders<Reservation>.Filter.Regex(
                x => x.Email,
                new BsonRegularExpression($"^{Regex.Escape(normalizedEmail)}$", "i"));

            return await _reservationCollection.Find(tourFilter & emailFilter).AnyAsync();
        }

        public async Task UpdateReservationAsync(UpdateReservationDto updateReservationDto)
        {
            var values = _mapper.Map<Reservation>(updateReservationDto);
            await _reservationCollection.FindOneAndReplaceAsync(x => x.ReservationId == updateReservationDto.ReservationId, values);
        }
    }
}
