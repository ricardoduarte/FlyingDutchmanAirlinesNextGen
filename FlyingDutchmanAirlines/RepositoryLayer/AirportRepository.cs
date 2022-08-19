using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class AirportRepository
{
    private readonly FlyingDutchmanAirlinesContext _context;

    public AirportRepository(FlyingDutchmanAirlinesContext _context)
    {
        this._context = _context;
    }

    public async Task<Airport> GetAirportById(int airportId)
    {
        if (!airportId.IsPositive())
        {
            Console.WriteLine($"Argument exception in GetAirportById! AirportID = {airportId}");
            throw new ArgumentException("Invalid argument provided");
        }
        return await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == airportId) ?? throw new AirportNotFoundException();
    }
}