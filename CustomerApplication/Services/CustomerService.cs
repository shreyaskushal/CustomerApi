using CustomerApplication.Data;
using CustomerApplication.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomerApplication.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly CustomerApiDbContext _dbContext;
		private DbContextOptionsBuilder<CustomerApiDbContext> dbContext;

		public CustomerService(CustomerApiDbContext dbContext)
		{
			this._dbContext = dbContext;
		}

		public CustomerService(DbContextOptionsBuilder<CustomerApiDbContext> dbContext)
		{
			this.dbContext = dbContext;
		}

		public IEnumerable<Customer> GetAllCustomers()
		{
			return _dbContext.Customers.ToList();
		}

		public Customer GetCustomerById(Guid id)
		{
			var existingCustomer = _dbContext.Customers.Find(id);
			if (existingCustomer != null)
			{
				return existingCustomer;
			}
			else
			{
				throw new Exception("Customer does not exists");
			}
		}

		public IEnumerable<Customer> GetAllCustomerByFirstName(string firstName)
		{
			return _dbContext.Customers.Where(x => x.FirstName.ToLower() == firstName.ToLower());
		}

		public Customer GetCustomerByFirstAndLastName(string firstName, string LastName)
		{
			return _dbContext.Customers.FirstOrDefault(x => x.FirstName.ToLower() == firstName.ToLower() && x.LastName.ToLower() == LastName.ToLower());
		}

		public IEnumerable<Customer> GetAllCustomersBetweenAge(int minAge, int maxAge)
		{
			return _dbContext.Customers.Where(x => x.Age >= minAge && x.Age <= maxAge);
		}

		public IEnumerable<Customer> GetAllCustomersByCountry(string country)
		{
			var result = new List<Customer>();
			foreach (var item in _dbContext.Customers)
			{
				var Address = Regex.Replace(item.Address, @"[^\w\s]+", "").Split();

				if (Address.Contains(country))
				{
					result.Add(item);
				}
			}
			return result;
		}

		public async Task<int> AddCustomer(Customer customer)
		{
			var existingCustomer = _dbContext.Customers.Any(x => x.FirstName == customer.FirstName && x.LastName == customer.LastName);

			if (!existingCustomer)
			{
				_dbContext.Customers.Add(customer);
				return await _dbContext.SaveChangesAsync();
			}
			else
			{
				throw new Exception("Customer already exists");
			}
		}

		public async Task<int> UpdateCustomer(Guid id, Customer customer)
		{
			var existingCustomer = _dbContext.Customers.Find(id);
			var customerWithSameName = _dbContext.Customers.Any(x => x.FirstName == customer.FirstName && x.LastName == customer.LastName);

			if (existingCustomer != null && !customerWithSameName)
			{
				existingCustomer.FirstName = customer.FirstName;
				existingCustomer.LastName = customer.LastName;
				existingCustomer.Age = customer.Age;
				existingCustomer.Address = customer.Address;

				return await _dbContext.SaveChangesAsync();
			}
			else
			{
				if (customerWithSameName)
				{
					throw new Exception("Customer already exists");
				}
				throw new Exception("Customer does not exists");
			}
		}

		public IEnumerable<Customer> UpdateAllCustomersAgeByLastName(string lastName, int age)
		{
			var existingCustomers = _dbContext.Customers.Where(x => x.LastName.ToLower() == lastName.ToLower()).ToList();

			if (existingCustomers.Any())
			{
				existingCustomers.ForEach(x => x.Age = age);

				_dbContext.SaveChanges();
				return existingCustomers;
			}
			else
			{
				throw new Exception("Customer does not exists");
			}
		}

		public void DeleteCustomerById(Guid id)
		{
			var existingCustomer = _dbContext.Customers.Find(id);

			if(existingCustomer != null)
			{
				_dbContext.Customers.Remove(existingCustomer);
				_dbContext.SaveChanges();
			}
			else
			{
				throw new Exception("Customer does not exists");
			}
		}

		public void DeleteMultipleCustomerByIds(IEnumerable<Guid> ids)
		{
			var existingCustomers = _dbContext.Customers.Where(x => ids.Contains(x.Id)).ToList();

			if (existingCustomers.Any())
			{
				_dbContext.Customers.RemoveRange(existingCustomers);
				_dbContext.SaveChanges();
			}
			else
			{
				throw new Exception("Customer does not exists");
			}
		}
	}
}



