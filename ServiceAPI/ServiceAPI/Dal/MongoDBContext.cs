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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Skip shadow types
                if (entityType.ClrType == null)
                    continue;

                entityType.Relational().TableName = entityType.ClrType.Name;
            }
            base.OnModelCreating(modelBuilder);
        }


        public void setupDb() {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var db = mongoClient.GetDatabase("concessionario");
            var collection = db.GetCollection<User>("users");
            var collection2 = db.GetCollection<Vehicle>("vehicles");
            var item = collection.Find(x => true);
            var item2 = collection2.Find(x => true);
            if (item == null)
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
                db.GetCollection<User>("Users").InsertOne(user1);
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
                db.GetCollection<User>("Users").InsertOne(user2);
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

                db.GetCollection<User>("Users").InsertOne(user3);
            }
            if (item2 == null)
            {
                Vehicle vehi1 = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    brand = "Toyota",
                    model = "StravaganteX9",
                    plate = "XY 999 AS",
                    price = 10000,
                };
                db.GetCollection<Vehicle>("Vehicles").InsertOne(vehi1);

                Vehicle vehi2 = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    brand = "BMW",
                    model = "X1",
                    plate = "XY 666 AS",
                    price = 200000,
                };
                db.GetCollection<Vehicle>("Vehicles").InsertOne(vehi2);

                Vehicle vehi3 = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    brand = "Mercedes",
                    model = "ClasseC",
                    plate = "CC 123 AS",
                    price = 50000,
                };
                db.GetCollection<Vehicle>("Vehicles").InsertOne(vehi3);
            }

        }


        public MongoDBContext()
        {

            try
            {
                var mongoClient = new MongoClient("mongodb://localhost:27017");
                database = mongoClient.GetDatabase("Concessionario");
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
