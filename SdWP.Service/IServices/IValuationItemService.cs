using SdWP.DTO.Responses;

namespace SdWP.Service.IServices
{
    public interface IValuationItemService
    {

        //definicje metod
        Task<List<ValuationItemResponse>> GetValuationItemsAsync(); 
        //przykładowa metoda do pobrania wszystkich wycen

        Task<List<ValuationItemResponse>> GetValuationItemByIdAsync(Guid id);
    }




}
