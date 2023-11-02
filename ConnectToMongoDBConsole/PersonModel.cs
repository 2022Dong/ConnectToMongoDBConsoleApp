
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConnectToMongoDBConsole;

public class PersonModel
{
    [BsonId]// add 2 decorators
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

}
