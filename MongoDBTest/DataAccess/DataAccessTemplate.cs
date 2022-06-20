using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDBTest.DataUserModel;
using System.Text;

namespace MongoDBTest.DataAccess
{
    public class DataAccessTemplate
    {
        private readonly string ConnectionString = "mongodb://127.0.0.1:27017"; //conn string for mongo db base
        private readonly string DatabaseName = "file_storage_db";
        private readonly string UserCollection = "users";

        //from freeCodeCamp
        private readonly IMongoCollection<UserModel> dbCollection;

        private readonly FilterDefinitionBuilder<UserModel> filterBuilder = Builders<UserModel>.Filter;

        public DataAccessTemplate()
        {
        }

        private IMongoCollection<T> ConnectToMongo<T>(in string collection)
        {
            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DatabaseName);
            return db.GetCollection<T>(collection);
        }

        /*
        *   Zanimljivo zapazanje -> Kada je metoda static i pokusa se pristupiti readonly parametru, kompajler ce prijaviti gresku
        *   Provjeriti zbog cega je to tako !
        */

        public void UploadFileFromAStream(string Email)
        {
            //var database = DatabaseHelper.GetDatabaseReference("localhost", "file_storage_db");

            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DatabaseName);

            // Create a FileStream Object
            // to write to a text file
            // The parameters are complete
            // path of the text file in the
            // system, in Create mode, the
            // access to this process is
            // Write and for other
            // processes is None
            FileStream fWrite = new FileStream(@"C:\Users\Administrator\Documents\TestCall.txt",
                         FileMode.Create, FileAccess.Write);

            // Store the text in the variable text
            var text = "This is some text written to the textfile " +
                           "named Textfile using FileStream class." + "and added some additional text." + "this is is new word file for testing";

            // Store the text in a byte array with
            // UTF8 encoding (8-bit Unicode
            // Transformation Format)
            byte[] writeArr = Encoding.UTF8.GetBytes(text);

            // Using the Write method write
            // the encoded byte array to
            // the textfile
            fWrite.Write(writeArr, 0, text.Length);

            // Closee the FileStream object
            fWrite.Close();

            IGridFSBucket bucket = new GridFSBucket(db);
            Stream stream = File.Open(@"C:\Users\Administrator\Documents\TestCall.txt", FileMode.Open);

            var options = new GridFSUploadOptions()
            {
                Metadata = new BsonDocument()
                {
                 {"author", Email},

                 {"time", DateTime.Now}
                 }
            };

            ObjectId id = bucket.UploadFromStream(@"C:\Users\Administrator\Documents\TestCall.txt", stream, options);

            Console.WriteLine(id.ToString() + " whatever");
            //Console.WriteLine()
        }

        public async Task DownloadFileToStream()
        {
            var client = new MongoClient(ConnectionString);
            var db = client.GetDatabase(DatabaseName);

            IGridFSBucket bucket = new GridFSBucket(db);

            // filter the db and find the file which is specified by that filter, you can pass ID or something else instead filename.
            var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(x => x.Filename, @"C:\Users\Administrator\Documents\TestCall.txt");

            var searchResult = await bucket.FindAsync(filter);

            var fileEntry = searchResult.FirstOrDefault();

            var file = @"C:\Users\Administrator\Documents\TestCall.txt";

            using (Stream fs = new FileStream(file, FileMode.CreateNew, FileAccess.Write))
            {
                await bucket.DownloadToStreamAsync(fileEntry.Id, fs);
                fs.Close();
            }
        }

        // Some generic methods
        public async Task<IReadOnlyCollection<UserModel>> GetAllAsync()
        {
            var dbCollection = ConnectToMongo<UserModel>(UserCollection);
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        //public async Task<UserModel> GetAsync(Guid id)
        //{
        //   // FilterDefinition<UserModel> filter = filterBuilder.Eq(entity => entity.Id, id);
        //    return await dbCollection.Find(filter).FirstOrDefaultAsync();
        //}

        public async Task CreateAsync(UserModel entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }
    }
}