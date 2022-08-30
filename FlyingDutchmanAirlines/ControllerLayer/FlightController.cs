using System.Net;
using Microsoft.AspNetCore.Mvc;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines.ControllerLayer;

public class FlightController : Controller
{
    private readonly FlightService _service;

    public FlightController(FlightService service)
    {
        _service = service;
    }

    public async Task<IActionResult> GetFlights()
    {
        try
        {
            Queue<FlightView> flights = new Queue<FlightView>();
            await foreach(FlightView flight in _service.GetFlights())
            {
                flights.Enqueue(flight);
            }
            return StatusCode((int)HttpStatusCode.OK, flights);
        }
        catch (FlightNotFoundException)
        {
            return StatusCode((int)HttpStatusCode.NotFound, "No flights were found in the database");
        }
        catch (Exception)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred");
        }
    }
}