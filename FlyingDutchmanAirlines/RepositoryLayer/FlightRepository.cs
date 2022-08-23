using System.Reflection;
using System.Runtime.CompilerServices;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class FlightRepository
{
    private readonly FlyingDutchmanAirlinesContext _context;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public FlightRepository()
    {
        if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
        {
            throw new Exception("This constructor should only be used for testing");
        }
    }

    public FlightRepository(FlyingDutchmanAirlinesContext _context)
    {
        this._context = _context;
    }

    public virtual async Task<Flight> GetFlightByFlightNumber(int flightNumber)
    {
        if (!flightNumber.IsPositive())
        {
            Console.WriteLine($"Could not find flight in GetFlightByFlightNumber! FlightNumber = {flightNumber}");
            throw new FlightNotFoundException();
        }
        return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber) ?? throw new FlightNotFoundException();
    }

    public virtual IEnumerable<Flight> GetFlights()
    {
        foreach(Flight flight in _context.Flights)
        {
           yield return flight;
        }
    }
}