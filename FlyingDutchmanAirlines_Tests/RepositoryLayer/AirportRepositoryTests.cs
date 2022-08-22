using Microsoft.EntityFrameworkCore;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines_Tests.Stubs;
using System.Collections.Generic;

namespace FlyingDutchmanAirlines_Tests.RepositoryLayer;

[TestClass]
public class AirportRepositoryTests {
    private FlyingDutchmanAirlinesContext _context = default!;
    private AirportRepository _repository = default!;

    [TestInitialize]
    public async Task TestInitialize()
    {
        DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman").Options;
        _context = new FlyingDutchmanAirlinesContext_Stub(dbContextOptions);
        _repository = new AirportRepository(_context);
        Assert.IsNotNull(_repository);
        
        SortedList<string, Airport> airports = new SortedList<string, Airport>
        {
            {
                "GOH",
                new Airport
                {
                    AirportId = 0,
                    City = "Nuuk",
                    Iata = "GOH"
                }
            },
            {
                "PHX",
                new Airport
                {
                    AirportId = 1,
                    City = "Phoenix",
                    Iata = "PHX"
                }
            },
            {
                "DDH",
                new Airport
                {
                    AirportId = 2,
                    City = "Bennington",
                    Iata = "DDH"
                }
            },
            {
                "RDU",
                new Airport
                {
                    AirportId = 3,
                    City = "Raleigh-Durham",
                    Iata = "RDU"
                }
            }
        };

        _context.Airports.AddRange(airports.Values);
        await _context.SaveChangesAsync();
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public async Task GetAirportById_Success(int airportId) {
        Airport airport = await _repository.GetAirportById(airportId);
        Assert.IsNotNull(airport);

        Airport dbAirport = _context.Airports.First(a => a.AirportId == airportId);
        Assert.AreEqual(dbAirport.AirportId, airport.AirportId);
        Assert.AreEqual(dbAirport.City, airport.City);
        Assert.AreEqual(dbAirport.Iata, airport.Iata);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task GetAirportById_Failure_InvalidInput() {
        StringWriter outputStream = new StringWriter();
        try {
            Console.SetOut(outputStream);
            await _repository.GetAirportById(-1);
        }
        catch (ArgumentException) {
            Assert.IsTrue(outputStream.ToString().Contains("Argument exception in GetAirportById! AirportID = -1"));
            throw new ArgumentException();
        }
        finally {
            outputStream.Dispose();
        }
        
    }

    [TestMethod]
    [ExpectedException(typeof(AirportNotFoundException))]
    public async Task GetAirportById_Failure_DatabaseException()
    {
        await _repository.GetAirportById(10);
    }
}