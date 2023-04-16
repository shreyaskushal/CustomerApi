using CustomerApplication.Data;
using CustomerApplication.Models;
using Microsoft.AspNetCore.Http;
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

		public CustomerService(CustomerApiDbContext dbContext)
		{
			this._dbContext = dbContext;
		}

		public IEnumerable<Customer> GetAllCustomers()
		{
			return _dbContext.Customers.ToList();
		}

		public Customer GetCustomerById(Guid id)
		{
			var existingCustomer = _dbContext.Customers.Find(id);

			if (id == Guid.Empty)
			{
				throw new ArgumentException("Id cannot be empty");
			}
			if (existingCustomer == null)
			{
				throw new Exception($"Customer with id {id} does not exist");
				
			}
			return existingCustomer;
		}

		public IEnumerable<Customer> GetAllCustomerByFirstName(string firstName)
		{
			if (string.IsNullOrEmpty(firstName))
			{
				throw new ArgumentException("First name cannot be null or empty");
			}
			return _dbContext.Customers.Where(x => x.FirstName.ToLower() == firstName.ToLower()).ToList();
		}

		public Customer GetCustomerByFirstAndLastName(string firstName, string LastName)
		{
			if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(LastName))
			{
				throw new ArgumentException("First name and last name cannot be null or empty");
			}
			return _dbContext.Customers.FirstOrDefault(x => x.FirstName.ToLower() == firstName.ToLower() && x.LastName.ToLower() == LastName.ToLower());
		}

		public IEnumerable<Customer> GetAllCustomersBetweenAge(int minAge, int maxAge)
		{
			if (minAge == default(int) || maxAge == default(int))
			{
				throw new ArgumentException("Age should be greater than 1");
			}
			return _dbContext.Customers.Where(x => x.Age >= minAge && x.Age <= maxAge).ToList();
		}

		public IEnumerable<Customer> GetAllCustomersByCountry(string country)
		{
			var result = new List<Customer>();

			if (string.IsNullOrEmpty(country))
			{
				throw new ArgumentException("Country cannot be null or empty");
			}
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
			var customerExists = _dbContext.Customers.Any(x => x.FirstName == customer.FirstName && x.LastName == customer.LastName);

			if (customerExists)
			{
				throw new ArgumentException($"Customer with name {customer.FirstName} {customer.LastName} already exists");
			}

			_dbContext.Customers.Add(customer);
			return await _dbContext.SaveChangesAsync();
			
		}

		public async Task<int> UpdateCustomer(Guid id, Customer customer)
		{
			var existingCustomer = _dbContext.Customers.Find(id);
			
			if (id == Guid.Empty)
			{
				throw new ArgumentException("Id cannot be empty");
			}
			if (existingCustomer == null)
			{
				throw new ArgumentException($"Customer with id {id} does not exist");
			}

			var customerWithSameNameExists = _dbContext.Customers.Any(x => x.Id != id && x.FirstName == customer.FirstName && x.LastName == customer.LastName);

			if (customerWithSameNameExists)
			{
				throw new ArgumentException($"Customer with name {customer.FirstName} {customer.LastName} already exists");
			}

			existingCustomer.FirstName = customer.FirstName;
			existingCustomer.LastName = customer.LastName;
			existingCustomer.Age = customer.Age;
			existingCustomer.Address = customer.Address;

			return await _dbContext.SaveChangesAsync();

		}

		public IEnumerable<Customer> UpdateAllCustomersAgeByLastName(string lastName, int age)
		{
			var existingCustomers = _dbContext.Customers.Where(x => x.LastName.ToLower() == lastName.ToLower()).ToList();

			if (string.IsNullOrEmpty(lastName))
			{
				throw new ArgumentException("lastName cannot be null or empty");
			}

			if (age == default(int))
			{
				throw new ArgumentException("Age should be greater than 1");
			}

			if (!existingCustomers.Any())
			{
				throw new ArgumentException($"Customer with lastname {lastName} does not exist");
			}

			existingCustomers.ForEach(x => x.Age = age);

			_dbContext.SaveChanges();
			return existingCustomers;
		}

		public void DeleteCustomerById(Guid id)
		{
			var existingCustomer = _dbContext.Customers.Find(id);

			if (id == Guid.Empty)
			{
				throw new ArgumentException("Id cannot be empty");
			}

			if (existingCustomer == null)
			{
				throw new ArgumentException($"Customer with id {id} does not exist");
			}

			_dbContext.Customers.Remove(existingCustomer);
			_dbContext.SaveChanges();
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
				throw new ArgumentException("Customers does not exist");
			}
		}
	}
}



