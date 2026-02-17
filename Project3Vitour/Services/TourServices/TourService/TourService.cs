using AutoMapper;
using MongoDB.Driver;
using Project3Vitour.Dtos.TourDtos;
using Project3Vitour.Entities;
using Project3Vitour.Settings;

namespace Project3Vitour.Services.TourServices.ITourService
{
    public class TourService : ITourService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Tour> _toursCollection;

        public TourService(IMapper mapper,IDatabaseSettings _databaseSettings)
        {
            var client =new MongoClient(_databaseSettings.ConnectionString);
            var database= client.GetDatabase(_databaseSettings.DatabaseName);
            _toursCollection = database.GetCollection<Tour>(_databaseSettings.TourCollectionName);
            _mapper = mapper;
        }

        public async Task CreateTourAsync(CreateTourDto createTourDto)
        {
            var value=_mapper.Map<Tour>(createTourDto);
            await _toursCollection.InsertOneAsync(value);
        }

        public async Task DeleteTourAsync(string id)
        {
            await _toursCollection.DeleteOneAsync(x => x.TourId == id);
        }

        public async Task<List<ResultTourDto>> GetAllTourAsync()
        {
           var values= await _toursCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultTourDto>>(values);
        }

        public async Task<GetTourByIdDto> GetTourByIdAsync(string id)
        {
            var value=await _toursCollection.Find(x => x.TourId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetTourByIdDto>(value);
        }

        public async Task UpdateTourAsync(UpdateTourDto updateTourDto)
        {
            var values=_mapper.Map<Tour>(updateTourDto);
            await _toursCollection.FindOneAndReplaceAsync(x => x.TourId == updateTourDto.TourId, values);
        }
    }
}
