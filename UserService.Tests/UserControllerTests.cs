using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using UserService.Controllers;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Tests;

[TestClass]
public class UserControllerTests
{
    private Mock<IUserRepository> _mockRepository = null!;
    private Mock<ILogger<UserController>> _mockLogger = null!;
    private UserController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserController>>();
        _controller = new UserController(_mockRepository.Object, _mockLogger.Object);
    }

    [TestMethod]
    public void GetById_ReturnsOk_WhenUserExists()
    {
        var userId = Guid.NewGuid().ToString();

        var user = new User
        {
            Id = userId,
            FirstName = "Enni",
            LastName = "Test",
            Email = "enni@example.com",
            PhoneNumber = "12345678",
            Membership = MembershipType.Standard,
            MembershipStatus = MembershipStatus.Active,
            TimeCreated = DateTime.UtcNow
        };

        _mockRepository
            .Setup(repository => repository.GetById(userId))
            .Returns(user);

        var result = _controller.GetById(userId);

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

        var okResult = result.Result as OkObjectResult;
        var returnedUser = okResult?.Value as User;

        Assert.IsNotNull(returnedUser);
        Assert.AreEqual(userId, returnedUser.Id);
    }

    [TestMethod]
    public void GetById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid().ToString();

        _mockRepository
            .Setup(repository => repository.GetById(userId))
            .Returns((User?)null);

        var result = _controller.GetById(userId);

        Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
    }
    
    [TestMethod]
    public void Delete_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var userId = Guid.NewGuid().ToString();

        _mockRepository
            .Setup(repository => repository.Delete(userId))
            .Returns(false);

        var result = _controller.Delete(userId);

        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
}