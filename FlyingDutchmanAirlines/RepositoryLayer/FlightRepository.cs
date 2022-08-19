using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class FlightRepository
{
    private readonly FlyingDutchmanAirlinesContext _context;

    public FlightRepository(FlyingDutchmanAirlinesContext _context)
    {
        this._context = _context;
    }

    public async Task<Flight> GetFlightByFlightNumber(int flightNumber, int originAirportId, int destinationAirportId)
    {
        if (!flightNumber.IsPositive())
        {
            Console.WriteLine($"Could not find flight in GetFlightByFlightNumber! FlightNumber = {flightNumber}");
            throw new FlightNotFoundException();
        }
        if (!originAirportId.IsPositive() || !destinationAirportId.IsPositive())
        {
            Console.WriteLine($"Argument Exception in GetFlightByFlightNumber! OriginAirportID = {originAirportId}, DestinationAirportID = {destinationAirportId}");
            throw new ArgumentException("Invalid arguments provided"); 
        }
        return await _context.Flights.FirstOrDefaultAsync(f => f.FlightNumber == flightNumber) ?? throw new FlightNotFoundException();
    }
}