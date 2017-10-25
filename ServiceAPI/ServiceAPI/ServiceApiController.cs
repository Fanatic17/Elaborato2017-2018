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


namespace ServiceAPI
{

    [Route("api")]
    public class ServiceApiController : Controller
    {
        //Mongo
        //MongoClient client;

        //public ServiceApiController()
        //{
        //    //Mongo
        //client = new MongoClient("mongodb://localhost:27017");
        //SetupDatabasewMongo();
        //SetupCollectionswMongo();
        //}

        static readonly object setupLock = new object();
        static readonly SemaphoreSlim parallelism = new SemaphoreSlim(2);


        //MONGO----------------------------------------------
        //MONGO----------------------------------------------

        //public void SetupDatabasewMongo()
        //{

        //    var create_db = client.GetDatabase("Concessionario");

        //}
        //public void SetupCollectionswMongo()
        //{


        //    if (client.GetDatabase("Concessionario").GetCollection<Customer>("customers") == null)
        //    {
        //        client.GetDatabase("Concessionario").CreateCollection("customers");
        //        GetExampleListCustomer();
        //    }

        //    if (client.GetDatabase("Concessionario").GetCollection<Vehicle>("vehicles") == null)
        //    {
        //        client.GetDatabase("Concessionario").CreateCollection("vehicles");
        //        //Mettere la lista di esempio
        //    }

        //}
        //public void GetExampleListCustomer()
        //{
        //    var collection_cust = client.GetDatabase("Concessionario").GetCollection<Customer>("customers");
        //    string[] array_nomi = new string[] { "Marco", "Simone", "Emanuele", "Niccolo", "Giovanni", "Riccardo", "Antonio", "Silvio" };
        //    string[] array_cognomi = new string[] { "Consoli", "Aquino", "GIoele", "Andronaco", "Franata", "Stuppia", "Pappone", "Cerullo" };

        //    for (int i = 0; i < 7; i++)
        //    {
        //        Customer p = new Customer();
        //        p.name = array_nomi[i];
        //        p.lastName = array_cognomi[i];
        //        p.birthDate = new DateTime();
        //        p.address = array_nomi[i] + array_cognomi[i];
        //        p.ownedVehicles = "";
        //    }
        //}
        //[HttpGet("setup")]
        //public IActionResult SetupDB()
        //{
        //    var create_db = client.GetDatabase("Concessionario");
        //    if (create_db != null) return Ok();
        //    else return null;
        //}
        //[HttpGet("customers")]
        //public IMongoCollection<Customer> GetAllCustomers()
        //{
        //    var collections = client.GetDatabase("Concessionario").GetCollection<Customer>("customer");

        //    return collections;
        //}
        //[HttpGet("vehicles")]
        //public IMongoCollection<Vehicle> GetAllVehicles()
        //{
        //    var collections = client.GetDatabase("Concessionario").GetCollection<Vehicle>("vehicle");
        //    return collections;
        //}

        //------------------------------------------------------------------
        //------------------------------------------------------------------


        //MYSQL MYSQL MYSQL

        [HttpGet("setup")]
        public IActionResult SetupDatabase()
        {
            lock (setupLock)
            {
                using (var context = new UsersDbContext())
                {
                    // Create database
                    context.Database.EnsureCreated();
                }
                return Ok("database created");
            }
        }



        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                await parallelism.WaitAsync();

                using (var context = new UsersDbContext())
                {
                    return Ok(await context.Users.ToListAsync());
                }
            }
            finally
            {
                parallelism.Release();
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser([FromQuery]int id)
        {
            using (var context = new UsersDbContext())
            {
                return Ok(await context.Users.FirstOrDefaultAsync(x => x.Id == id));
            }
        }

        [HttpGet("userwname")]
        public async Task<IActionResult> GetUserwName([FromQuery]string name)
        {
            using (var context = new UsersDbContext())
            {
                return Ok(await context.Users.FirstOrDefaultAsync(x =>x.name==name));
            }
        }

        [HttpGet("userwsurname")]
        public async Task<IActionResult> GetUserwSurname([FromQuery]string surname)
        {
            using (var context = new UsersDbContext())
            {
                return Ok(await context.Users.FirstOrDefaultAsync(x => x.lastName == surname));
            }
        }

        [HttpPut("user")]
        public async Task<IActionResult> CreateUser([FromBody]User user)
        {
            using (var context = new UsersDbContext())
            {
                context.Users.Add(user);

                await context.SaveChangesAsync();

                return Ok();
            }
        }

        [HttpPost("user")]
        public async Task<IActionResult> UpdateUser([FromBody]User user)
        {
            using (var context = new UsersDbContext())
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return Ok();
            }
        }


        [HttpDelete("user")]
        public async Task<IActionResult> DeleteUser([FromQuery]int id)
        {
            using (var context = new UsersDbContext())
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok();


            }
        }
    }


}
