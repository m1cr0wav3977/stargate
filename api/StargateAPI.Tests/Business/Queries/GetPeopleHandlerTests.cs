using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Queries
{
    public class GetPeopleHandlerTests
    {
        [Fact]
        public async Task Handle_WhenNoPeopleExist_ShouldReturnEmptyList()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new GetPeopleHandler(context);
            var request = new GetPeople();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.People.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenPeopleExist_ShouldReturnAllPeople()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            await TestDbContextHelper.CreateTestPersonAsync(context, "Person 1");
            await TestDbContextHelper.CreateTestPersonAsync(context, "Person 2");
            await TestDbContextHelper.CreateTestPersonAsync(context, "Person 3");
            
            var handler = new GetPeopleHandler(context);
            var request = new GetPeople();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.People.Should().HaveCount(3);
            result.People.Should().Contain(p => p.Name == "Person 1");
            result.People.Should().Contain(p => p.Name == "Person 2");
            result.People.Should().Contain(p => p.Name == "Person 3");
        }

        [Fact]
        public async Task Handle_WhenPeopleWithAstronautDetailsExist_ShouldReturnPeopleWithDetails()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person1 = await TestDbContextHelper.CreateTestPersonAsync(context, "Astronaut 1");
            var person2 = await TestDbContextHelper.CreateTestPersonAsync(context, "Regular Person");
            await TestDbContextHelper.CreateTestAstronautDetailAsync(context, person1.Id);
            
            var handler = new GetPeopleHandler(context);
            var request = new GetPeople();

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.People.Should().HaveCount(2);
            var astronaut = result.People.FirstOrDefault(p => p.Name == "Astronaut 1");
            astronaut.Should().NotBeNull();
        }
    }
}

