using Amazon.Runtime.SharedInterfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace testconsole
{
    internal class Program
    {
        static MongoClient client = new MongoClient("mongodb://localhost:27017");
        static IMongoDatabase database = client.GetDatabase("test2");
        static IMongoCollection<BsonDocument> districts = database.GetCollection<BsonDocument>("districts");
        static IMongoCollection<BsonDocument> candidates = database.GetCollection<BsonDocument>("candidates ");
        static IMongoCollection<BsonDocument> voting = database.GetCollection<BsonDocument>("voting ");
        static IMongoCollection<BsonDocument> bulletin = database.GetCollection<BsonDocument>("bulletin ");

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            //var filter = Builders<BsonDocument>.Filter.Gte("_id", 1) & Builders<BsonDocument>.Filter.Lte("_id", 5);

            Console.WriteLine("read(0), write(1),del(2)");
            int nom = int.Parse(Console.ReadLine());
            if (nom == 0) 
            {
                Read();
            }
            else if (nom == 1)
            {
                Add();
                Read();
            }
            else if (nom==2)
            {
                Dell();
                Read();
            }
            else if (nom == 3)
            {
                var voters = database.GetCollection<BsonDocument>("voters");
                var filter = Builders<BsonDocument>.Filter.Gte("_id", 1) & Builders<BsonDocument>.Filter.Lte("_id", 5);
                var documents = voters.Find(filter).ToList(); // Виконання запиту та отримання результатів у вигляді списку документів

                foreach (var document in documents)
                {
                    Console.WriteLine(document.ToJson()); // Вивід результатів
                }
            }

        }
        static public void Add()
        {
            User newUser = new User();
            Console.WriteLine("print id");
            newUser._id= int.Parse(Console.ReadLine());
            Console.WriteLine("first_name");
            newUser.first_name= Console.ReadLine();
            Console.WriteLine("last_name");
            newUser.last_name = Console.ReadLine();
            Console.WriteLine("patronymic");
            newUser.patronymic = Console.ReadLine();
            Console.WriteLine("id_districts");
            newUser.id_districts =int.Parse(Console.ReadLine());
            Console.WriteLine("date_of_birth dd mm yyyy");
            string[] line = Console.ReadLine().Trim().Split(" ",StringSplitOptions.RemoveEmptyEntries) ;
            int day =int.Parse(line[0]);
            int month =int.Parse(line[1]);
            int year =int.Parse(line[2]);
            newUser.date_of_birth = new DateTime(year, month, day);
            Console.WriteLine("email");
            newUser.email = Console.ReadLine();
            Console.WriteLine("password");
            newUser.password = Console.ReadLine();
            var voters = database.GetCollection<User>("voters");
            while (true)
            {
                try
                {
                    voters.InsertOne(newUser);
                    break;
                }
                catch (MongoWriteException ex)
                {
                    if (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
                    {
                        Console.WriteLine("Помилка: _id вже існує в колекції.");
                    }
                    else
                    {
                        Console.WriteLine("Інша помилка при вставці документу.");
                    }
                }
                Console.WriteLine("input new id");
                newUser._id = int.Parse(Console.ReadLine());
            }
        }
    static public void Dell()
        {
            var voters = database.GetCollection<BsonDocument>("voters");
            Console.WriteLine("input id");
            int id =int.Parse(Console.ReadLine());
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id); 
            var result = voters.DeleteOne(filter);
            if (result.DeletedCount > 0)
            {
                Console.WriteLine("Документ був успішно видалений.");
            }
            else
            {
                Console.WriteLine("Документ не був знайдений або видалений.");
            }
        }
        static public void Read()
        {
            var voters = database.GetCollection<User>("voters");
           /* var filter = new BsonDocument("$and", new BsonArray { new BsonDocument("_id", new BsonDocument("$gte", 1)),
               new BsonDocument("_id", new BsonDocument("$lte", 5)) });*/
            var filter1 = new BsonDocument(new BsonDocument());
            List<User> people = voters.Find(filter1).ToList();
            foreach (User doc in people)
            {
                Console.WriteLine("{0} {1} {2} {3} {4} {5}",
                doc._id, doc.first_name, doc.last_name, doc.patronymic, doc.id_districts, doc.date_of_birth);
            }
        }
    }

    public class User
    { 
        public int _id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string patronymic { get; set; }
        public int id_districts { get; set; }
        public DateTime date_of_birth { get; set; }
        public string email { get; set; }
        public string password { get; set; }

    }
    /*MongoDB.Driver.FilterDefinition<BsonDocument>[] filters = {
         Builders<BsonDocument>.Filter.Gte("id_voters", 1) & Builders<BsonDocument>.Filter.Lte("id_voters", 5),
         Builders<BsonDocument>.Filter.In("id_districts", new List<int> { 1, 3, 5 }),
         Builders<BsonDocument>.Filter.Regex("name", new BsonRegularExpression(".*2023.*")) };*/
}



