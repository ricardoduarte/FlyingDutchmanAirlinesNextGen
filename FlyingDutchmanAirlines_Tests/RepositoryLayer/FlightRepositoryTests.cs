using Microsoft.EntityFrameworkCore;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines_Tests.Stubs;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer;

[TestClass]
public class FlightRepositoryTests {
    private FlyingDutchmanAirlinesContext _context = default!;
    private FlightRepository _repository = default!;

    [TestInitialize]
    public async Task TestInitialize()
    {
        DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman").Options;
        _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);
        _repository = new FlightRepository(_context);
        Assert.IsNotNull(_repository);

        Flight flight = new Flight
        {
            FlightNumber = 1,
            Origin = 1,
            Destination = 2
        };

        Flight flight2 = new Flight {
            FlightNumber = 10,
            Origin = 3,
            Destination = 4
        };

        _context.Flights.Add(flight);
        _context.Flights.Add(flight2);
        await _context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task GetFlightByFlightNumber_Success() {
        Flight flight = await _repository.GetFlightByFlightNumber(1);
        Assert.IsNotNull(flight);

        Flight dbFlight = _context.Flights.First(f => f.FlightNumber == 1);
        Assert.IsNotNull(flight);

        Assert.AreEqual(dbFlight.FlightNumber, flight.FlightNumber);
        Assert.AreEqual(dbFlight.Origin, flight.Origin);
        Assert.AreEqual(dbFlight.Destination, flight.Destination);
    }

    [TestMethod]
    [ExpectedException(typeof(FlightNotFoundException))]
    public async Task GetFlightByFlightNumber_Failure_InvalidFlightNumber() {
        await _repository.GetFlightByFlightNumber(-1);
    }

    [TestMethod]
    [ExpectedException(typeof(FlightNotFoundException))]
    public async Task GetFlightByFlightNumber_Failure_DatabaseException() {
        await _repository.GetFlightByFlightNumber(2);
    }

    [TestMethod]
    public void GetFlights_Success()
    {
        IEnumerable<Flight> flights = _repository.GetFlights();
        Assert.AreEqual(flights.Count(), 2);
    }
}