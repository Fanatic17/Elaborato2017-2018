using Microsoft.AspNetCore.Mvc;
using ServiceAPI.Dal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using MongoDB.Driver;
using MongoDB.Bson;

using MongoDB.Bson;

namespace ServiceAPI
{

    [Route("api")]
    public class ServiceApiControllerwMongo : Controller
    {
        MongoClient client;

        public ServiceApiControllerwMongo()
        {
            client = new MongoClient("mongodb://localhost:27017");
            SetupDatabase();
            SetupCollections();
        }

        static readonly object setupLock = new object();
        static readonly SemaphoreSlim parallelism = new SemaphoreSlim(2);


        //Setup the mongoDB Database
        public void SetupDatabase()
        {

            var create_db = client.GetDatabase("Concessionario");

        }

        public void SetupCollections()
        {


            if (client.GetDatabase("Concessionario").GetCollection<Customer>("customers") == null)
            {
                client.GetDatabase("Concessionario").CreateCollection("customers");
                GetExampleListCustomer();
            }
            
            if (client.GetDatabase("Concessionario").GetCollection<Vehicle>("vehicles") == null)
            {
                client.GetDatabase("Concessionario").CreateCollection("vehicles");
                //Mettere la lista di esempio
            }

        }

        public void GetExampleListCustomer()
        {
            var collection_cust = client.GetDatabase("Concessionario").GetCollection<Customer>("customers");
            string[] array_nomi = new string[] { "Marco", "Simone", "Emanuele", "Niccolo", "Giovanni", "Riccardo", "Antonio", "Silvio" };
            string[] array_cognomi = new string[] { "Consoli", "Aquino", "GIoele", "Andronaco", "Franata", "Stuppia", "Pappone", "Cerullo" };

            for (int i = 0; i < 7; i++)
            {
                Customer p = new Customer();
                p.name = array_nomi[i];
                p.lastName = array_cognomi[i];
                p.birthDate = new DateTime();
                p.address = array_nomi[i] + array_cognomi[i];
                p.ownedVehicles = "";
            }
        }



        [HttpGet("setup")]
        public IActionResult SetupDB()
        {
            var create_db = client.GetDatabase("Concessionario");
            if (create_db != null) return Ok();
            else return null;
        }

        //Get all customers from db
        [HttpGet("customers")]
        public IMongoCollection<Customer> GetAllCustomers()
        {
            var collections = client.GetDatabase("Concessionario").GetCollection<Customer>("customer");

            return collections;
        }

        [HttpGet("vehicles")]
        public IMongoCollection<Vehicle> GetAllVehicles()
        {
            var collections = client.GetDatabase("Concessionario").GetCollection<Vehicle>("vehicle");
            return collections;
        }

        //[HttpPost("customers")]

        //[HttpPost("vehicles")]


    }
}