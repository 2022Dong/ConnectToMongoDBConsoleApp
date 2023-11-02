using MongoDataAccessLibrary.Models;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using System;
using System.Collections;

namespace MongoDataAccessLibrary.DataAccess;

public class ChoreDataAccess
{
    // const - more efficient when it comes to memory usage.
    private const string ConnectionString = "mongodb://127.0.0.1:27017"; // To test transation, need to replace to Altas connectionString here. 
    private const string DatabaseName = "choredb";
    private const string ChoreCollection = "chore_chart"; // like tables
    private const string UserCollection = "users";
    private const string ChoreHistoryCollection = "chore_history";

    private IMongoCollection<T> ConnectToMongo<T>(in string collection) // in = ref, but cannot modify it once we pass it in. More efficient.
    {
        var client = new MongoClient(ConnectionString);
        var db = client.GetDatabase(DatabaseName);
        return db.GetCollection<T>(collection);
    }

    public async Task<List<UserModel>> GetAllUsers()
    {
        var usersCollection = ConnectToMongo<UserModel>(UserCollection);
        var results = await usersCollection.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<List<ChoreModel>> GetAllChores()
    {
        var choresCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var results = await choresCollection.FindAsync(_ => true);
        return results.ToList();
    }

    // filter
    public async Task<List<ChoreModel>> GetAllChoreForAUser(UserModel user)
    {
        var chorsCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var results = await chorsCollection.FindAsync(c => c.AssignedTo.Id == user.Id);
        return results.ToList();
    }

    public Task CreateUser(UserModel user)
    {
        var usersCollection = ConnectToMongo<UserModel>(UserCollection);
        return usersCollection.InsertOneAsync(user);
    }

    public Task CreateChore(ChoreModel chore)
    {
        var choresCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        return choresCollection.InsertOneAsync(chore);
    }

    public Task UpdateChore(ChoreModel chore)
    {
        var chorsCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var filter = Builders<ChoreModel>.Filter.Eq("Id", chore.Id);
        return chorsCollection.ReplaceOneAsync(filter, chore, new ReplaceOptions { IsUpsert = true});
    }

    public Task DeleteChore(ChoreModel chore)
    {
        var chorsCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        return chorsCollection.DeleteOneAsync(c => c.Id == chore.Id);
    }

    // Fix LastComleted: null
    public async Task CompleteChore(ChoreModel chore)
    {
        var chorsCollection = ConnectToMongo<ChoreModel>(ChoreCollection);
        var filter = Builders<ChoreModel>.Filter.Eq("Id", chore.Id);
        await chorsCollection.ReplaceOneAsync(filter, chore);

        var choreHistoryCollection = ConnectToMongo<ChoreHistoryModel>(ChoreHistoryCollection);
        await choreHistoryCollection.InsertOneAsync(new ChoreHistoryModel(chore)); // ctor 2

        //// Transation  - cloud - Atlas
        ///
        // locally we just have one copy of our db, 
        // so we don't have that redundancy, so we can't do a transaction locally.

        //var client = new MongoClient(ConnectionString); // manully connect
        //using var session = await client.StartSessionAsync();

        //session.StartTransaction();

        //try
        //{
        //    var db = client.GetDatabase(DatabaseName);
        //    var choresCollection = db.GetCollection<ChoreModel>(ChoreCollection);
        //    var filter = Builders<ChoreModel>.Filter.Eq("Id", chore.Id);
        //    await choresCollection.ReplaceOneAsync(filter, chore);

        //    var choreHistoryCollection = ConnectToMongo<ChoreHistoryModel>(ChoreHistoryCollection);
        //    await choreHistoryCollection.InsertOneAsync(new ChoreHistoryModel(chore)); // ctor 2

        //    await session.CommitTransactionAsync();
        //}
        //catch (Exception ex)
        //{
        //    await session.AbortTransactionAsync();
        //    Console.WriteLine(ex.Message);
        //}
    }
}

