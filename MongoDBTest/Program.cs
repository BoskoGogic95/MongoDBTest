using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDBTest.DataAccess;
using MongoDBTest.DataUserModel;

// See https://aka.ms/new-console-template for more information
string connectionString = "mongodb://127.0.0.1:27017"; //conn string for mongo db base
string databaseName = "simple_db";
string collectionName = "people";

var client = new MongoClient(connectionString);
var db = client.GetDatabase(databaseName);
var collection = db.GetCollection<PersonModel>(collectionName);

var person = new PersonModel
{
    FirstName = "Bosko",
    LastName = "Gogic"
};

await collection.InsertOneAsync(person);

var results = await collection.FindAsync(_ => true); // we will return every record

foreach (var result in results.ToList())
{
    Console.WriteLine(value: $"{result.Id}: {result.FirstName}: {result.LastName}");
}

DataAccessTemplate dat = new DataAccessTemplate();
string Email = "testCall@boing.rs";
//dat.UploadFileFromAStream(Email);
await dat.DownloadFileToStream();
