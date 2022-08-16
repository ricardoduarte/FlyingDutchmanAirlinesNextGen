﻿using System;
using System.Collections.Generic;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models
{
    public sealed class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; }

        public Customer(string name)
        {
            Name = name;
            Bookings = new HashSet<Booking>();
        }
    }
}
