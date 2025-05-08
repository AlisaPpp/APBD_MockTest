using Models;
namespace Application;

public interface ICurrencyService
{
    Task<bool> AddCurrency(CurrencyRequest request);
    Task<object?> SearchByType(SearchRequest request);
    
    
}