using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Cse325GroupProject.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("size")]
    public string Size { get; set; } = string.Empty;

    [BsonElement("quantity")]
    public int Quantity { get; set; }

    [BsonElement("location")]
    public string Location { get; set; } = string.Empty;

    [BsonElement("providerName")]
    public string ProviderName { get; set; } = string.Empty;

    [BsonElement("expirationDate")]
    public DateTime? ExpirationDate { get; set; }

}