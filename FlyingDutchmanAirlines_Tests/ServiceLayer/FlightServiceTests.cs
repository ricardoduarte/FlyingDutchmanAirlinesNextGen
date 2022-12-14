using Moq;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.RepositoryLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines_Tests;

[TestClass]
public class FlightServiceTests
{
    private Mock<FlightRepository> _mockFlightRepository = default!;
    private Mock<AirportRepository> _mockAirportRepository = default!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockFlightRepository = new Mock<FlightRepository>();
        _mockAirportRepository = new Mock<AirportRepository>();

        Flight flightInDatabase = new Flight {
            FlightNumber = 148,
            Origin = 31,
            Destination = 92
        };

        Queue<Flight> mockReturn = new Queue<Flight>(1);
        mockReturn.Enqueue(flightInDatabase);
        _mockFlightRepository.Setup(repository => repository.GetFlights()).Returns(mockReturn);
        _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(148)).Returns(Task.FromResult(flightInDatabase));

        _mockAirportRepository.Setup(repository => repository.GetAirportById(31)).ReturnsAsync(new Airport
        {
            AirportId = 31,
            City = "Mexico City",
            Iata = "MEX"
        });
        _mockAirportRepository.Setup(repository => repository.GetAirportById(92)).ReturnsAsync(new Airport
        {
            AirportId = 92,
            City = "Ulaanbaataar",
            Iata = "UBN"
        });
    }

    [TestMethod]
    public async Task GetFlights_Success()
    {
        FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

        await foreach (FlightView view in service.GetFlights())
        {
            Assert.IsNotNull(view);
            Assert.AreEqual(view.FlightNumber, "148");
            Assert.AreEqual(view.Origin.City, "Mexico City");
            Assert.AreEqual(view.Origin.Code, "MEX");
            Assert.AreEqual(view.Destination.City, "Ulaanbaataar");
            Assert.AreEqual(view.Destination.Code, "UBN");
        }
    }

    [TestMethod]
    [ExpectedException(typeof(FlightNotFoundException))]
    public async Task GetFlights_Failure_RepositoryException()
    {
        _mockAirportRepository.Setup(repository => repository.GetAirportById(31)).ThrowsAsync(new FlightNotFoundException());

        FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

        await foreach (FlightView _ in service.GetFlights()) {;}
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task GetFlights_Failure_RegularException()
    {
        _mockAirportRepository.Setup(repository => repository.GetAirportById(31)).ThrowsAsync(new NullReferenceException());

        FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

        await foreach (FlightView _ in service.GetFlights()) {;}
    }

    [TestMethod]
    public async Task GetFlightByFlightNumber_Success()
    {
        FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);

        FlightView view = await service.GetFlightByFlightNumber(148);
        Assert.IsNotNull(view);
        Assert.AreEqual(view.FlightNumber, "148");
        Assert.AreEqual(view.Origin.City, "Mexico City");
        Assert.AreEqual(view.Origin.Code, "MEX");
        Assert.AreEqual(view.Destination.City, "Ulaanbaataar");
        Assert.AreEqual(view.Destination.Code, "UBN");
    }

    [TestMethod]
    [ExpectedException(typeof(FlightNotFoundException))]
    public async Task GetFlightByFlightNumber_Failure_RepositoryException_FlightNotFoundException()
    {
        _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(-1)).Throws(new FlightNotFoundException());
        FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
        await service.GetFlightByFlightNumber(-1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task GetFlightByFlightNumber_Failure_RepositoryException_ArgumentException()
    {
        _mockFlightRepository.Setup(repository => repository.GetFlightByFlightNumber(-1)).Throws(new OverflowException());
        FlightService service = new FlightService(_mockFlightRepository.Object, _mockAirportRepository.Object);
        await service.GetFlightByFlightNumber(-1);
    }
}