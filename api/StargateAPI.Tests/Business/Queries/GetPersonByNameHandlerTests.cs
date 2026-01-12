using FluentAssertions;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Queries
{
    public class GetPersonByNameHandlerTests
    {
        [Fact]
        public async Task Handle_WhenPersonExists_ShouldReturnPerson()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "John Doe");
            
            var handler = new GetPersonByNameHandler(context);
            var request = new GetPersonByName { Name = "John Doe" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Person.Should().NotBeNull();
            result.Person!.Name.Should().Be("John Doe");
            result.Person.PersonId.Should().Be(person.Id);
        }

        [Fact]
        public async Task Handle_WhenPersonDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new GetPersonByNameHandler(context);
            var request = new GetPersonByName { Name = "Non Existent" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Person.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenPersonHasAstronautDetail_ShouldIncludeDetail()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Astronaut");
            await TestDbContextHelper.CreateTestAstronautDetailAsync(context, person.Id, "CAPT", "Commander");
            
            var handler = new GetPersonByNameHandler(context);
            var request = new GetPersonByName { Name = "Astronaut" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Person.Should().NotBeNull();
            result.Person!.Name.Should().Be("Astronaut");
            result.Person.CurrentRank.Should().Be("CAPT");
            result.Person.CurrentDutyTitle.Should().Be("Commander");
        }

        [Fact]
        public async Task Handle_WhenPersonHasNoAstronautDetail_ShouldReturnPersonWithoutDetail()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            await TestDbContextHelper.CreateTestPersonAsync(context, "Regular Person");
            
            var handler = new GetPersonByNameHandler(context);
            var request = new GetPersonByName { Name = "Regular Person" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Person.Should().NotBeNull();
            result.Person!.Name.Should().Be("Regular Person");
        }
    }
}

