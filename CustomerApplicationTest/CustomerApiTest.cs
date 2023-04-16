using CustomerApplication.Controllers;
using CustomerApplication.Data;
using CustomerApplication.Models;
using CustomerApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CustomerApplicationTest
{
	public class CustomerApiTest
	{
		[Fact]
		public async void AddCustomer_Returns_Customer()
		{
			//Arrange

			var options = new DbContextOptionsBuilder<CustomerApiDbContext>()
				.UseInMemoryDatabase(databaseName: "CustomerDb")
				.Options;

			var dbContext = new CustomerApiDbContext(options);

			var customer = new CustomerRequest { FirstName = "Joe", LastName = "Smith", Age = 30, Address = "Netherlands" };

			var customerService = new CustomerService(dbContext);
			var customerController = new CustomerController(customerService);

			// Act
			var result = await customerController.AddCustomer(customer);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var customers = Assert.IsAssignableFrom<Customer>(okResult.Value);
			Assert.Equal("Joe", customers.FirstName);
		}

		[Fact]
		public void GetCustomers_Returns_AllCustomers()
		{
			//Arrange

			var options = new DbContextOptionsBuilder<CustomerApiDbContext>()
				.UseInMemoryDatabase(databaseName: "CustomerDb")
				.Options;

			var dbContext = new CustomerApiDbContext(options);

			dbContext.Customers.Add(new Customer { Id = Guid.NewGuid(), FirstName = "John", LastName = "Smith", Age = 30, Address = "Netherlands" });
			dbContext.Customers.Add(new Customer { Id = Guid.NewGuid(), FirstName = "Steve", LastName = "Smith", Age = 30, Address = "Netherlands" });
			dbContext.SaveChanges();

			var customerService = new CustomerService(dbContext);
			var customerController = new CustomerController(customerService);

			// Act
			var result = customerController.GetAllCustomers();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var customers = Assert.IsAssignableFrom<IEnumerable<Customer>>(okResult.Value);
			Assert.Equal("Netherlands", customers.First().Address);
			Assert.Equal(200, okResult.StatusCode);

		}

		[Fact]
		public void GetCustomerById_Returns_NotFoundError()
		{
			//Arrange

			var options = new DbContextOptionsBuilder<CustomerApiDbContext>()
				.UseInMemoryDatabase(databaseName: "CustomerDb")
				.Options;

			var customerId = Guid.NewGuid();

			var dbContext = new CustomerApiDbContext(options);

			var customerService = new CustomerService(dbContext);
			var customerController = new CustomerController(customerService);

			// Act
			var result = customerController.GetCustomerById(customerId);

			// Assert
			var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
			Assert.Equal(404, notFoundResult.StatusCode);
			Assert.Equal($"Customer with id {customerId} does not exist", notFoundResult.Value);
		}

		[Fact]
		public void UpdateAllCustomersAgeByLastName_Returns_Customers()
		{
			//Arrange
			string lastName = "Smith";
			int newAge = 30;

			var options = new DbContextOptionsBuilder<CustomerApiDbContext>()
				.UseInMemoryDatabase(databaseName: "CustomerDb")
				.Options;

			var dbContext = new CustomerApiDbContext(options);

			dbContext.Customers.Add(new Customer { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Age = 30, Address = "Netherlands" });
			dbContext.SaveChanges();

			var customerService = new CustomerService(dbContext);
			var customerController = new CustomerController(customerService);

			// Act
			var result = customerController.UpdateAllCustomersAgeByLastName(lastName, newAge);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result);
			var customers = Assert.IsAssignableFrom<IEnumerable<Customer>>(okResult.Value);
			Assert.Equal(30, customers.First().Age);
			Assert.Equal(200, okResult.StatusCode);
		}

		[Fact]
		public void DeleteCustomerById_Returns_Success()
		{
			//Arrange

			var options = new DbContextOptionsBuilder<CustomerApiDbContext>()
				.UseInMemoryDatabase(databaseName: "CustomerDb")
				.Options;

			var dbContext = new CustomerApiDbContext(options);

			dbContext.Customers.Add(new Customer { Id = new Guid("fa4a05bb-03c8-4d3f-a3c2-4c1d8b44f9b3"), FirstName = "Jane", LastName = "Smith", Age = 30, Address = "Netherlands" });
			dbContext.SaveChanges();

			var customerService = new CustomerService(dbContext);
			var customerController = new CustomerController(customerService);

			// Act
			var result = customerController.DeleteCustomerById(new Guid("fa4a05bb-03c8-4d3f-a3c2-4c1d8b44f9b3"));

			// Assert
			var okResult = Assert.IsType<OkResult>(result);
			Assert.Equal(200, okResult.StatusCode);
		}
	}
}
