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
                return database.GetCollection<User>("Users");
            }
        }

        public IMongoCollection<Vehicle> Vehicles
        {
            get
            {
                return database.GetCollection<Vehicle>("Vehicles");
            }
        }

    }
}
