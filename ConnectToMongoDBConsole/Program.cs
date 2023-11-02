using MongoDB.Driver;
using ConnectToMongoDBConsole;  //PersonModel.cs - nameSpace
using MongoDataAccessLibrary.DataAccess;
using MongoDataAccessLibrary.Models;

//string connectionString = "mongodb://127.0.0.1:27017";
//string databaseName = "simple_db";
//string collectionName = "people";

//var client = new MongoClient(connectionString);
//var db =  client.GetDatabase(databaseName);
//var collection = db.GetCollection<PersonModel>(collectionName); // similar to table

//var person = new PersonModel { FirstName = "Dongyun", LastName = "Huang" };

//await collection.InsertOneAsync(person);

//var results = await collection.FindAsync(_ => true); // return every records

//foreach (var result in results.ToList())
//{
//    Console.WriteLine($"{result.Id}: {result.FirstName} {result.LastName}");
//}

ChoreDataAccess db = new ChoreDataAccess();

await db.CreateUser(new UserModel() { FirstName = "Dong", LastName = "Huang" }); // await

var users = await db.GetAllUsers();

var chore = new ChoreModel() 
{ 
    AssignedTo = users.First(), 
    ChoreText = "Mow the lawn", 
    FrequencyInDays = 7 
};

await db.CreateChore(chore);


//await db.CompleteChore(chore);
var chores = await db.GetAllChores();

var newChore = chores.First();
newChore.LastCompleted = DateTime.UtcNow;

await db.CompleteChore(newChore);