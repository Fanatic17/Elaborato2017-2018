using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceAPI.Dal
{
    public class Customer
    {
        public string name { get; set; }
        public string lastName { get; set; }
        public DateTime birthDate { get; set; }
        public string address { get; set; }
        public string ownedVehicles { get; set; }

    }
}
