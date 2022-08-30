using System.Runtime.ExceptionServices;
using System.Runtime.CompilerServices;
using System.Reflection;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.Views;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class FlightService
{
    
    private readonly FlightRepository _flightRepository = default!;
    private readonly AirportRepository _airportRepository = default!;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public FlightService()
    {
        if (Assembly.GetExecutingAssembly().FullName == Assembly.GetCallingAssembly().FullName)
        {
            throw new Exception("This constructor should only be used for testing");
        }
    }

    public FlightService(FlightRepository flightRepository, AirportRepository airportRepository)
    {
        _flightRepository = flightRepository;
        _airportRepository = airportRepository;
    }

    public virtual async IAsyncEnumerable<FlightView> GetFlights()
    {
        IEnumerable<Flight> flights = _flightRepository.GetFlights();
        foreach (Flight flight in flights)
        {
            Airport originAirport;
            Airport destinationAirport;
            try
            {
                originAirport = await _airportRepository.GetAirportById(flight.Origin);
                destinationAirport = await _airportRepository.GetAirportById(flight.Destination);
            }
            catch (FlightNotFoundException)
            {
                throw new FlightNotFoundException();
            }
            catch (Exception)
            {
                throw new ArgumentException();
            }
            yield return new FlightView(
                flight.FlightNumber.ToString(),
                (originAirport.City, originAirport.Iata),
                (destinationAirport.City, destinationAirport.Iata)
            );
        }
    }

    public virtual async Task<FlightView> GetFlightByFlightNumber(int flightNumber)
    {
        try
        {
            Flight flight = await _flightRepository.GetFlightByFlightNumber(flightNumber);
            Airport originAirport = await _airportRepository.GetAirportById(flight.Origin);
            Airport destinationAirport = await _airportRepository.GetAirportById(flight.Destination);

            return new FlightView(
                flight.FlightNumber.ToString(),
                (originAirport.City, originAirport.Iata),
                (destinationAirport.City, destinationAirport.Iata)
            );
        }
        catch (FlightNotFoundException)
        {
            throw new FlightNotFoundException();
        }
        catch (Exception)
        {
            throw new ArgumentException();
        }
    }
}