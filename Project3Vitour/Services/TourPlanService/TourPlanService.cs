using AutoMapper;
using MongoDB.Driver;
using Project3Vitour.Dtos.TourPlanDto;
using Project3Vitour.Entities;
using Project3Vitour.Settings;

namespace Project3Vitour.Services.TourPlanService
{
    public class TourPlanService : ITourPlanService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<TourPlan> _tourPlanCollection;

        public TourPlanService(IMapper mapper,IDatabaseSettings databaseSettings)
        {
            var client=new MongoClient(databaseSettings.ConnectionString);
            var database=client.GetDatabase(databaseSettings.DatabaseName);
            _tourPlanCollection=database.GetCollection<TourPlan>(databaseSettings.TourPlanCollectionName);
            _mapper = mapper;
        }

       
        public Task CreateTourPlanAsync(CreateTourPlanDto createTourPlanDto)
        {
            var values=_mapper.Map<TourPlan>(createTourPlanDto);
            return _tourPlanCollection.InsertOneAsync(values);
        }

        public async Task DeleteTourPlanAsync(string id)
        {
            await _tourPlanCollection.DeleteOneAsync(x => x.TourPlanId == id);
        }

        public async Task<List<ResultTourPlanDto>> GetAllTourPlanAsync()
        {
            var result=await  _tourPlanCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultTourPlanDto>>(result);
        }

        public async Task<GetTourPlanByIdDto> GetTourPlanByIdAsync(string id)
        {
            var value =await  _tourPlanCollection.Find(x => x.TourPlanId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetTourPlanByIdDto>(value);
        }

        public Task UpdateTourPlanAsync(UpdateTourPlanDto updateTourPlanDto)
        {
            var values=_mapper.Map<TourPlan>(updateTourPlanDto);
            return _tourPlanCollection.FindOneAndReplaceAsync(x => x.TourPlanId == updateTourPlanDto.TourPlanId, values);
        }
    }
}
