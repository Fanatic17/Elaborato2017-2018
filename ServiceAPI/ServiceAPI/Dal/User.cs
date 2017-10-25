using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceAPI.Dal
{
    public class User
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string lastName { get; set; }
        public DateTime birthDate { get; set; }
        public string address { get; set; }
        public List<Vehicle> ownedVehicles { get; set; }
        public bool isAdmin { get; set; }
    }
}
