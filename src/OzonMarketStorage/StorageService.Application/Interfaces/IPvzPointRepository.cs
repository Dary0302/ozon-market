using StorageService.Domain;

namespace StorageService.Application.Interfaces;

public interface IPvzPointRepository
{
    Task Add(PvzPoint pvzPoint);

    Task<PvzPoint> Get(Guid id);
    
    Task Update(PvzPoint pvzPoint);
    
    Task Delete(Guid id);
}