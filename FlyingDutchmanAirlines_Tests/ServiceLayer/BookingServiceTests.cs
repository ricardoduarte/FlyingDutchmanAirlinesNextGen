using Moq;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines_Tests;

[TestClass]
public class BookingServiceTests
{
    private Mock<BookingRepository> _mockBookingRepository = default!;
    private Mock<CustomerRepository> _mockCustomerRepository = default!;
    private Mock<FlightRepository> _mockFlightRepository = default!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockBookingRepository = new Mock<BookingRepository>();
        _mockCustomerRepository = new Mock<CustomerRepository>();
        _mockFlightRepository = new Mock<FlightRepository>();
    }

    [TestMethod]
    public async Task CreateBooking_Success()
    {
        _mockBookingRepository.Setup(repository => repository.CreateBooking(0, 0)).Returns(Task.CompletedTask);
        _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Leo Tolstoy")).Returns(Task.FromResult(new Customer("Leo Tolstoy")));
        _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(0)).Returns(Task.FromResult(new Flight()));

        BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);
        (bool result, Exception exception) = await service.CreateBooking("Leo Tolstoy", 0);

        Assert.IsTrue(result);
        Assert.IsNull(exception);
    }

    [TestMethod]
    [DataRow("", 0)]
    [DataRow(null, -1)]
    [DataRow("Galileo Galilei", -1)]
    public async Task CreateBooking_Failure_InvalidInputArguments(string name, int flightNumber)
    {
        BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);
        (bool result, Exception exception) = await service.CreateBooking(name, flightNumber);

        Assert.IsFalse(result);
        Assert.IsNotNull(exception);
    }

    [TestMethod]
    public async Task CreateBooking_Failure_ArgumentException()
    {
        _mockBookingRepository.Setup(repository => repository.CreateBooking(0, 1)).Throws(new ArgumentException());
        _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Galileo Galilei")).Returns(Task.FromResult(new Customer("Galileo Galilei") { CustomerId = 0 } ));
        _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(1)).Returns(Task.FromResult(new Flight()));

        BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);
        
        (bool result, Exception exception) = await service.CreateBooking("Galileo Galilei", 1);
        Assert.IsFalse(result);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(ArgumentException));
    }

    [TestMethod]
    public async Task CreateBooking_Failure_CouldNotAddBookingToDatabaseException()
    {
        _mockBookingRepository.Setup(repository => repository.CreateBooking(1, 2)).Throws(new CouldNotAddBookingToDatabaseException());
        _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Eise Eisinga")).Returns(Task.FromResult(new Customer("Eise Eisinga") { CustomerId = 1 } ));

        BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);
        
        (bool result, Exception exception) = await service.CreateBooking("Eise Eisinga", 2);
        Assert.IsFalse(result);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
    }

    [TestMethod]
    public async Task CreateBooking_Failure_FlightNotInDatabase()
    {
        _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(-1)).Throws(new FlightNotFoundException());

        BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);
        
        (bool result, Exception exception) = await service.CreateBooking("Maurits Escher", 1);
        Assert.IsFalse(result);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
    }

    [TestMethod]
    public async Task CreateBooking_Success_CustomerNotInDatabase()
    {
        _mockBookingRepository.Setup(repository => repository.CreateBooking(0, 0)).Returns(Task.CompletedTask);
        _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Konrad Zuse")).Throws(new CustomerNotFoundException());

        BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);
        
        (bool result, Exception exception) = await service.CreateBooking("Konrad Zuse", 0);
        Assert.IsFalse(result);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(CustomerNotFoundException));
    }

    [TestMethod]
    public async Task CreateBooking_Failure_CustomerNotInDatabase_RepositoryFailure()
    {
        _mockBookingRepository.Setup(repository => repository.CreateBooking(0, 0)).Throws(new CouldNotAddBookingToDatabaseException());
        _mockCustomerRepository.Setup(repository => repository.GetCustomerByName("Bill Gates")).Returns(Task.FromResult(new Customer("Bill Gates")));
        _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(1)).Returns(Task.FromResult(new Flight()));

        BookingService service = new BookingService(_mockBookingRepository.Object, _mockCustomerRepository.Object, _mockFlightRepository.Object);
        
        (bool result, Exception exception) = await service.CreateBooking("Bill Gates", 0);
        Assert.IsFalse(result);
        Assert.IsNotNull(exception);
        Assert.IsInstanceOfType(exception, typeof(CouldNotAddBookingToDatabaseException));
    }
}