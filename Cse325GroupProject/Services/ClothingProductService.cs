using MongoDB.Driver;
using Cse325GroupProject.Models;

namespace Cse325GroupProject.Services;

public class ClothingProductService
{
    private readonly IMongoCollection<Product> _collection;

    public ClothingProductService(IMongoClient client, IConfiguration config)
    {
        var dbName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME")
                 ?? config["MongoDB:DatabaseName"]
                 ?? "inventory-tracker";

        var database = client.GetDatabase(dbName);
        _collection = database.GetCollection<Product>("Products");

    }

    public async Task<List<Product>> GetAllAsync() =>
        await _collection.Find(_ => true).ToListAsync();

    public async Task<Product?> GetByIdAsync(string id) =>
        await _collection.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Product product) =>
        await _collection.InsertOneAsync(product);

    public async Task UpdateAsync(string id, Product product) =>
        await _collection.ReplaceOneAsync(p => p.Id == id, product);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(p => p.Id == id);
}