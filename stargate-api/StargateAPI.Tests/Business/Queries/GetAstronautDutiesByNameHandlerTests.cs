using FluentAssertions;
using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Queries
{
    public class GetAstronautDutiesByNameHandlerTests
    {
        [Fact]
        public async Task Handle_WhenPersonHasNoDuties_ShouldReturnEmptyList()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            
            var handler = new GetAstronautDutiesByNameHandler(context);
            var request = new GetAstronautDutiesByName { Name = "Test Person" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AstronautDuties.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenPersonHasDuties_ShouldReturnAllDuties()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            await TestDbContextHelper.CreateTestAstronautDutyAsync(context, person.Id, "CPT", "Spaceman", DateTime.UtcNow.Date);
            await TestDbContextHelper.CreateTestAstronautDutyAsync(context, person.Id, "CAPT", "Commander", DateTime.UtcNow.AddDays(30).Date);
            
            var handler = new GetAstronautDutiesByNameHandler(context);
            var request = new GetAstronautDutiesByName { Name = "Test Person" };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AstronautDuties.Should().HaveCount(2);
            result.AstronautDuties.Should().Contain(d => d.DutyTitle == "Spaceman");
            result.AstronautDuties.Should().Contain(d => d.DutyTitle == "Commander");
        }

        [Fact]
        public async Task Handle_WhenPersonDoesNotExist_ShouldThrowBadHttpRequestException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new GetAstronautDutiesByNameHandler(context);
            var request = new GetAstronautDutiesByName { Name = "Non Existent" };

            // Act
            var act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("Bad Request");
        }
    }
}

