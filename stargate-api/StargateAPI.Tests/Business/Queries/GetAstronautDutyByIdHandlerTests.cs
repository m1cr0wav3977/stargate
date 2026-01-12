using FluentAssertions;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Queries
{
    public class GetAstronautDutyByIdHandlerTests
    {
        [Fact]
        public async Task Handle_WhenDutyExists_ShouldReturnDuty()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            var duty = await TestDbContextHelper.CreateTestAstronautDutyAsync(context, person.Id, "CPT", "Spaceman");
            
            var handler = new GetAstronautDutyByIdHandler(context);
            var request = new GetAstronautDutyById { Id = duty.Id };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AstronautDuty.Should().NotBeNull();
            result.AstronautDuty!.Id.Should().Be(duty.Id);
            result.AstronautDuty.DutyTitle.Should().Be("Spaceman");
            result.AstronautDuty.Rank.Should().Be("CPT");
        }

        [Fact]
        public async Task Handle_WhenDutyDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new GetAstronautDutyByIdHandler(context);
            var request = new GetAstronautDutyById { Id = 999 };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AstronautDuty.Should().BeNull();
        }
    }
}

