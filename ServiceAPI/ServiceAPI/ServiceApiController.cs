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
        static readonly object setupLock = new object();
        static readonly SemaphoreSlim parallelism = new SemaphoreSlim(2);
        

        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //--------------------MONGO APIs------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        [HttpGet("setupmongo")]
        public IActionResult SetupDBMongo()
        {
            lock (setupLock)
            {
                using (var context = new MongoDBContext())
                {
                    // Create database
                    context.Database.EnsureCreated();                  
                }
                return Ok("database created");
            }

        }

        //Get all Users in mongoDB
        [HttpGet("mongo/users")]
        public IActionResult GetAllUsers()
        {
            MongoDBContext dBContext = new MongoDBContext();

                List<User> userList = dBContext.Users.Find(x => true).ToList();

            return View(userList);
        }

       
        //Get total amount per specified user.
        [HttpGet("user/totalamount")]
        public IActionResult GetTotalAmountForaSpecifiedUser([FromQuery] string name,[FromQuery] string lastname) {
            int somma = 0;
            MongoDBContext dBContext = new MongoDBContext();
            var entity = dBContext.Users.Find(m => m.name == name && m.lastName==lastname).FirstOrDefault();
            foreach (var item in entity.ownedVehicles.Where(c => c.price != 0)) {
                somma = somma + item.price;
            }
            return View(somma);

        }

       
        //Put a new user.
        [HttpPut("mongo/user")]
        public  IActionResult InsertUserMongo(User entity)
        {
            MongoDBContext dbContext = new MongoDBContext();
            entity.Id = Guid.NewGuid();
            dbContext.Users.InsertOne(entity);
            dbContext.SaveChanges();
            return View(entity);
        }


        //Delete user w Mongo
        [HttpDelete("user")]
        public IActionResult DeleteUser(Guid id)
        {
            MongoDBContext dbContext = new MongoDBContext();
            dbContext.Users.DeleteOne(m => m.Id == id);
            return Redirect("/");
        }

        //Admins
        [HttpGet("user/admins")]
        public IActionResult GetAdmins()
        {
            MongoDBContext dBcontext = new MongoDBContext();
            var lista = dBcontext.Users.Find(x => x.isAdmin == true).ToList();
            return View(lista);
            
        }


        //Normal Users
        [HttpGet("user/normalusers")]
        public IActionResult GetNormalUsers()
        {
            MongoDBContext dBcontext = new MongoDBContext();
            var lista = dBcontext.Users.Find(x => x.isAdmin == false).ToList();
            return View(lista);

        }


        //ModifyUser w Mongo
        [HttpPost("mongo/user")]
        public IActionResult ModifyUser(User user) {
            MongoDBContext dBContext = new MongoDBContext();
            dBContext.Users.ReplaceOne(m => m.Id == user.Id, user);
            return View(user);
        }

        //Find users that have already 1 or more vehicles
        [HttpGet("user/withmanyvehicles")]
        public IActionResult FindUsersWithVehicles() {
            List<User> u = new List<User>();
            MongoDBContext dBContext = new MongoDBContext();
            var item = dBContext.Users.Find(x => x.ownedVehicles.Count > 1).ToList();
            return View(item);
         }


        //Get all vehicles.
        [HttpGet("mongo/vehicles")]
        public IActionResult GetAllVehicles()
        {
            MongoDBContext dBContext = new MongoDBContext();

            List<Vehicle> vehicleList = dBContext.Vehicles.Find(x => true).ToList();

            return View(vehicleList);
        }


        //Put a new vehicle.
        [HttpPut("mongo/vehicle")]
        public IActionResult InsertNewVehicle(Vehicle entity)
        {
            MongoDBContext dbContext = new MongoDBContext();
            entity.Id = Guid.NewGuid();
            dbContext.Vehicles.InsertOne(entity);
            dbContext.SaveChanges();
            return View(entity);
        }


        //Get all Vehicles in mongoDB
        [HttpGet("mongo/vehicles")]
        public IActionResult GetAllVehiclesMongo()
        {
            MongoDBContext dBContext = new MongoDBContext();

            List<Vehicle> vehicleList = dBContext.Vehicles.Find(x => true).ToList();

            return View(vehicleList);
        }

        //Modify Vehicle w Mongo
        [HttpPost("mongo/vehicle")]
        public IActionResult ModifyVehicle(Vehicle vehicle)
        {
            MongoDBContext dBContext = new MongoDBContext();
            dBContext.Vehicles.ReplaceOne(m => m.Id == vehicle.Id, vehicle);
            return View(vehicle);
        }


        //Delete Vehicle w Mongo
        [HttpDelete("vehicle")]
        public IActionResult Delete(Guid id)
        {
            MongoDBContext dbContext = new MongoDBContext();
            dbContext.Vehicles.DeleteOne(m => m.Id == id);
            return Redirect("/");
        }

        //Find all users trough the price of their ownedvehicles.
        [HttpGet("")]
        public IActionResult FindUsersWithPrice([FromQuery] int price, int intorno)
        {
            int limite_inferiore = price - intorno;
            int limite_superiore = price + intorno;
            List<User> u = new List<User>();
            MongoDBContext dBContext = new MongoDBContext();

            foreach (var find_user in dBContext.Users.AsQueryable().
                                       Where(p => p.ownedVehicles.Any(c => c.price >= limite_inferiore 
                                       && c.price <= limite_superiore)))
            {
                u.Add(find_user);
            }

            return View(u);

        }

        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //---------------------------MySQL APIs-------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------
        //------------------------------------------------------------------




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
        public async Task<IActionResult> GetUser([FromQuery]Guid id)
        {
            using (var context = new UsersDbContext())
            {
                return Ok(await context.Users.FirstOrDefaultAsync(x => x.Id==id));
            }
        }

        [HttpGet("userwname")]
        public async Task<IActionResult> GetUserwName([FromQuery]string name)
        {
            using (var context = new UsersDbContext())
            {
                return Ok(await context.Users.FirstOrDefaultAsync(x => x.name == name));
            }
        }

        [HttpGet("admins")]
        public List<User> GetAdminsSql()
        {
            using (var context = new UsersDbContext())
            {
                var lista = context.Users.Where(x => x.isAdmin == true).ToList();
                if (lista != null) return lista;
                else return null;
            }
        }

        [HttpGet("normalUsers")]
        public List<User> GetNormalUsersSql()
        {
            using (var context = new UsersDbContext())
            {
                var lista = context.Users.Where(x => x.isAdmin == false).ToList();
                if (lista != null) return lista;
                else return null;
            }
        }
        //Get Most spendaccioni users.
        [HttpGet("getmostownedvehicles")]
        public List<User> GetMostPopularVehicles([FromQuery] string brand, int dimensione)
        {
            using (var context = new UsersDbContext()) {
                var Lista_utenti =context.Users.Where(x => (x.ownedVehicles.Count > dimensione)).ToList();
                if (Lista_utenti != null)
                    return Lista_utenti;
                else
                    return null;

            }
        }
        //Get all vehicle with a specified brand
        [HttpGet("GetBrandVehicles")]
        public List<Vehicle> GetBrandVehicles([FromQuery] string brand) {
            using (var context = new UsersDbContext()) {
                 var lista = context.Vehicles.Where(x => x.brand == brand).ToList();
                return lista;
                
            }


        }

        //Get all near prices about a main price
        [HttpGet("nearestprices")]
        public List<Vehicle> GetPriceVehicles([FromQuery]int price,int intorno) {
                using(var context=new UsersDbContext())
            {
                //intorno di 2000.
                int limite_superiore = price + intorno;
                int limite_inferiore = price - intorno;
                var list=context.Vehicles.Where(
                    x => x.price >= limite_inferiore && x.price <= limite_superiore)
                    .ToList();
                if (list != null) return list;
                else return null;
            }
        }

        //Find user from his sunrmae
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

        [HttpPut("vehicle")]
        public async Task<IActionResult> CreateVehicle([FromBody]Vehicle vehicle)
        {
            using (var context = new UsersDbContext())
            {
                context.Vehicles.Add(vehicle);

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

        [HttpPost("vehicle")]
        public async Task<IActionResult> UpdateVehicle([FromBody]Vehicle vehicle)
        {
            using (var context = new UsersDbContext())
            {
                context.Vehicles.Update(vehicle);
                await context.SaveChangesAsync();
                return Ok();
            }
        }

        [HttpDelete("user")]
        public async Task<IActionResult> DeleteUserSql([FromQuery]Guid id)
        {
            using (var context = new UsersDbContext())
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
                context.Users.Remove(user);
                await context.SaveChangesAsync();
                return Ok();


            }
        }

        [HttpDelete("vehicle")]
        public async Task<IActionResult> DeleteVehicle([FromQuery]Guid id)
        {
            using (var context = new UsersDbContext())
            {
                var vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
                context.Vehicles.Remove(vehicle);
                await context.SaveChangesAsync();
                return Ok();


            }
        }
    }


}
