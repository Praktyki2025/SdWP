using SdWP.DTO.Responses;

namespace SdWP.Service.IServices
{
    public interface IValuationItemService
    {

        //definicje metod
        Task<List<ValuationItemResponse>> GetValuationItemsAsync(); 
        //przykładowa metoda do pobrania wszystkich wycen

        Task<ValuationItemResponse> GetValuationItemByIdAsync(Guid id);

        //to taka jakby flaga? nie wei mczy moze tak byc jak wczesniej wzraca tylko ok i nie ma petli
        Task<bool> DeleteValuationItemAsync(Guid id);


        //CRUD - dodac CreateValuation item, UpdateValuationItem i DeleteValuationItem

        //dodac to samo tyle ze w repository 



    }




}
