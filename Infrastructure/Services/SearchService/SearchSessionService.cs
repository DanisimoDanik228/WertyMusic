using Domain.Models;

namespace Infrastructure.Services.SearchService;

public class SearchSessionService<T>
{
    Dictionary<string, List<T>> _storage;

    public SearchSessionService()
    {
        _storage = new();
    }

    public void AddConnection(string connectionId)
    {
        if (_storage.ContainsKey(connectionId))
        {
            _storage[connectionId].Clear();
        }
        else
        {
            _storage.Add(connectionId,new List<T>());
        }
    }

    public void Clear(string connectionId)
    {
        if (!_storage.ContainsKey(connectionId))
        {
            throw new ArgumentException($"Connection with id {connectionId} does not exist");
        }

        _storage[connectionId].Clear();
    }
    
    public void Add(string connectionId,T item)
    {
        if (!_storage.ContainsKey(connectionId))
        {
            throw new ArgumentException($"Connection with id {connectionId} does not exist");
        }
        
        _storage[connectionId].Add(item);
    }

    public IList<T> Get(string connectionId)
    {
        if (!_storage.ContainsKey(connectionId))
        {
            throw new ArgumentException($"Connection with id {connectionId} does not exist");
        }
        
        return _storage[connectionId];
    }
}