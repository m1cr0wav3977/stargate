using FluentAssertions;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Queries
{
    public class GetPersonByIdHandlerTests
    {
        [Fact]
        public async Task Handle_WhenPersonExists_ShouldReturnPerson()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            
            var handler = new GetPersonByIdHandler(context);
            var request = new GetPersonById { Id = person.Id };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Person.Should().NotBeNull();
            result.Person!.PersonId.Should().Be(person.Id);
            result.Person.Name.Should().Be("Test Person");
        }

        [Fact]
        public async Task Handle_WhenPersonDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new GetPersonByIdHandler(context);
            var request = new GetPersonById { Id = 999 };

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
            
            var handler = new GetPersonByIdHandler(context);
            var request = new GetPersonById { Id = person.Id };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Person.Should().NotBeNull();
            result.Person!.PersonId.Should().Be(person.Id);
        }
    }
}

