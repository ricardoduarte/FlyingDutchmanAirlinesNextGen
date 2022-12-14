using Microsoft.EntityFrameworkCore;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using FlyingDutchmanAirlines.RepositoryLayer;

namespace FlyingDutchmanAirlines_Tests;

[TestClass]
public class CustomerRepositoryTests
{
    private FlyingDutchmanAirlinesContext _context = default!;
    private CustomerRepository _repository = default!;

    [TestInitialize]
    public async Task TestInitialize()
    {
        DbContextOptions<FlyingDutchmanAirlinesContext> dbContextOptions = new DbContextOptionsBuilder<FlyingDutchmanAirlinesContext>().UseInMemoryDatabase("FlyingDutchman").Options;
        _context = new FlyingDutchmanAirlinesContext(dbContextOptions);
        _repository = new CustomerRepository(_context);
        Assert.IsNotNull(_repository);

        Customer testCustomer = new Customer("Linus Torvalds");
        _context.Customers.Add(testCustomer);
        await _context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task CreateCustomer_Success()
    {
        bool result = await _repository.CreateCustomer("Ricardo Duarte");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task CreateCustomer_Failure_NameIsNull()
    {
        bool result = await _repository.CreateCustomer(null!);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task CreateCustomer_Failure_NameIsEmptyString()
    {
        bool result = await _repository.CreateCustomer("");
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow('#')]
    [DataRow('@')]
    [DataRow('%')]
    [DataRow('*')]
    [DataRow('&')]
    public async Task CreateCustomer_Failure_NameContainsInvalidChars(char invalidChar)
    {
        bool result = await _repository.CreateCustomer("Ricardo Duarte" + invalidChar);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task CreateCustomer_Failure_DatabaseAccessError()
    {
        CustomerRepository repository = new CustomerRepository(null!);
        Assert.IsNotNull(repository);

        bool result = await repository.CreateCustomer("Ricardo Duarte");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task GetCustomerByName_Success()
    {
        Customer customer = await _repository.GetCustomerByName("Linus Torvalds");
        Assert.IsNotNull(customer);

        Customer dbCustomer = _context.Customers.First();
        Assert.AreEqual(customer, dbCustomer);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(null)]
    [DataRow("#")]
    [DataRow("@")]
    [DataRow("%")]
    [DataRow("*")]
    [DataRow("&")]
    [ExpectedException(typeof(CustomerNotFoundException))]
    public async Task GetCustomerByName_Failure_InvalidName(string name)
    {
        await _repository.GetCustomerByName(name);
    }
}