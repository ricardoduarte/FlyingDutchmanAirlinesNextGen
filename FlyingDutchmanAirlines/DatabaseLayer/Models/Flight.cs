using System;
using System.Collections.Generic;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models
{
    public sealed class Flight
    {
        public int FlightNumber { get; set; }
        public int Origin { get; set; }
        public int Destination { get; set; }

        public Airport DestinationNavigation { get; set; } = null!;
        public Airport OriginNavigation { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; }

        public Flight()
        {
            Bookings = new HashSet<Booking>();
        }
    }
}
