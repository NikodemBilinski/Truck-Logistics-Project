using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using TrucksLogisticsServerAPI.Controllers;
using TrucksLogisticsServerAPI.Data;
using TrucksLogisticsServerAPI.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.Identity.Client;
using System.Diagnostics;
namespace tests;

public class UnitTest1
{

    private DataContext GetDataContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlServer("Server=.\\SQLEXPRESS;Database=TruckDB;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new DataContext(options);
    }

    

    [Fact]
    public async Task Test1()
    {
        // arrange

        var user = new Users
        {
            FirstName = "jj",
            LastName = "User",
            Age = 30,
            Username = "testuser",
            Password = "password",
            Role = "user"
        };

        var context = GetDataContext();

        var userscontroller = new UsersController(context);

        // act

        var result = await userscontroller.AddUser(user);


        // assert
        
        var saveduser = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");

        Assert.NotNull(saveduser);

        //cleanup

        context.Users.Remove(saveduser);
        await context.SaveChangesAsync();

    }
}
