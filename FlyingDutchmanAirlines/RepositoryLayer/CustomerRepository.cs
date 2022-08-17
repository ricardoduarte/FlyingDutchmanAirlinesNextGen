using System.Linq;
using FlyingDutchmanAirlines.DatabaseLayer;
using FlyingDutchmanAirlines.DatabaseLayer.Models;
using FlyingDutchmanAirlines.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FlyingDutchmanAirlines.RepositoryLayer;

public class CustomerRepository
{
    private readonly FlyingDutchmanAirlinesContext _context;

    public CustomerRepository(FlyingDutchmanAirlinesContext _context)
    {
        this._context = _context;
    }

    public async Task<bool> CreateCustomer(string name)
    {
        if (IsInvalidCustomerName(name))
        {
            return false;
        }

        try
        {
            Customer customer = new Customer(name);
            using (_context)
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<Customer> GetCustomerByName(string name)
    {
        if (IsInvalidCustomerName(name))
        {
            throw new CustomerNotFoundException();
        }
        return await _context.Customers.FirstOrDefaultAsync(c => c.Name == name) ?? throw new CustomerNotFoundException();
    }

    private bool IsInvalidCustomerName(string name)
    {
        char[] forbiddenChars = {'!', '@', '#', '%', '$', '&', '*'};
        return string.IsNullOrEmpty(name) || name.Any(x => forbiddenChars.Contains(x));
    }
}
