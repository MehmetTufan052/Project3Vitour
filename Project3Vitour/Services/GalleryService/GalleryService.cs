using AutoMapper;
using MongoDB.Driver;
using Project3Vitour.Dtos.GalleryDto;
using Project3Vitour.Entities;
using Project3Vitour.Settings;

namespace Project3Vitour.Services.GalleryService
{
    public class GalleryService : IGalleryService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Gallery> _galleryCollection;

        public GalleryService(IMapper mapper,IDatabaseSettings _databaseSettings)
        {
            var client=new MongoClient(_databaseSettings.ConnectionString);
            var database=client.GetDatabase(_databaseSettings.DatabaseName);
            _galleryCollection=database.GetCollection<Gallery>(_databaseSettings.GalleryCollectionName);
            _mapper = mapper;
        }

       
        public async Task CreateGalleryAsync(CreateGalleryDto createGalleryDto)
        {
            var values=_mapper.Map<Gallery>(createGalleryDto);
            await _galleryCollection.InsertOneAsync(values);
        }

        public async Task DeleteGalleryAsync(string id)
        {
            await _galleryCollection.DeleteOneAsync(x => x.GalleryId == id);
        }

        public async Task<List<ResultGalleryDto>> GetAllGalleryAsync()
        {
            var values=await _galleryCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultGalleryDto>>(values);

        }

        public async Task<GetGalleryByIdDto> GetGalleryByIdAsync(string id)
        {
            var values=await _galleryCollection.Find(x=>x.GalleryId==id).FirstOrDefaultAsync();
            return _mapper.Map<GetGalleryByIdDto>(values);
        }

        public Task UpdateGalleryAsync(UpdateGalleryDto updateGalleryDto)
        {
            var values=_mapper.Map<Gallery>(updateGalleryDto);
            return _galleryCollection.FindOneAndReplaceAsync(x=>x.GalleryId==updateGalleryDto.GalleryId,values);
        }
    }
}
