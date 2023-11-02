
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MongoDataAccessLibrary.Models;

public class ChoreHistoryModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string ChoreId{ get; set; }
    public string ChoreText{ get; set; }
    public DateTime DateCompleted { get; set; }
    public UserModel WhoCompleted { get; set; }

    // ctor 1
    public ChoreHistoryModel()
    {
            
    }

    // ctor 2
    public ChoreHistoryModel(ChoreModel chore)
    {
        ChoreId = chore.Id;
        DateCompleted = chore.LastCompleted ?? DateTime.Now;
        WhoCompleted = chore.AssignedTo;
        ChoreText = chore.ChoreText;

    }
}
