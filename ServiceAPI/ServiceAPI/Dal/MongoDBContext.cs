using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceAPI.Dal
{
    public class MongoDBContext : DbContext
    {
        private IMongoDatabase database { get; }




        public MongoDBContext()
        {

            try
            {
                
                var mongoClient = new MongoClient("mongodb://localhost:27017");
                database = mongoClient.GetDatabase("Concessionario");
                var collection = database.GetCollection<User>("users");
                var collection2 = database.GetCollection<Vehicle>("vehicles");
                var res = collection.Find(x => x.Id != null).FirstOrDefault();
                var res2 = collection2.Find(x => x.Id != null).FirstOrDefault();
                if (res == null)
                {
                    User user1 = new User
                    {
                        Id = Guid.NewGuid(),
                        name = "Guido",
                        lastName = "Dieganza",
                        address = "non sadsda",
                        ownedVehicles = null,
                        birthDate = new DateTime(),
                        isAdmin = false,

                    };
                    System.Console.WriteLine("Il nome dell'utente e: {0}", user1.name);
                    collection.InsertOne(user1);
                    User user2 = new User
                    {
                        Id = Guid.NewGuid(),
                        name = "Nicco",
                        lastName = "consoli",
                        address = "nonasd asd",
                        ownedVehicles = null,
                        birthDate = new DateTime(),
                        isAdmin = true,

                    };
                    collection.InsertOne(user2);
                    User user3 = new User
                    {
                        Id = Guid.NewGuid(),
                        name = "GIovanni",
                        lastName = "Daqunio",
                        address = "asd",
                        ownedVehicles = null,
                        birthDate = new DateTime(),
                        isAdmin = true,

                    };

                    collection.InsertOne(user3);
                }
                else System.Console.WriteLine("Database utenti gia popolato!\n");
                if (res2 == null)
                {
                    Vehicle vehi1 = new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        brand = "Toyotsa",
                        model = "StravaganteX9",
                        plate = "XY 999 AS",
                        price = 10000,
                    };
                    collection2.InsertOne(vehi1);

                    Vehicle vehi2 = new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        brand = "BMW",
                        model = "X1",
                        plate = "XY 666 AS",
                        price = 200000,
                    };
                    collection2.InsertOne(vehi2);

                    Vehicle vehi3 = new Vehicle
                    {
                        Id = Guid.NewGuid(),
                        brand = "Mercedes",
                        model = "ClasseC",
                        plate = "CC 123 AS",
                        price = 50000,
                    };
                    collection2.InsertOne(vehi3);
                }
                else System.Console.WriteLine("Database veicoli gia popolato!\n");
            }
            catch (Exception e)
            {
                throw new Exception("Non posso accedere al server.", e);
            }
        }

        public IMongoCollection<User> Users
        {
            get
            {
                return database.GetCollection<User>("users");
            }

        }

        public IMongoCollection<Vehicle> Vehicles
        {
            get
            {
                return database.GetCollection<Vehicle>("vehicles");
            }

        }

    }
}
