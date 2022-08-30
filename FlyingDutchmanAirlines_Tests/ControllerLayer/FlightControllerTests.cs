using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FlyingDutchmanAirlines.ControllerLayer;
using FlyingDutchmanAirlines.ServiceLayer;
using FlyingDutchmanAirlines.Views;
using FlyingDutchmanAirlines.Exceptions;

namespace FlyingDutchmanAirlines_Tests;

[TestClass]
public class FlightControllerTests
{
    [TestMethod]
    public async Task GetFlights_Success()
    {
        Mock<FlightService> service = new Mock<FlightService>();
        List<FlightView> returnFlightViews = new List<FlightView>(2)
        {
            new FlightView("192", ("Groningen", "GRQ"), ("Phoenix", "PFX")),
            new FlightView("841", ("New York City", "JFK"), ("London", "LHR")),
        };

        service.Setup(s => s.GetFlights()).Returns(FlightViewAsyncGenerator(returnFlightViews));

        FlightController controller = new FlightController(service.Object);
        ObjectResult response = await controller.GetFlights() as ObjectResult;

        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.OK, response.StatusCode);

        Queue<FlightView> content = response.Value as Queue<FlightView>;
        Assert.IsNotNull(content);
        Assert.IsTrue(returnFlightViews.All(flight => content.Contains(flight)));
    }

    [TestMethod]
    public async Task GetFlights_Failure_FlightNotFoundException_404()
    {
        Mock<FlightService> service = new Mock<FlightService>();
        service.Setup(s => s.GetFlights()).Throws(new FlightNotFoundException());

        FlightController controller = new FlightController(service.Object);
        ObjectResult response = await controller.GetFlights() as ObjectResult;

        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.NotFound, response.StatusCode);
        Assert.AreEqual("No flights were found in the database", response.Value);
    }

    [TestMethod]
    public async Task GetFlights_Failure_ArgumentException_500()
    {
        Mock<FlightService> service = new Mock<FlightService>();
        service.Setup(s => s.GetFlights()).Throws(new ArgumentException());

        FlightController controller = new FlightController(service.Object);
        ObjectResult response = await controller.GetFlights() as ObjectResult;

        Assert.IsNotNull(response);
        Assert.AreEqual((int)HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.AreEqual("An error occurred", response.Value);
    }

    private async IAsyncEnumerable<FlightView> FlightViewAsyncGenerator(IEnumerable<FlightView> views)
    {
        foreach (FlightView view in views)
        {
            yield return view;
        }
    }
}