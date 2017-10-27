using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using ServiceAPI.Dal;
namespace ServiceAPI
{
    class Program
    {


        static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();
            
            Task restService = host.RunAsync();
            //System.Diagnostics.Process.Start("chrome.exe", "http://localhost/netcoreapp2.0/corsoing/");
            MongoDBContext m = new MongoDBContext();
            m.setupDb();
            restService.Wait();

        }
    }
}
