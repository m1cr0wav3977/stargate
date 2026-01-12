using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Commands.Person
{
    public class CreatePersonHandlerTests
    {
        [Fact]
        public async Task Handle_WhenNameIsValid_ShouldCreatePersonAndReturnId()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new CreatePersonHandler(context);
            var request = new CreatePerson { Name = "John Doe" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            
            var createdPerson = await context.People.FirstOrDefaultAsync(p => p.Id == result.Id);
            createdPerson.Should().NotBeNull();
            createdPerson!.Name.Should().Be("John Doe");
        }

        [Fact]
        public async Task Handle_WhenNameIsValid_ShouldSaveToDatabase()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new CreatePersonHandler(context);
            var request = new CreatePerson { Name = "Jane Smith" };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var people = await context.People.ToListAsync();
            people.Should().Contain(p => p.Name == "Jane Smith");
            people.Count.Should().Be(1);
        }

        [Fact]
        public async Task Handle_WhenCalledMultipleTimes_ShouldCreateMultiplePeople()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new CreatePersonHandler(context);

            // Act
            await handler.Handle(new CreatePerson { Name = "Person 1" }, CancellationToken.None);
            await handler.Handle(new CreatePerson { Name = "Person 2" }, CancellationToken.None);
            await handler.Handle(new CreatePerson { Name = "Person 3" }, CancellationToken.None);

            // Assert
            var people = await context.People.ToListAsync();
            people.Count.Should().Be(3);
            people.Should().Contain(p => p.Name == "Person 1");
            people.Should().Contain(p => p.Name == "Person 2");
            people.Should().Contain(p => p.Name == "Person 3");
        }
    }
}

