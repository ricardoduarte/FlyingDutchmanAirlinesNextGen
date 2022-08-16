using System;
using System.Collections.Generic;

namespace FlyingDutchmanAirlines.DatabaseLayer.Models
{
    public sealed class Airport
    {
        public int AirportId { get; set; }
        public string City { get; set; } = null!;
        public string Iata { get; set; } = null!;

        public ICollection<Flight> FlightDestinationNavigations { get; set; }
        public ICollection<Flight> FlightOriginNavigations { get; set; }

        public Airport()
        {
            FlightDestinationNavigations = new HashSet<Flight>();
            FlightOriginNavigations = new HashSet<Flight>();
        }
    }
}
