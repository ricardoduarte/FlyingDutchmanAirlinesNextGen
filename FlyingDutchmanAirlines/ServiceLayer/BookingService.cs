using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines.ServiceLayer;

public class BookingService
{
    private readonly BookingRepository _bookingRepository = default!;
    private readonly CustomerRepository _customerRepository = default!;
    private readonly FlightRepository _flightRepository = default!;

    public BookingService(
        BookingRepository bookingRepository,
        CustomerRepository customerRepository,
        FlightRepository flightRepository)
    {
        _bookingRepository = bookingRepository;
        _customerRepository = customerRepository;
        _flightRepository = flightRepository;
    }

    public async Task<(bool, Exception?)> CreateBooking(string name, int flightNumber)
    {
        if (string.IsNullOrEmpty(name) || !flightNumber.IsPositive())
        {
            return (false, new ArgumentException());
        }

        try
        {
            if (!await FlightExistsInDatabase(flightNumber))
            {
                throw new CouldNotAddBookingToDatabaseException();
            }
            Customer customer = default!;
            try
            {
                customer = await _customerRepository.GetCustomerByName(name);
            }
            catch (CustomerNotFoundException)
            {
                await _customerRepository.CreateCustomer(name);
                return await CreateBooking(name, flightNumber);
            }
            await _bookingRepository.CreateBooking(customer.CustomerId, flightNumber);
            return (true, null);
        }
        catch (Exception exception)
        {
            return (false, exception);
        }
    }

    private async Task<bool> FlightExistsInDatabase(int flightNumber)
    {
        try
        {
            return await _flightRepository.GetFlightByFlightNumber(flightNumber) != null;
        }
        catch (FlightNotFoundException)
        {
            return false;
        }
    }
}