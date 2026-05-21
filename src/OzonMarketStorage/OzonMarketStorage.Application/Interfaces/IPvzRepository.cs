using OzonMarketStorage.Domain;

namespace OzonMarketStorage.Application.Interfaces;

public interface IPvzRepository
{
    Task Add(Pvz pvz);

    Task<Pvz> Get(Guid id);
    
    Task Update(Pvz pvz);
    
    Task Delete(Guid id);
}