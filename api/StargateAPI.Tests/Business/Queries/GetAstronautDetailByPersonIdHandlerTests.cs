using FluentAssertions;
using StargateAPI.Business.Queries;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Queries
{
    public class GetAstronautDetailByPersonIdHandlerTests
    {
        [Fact]
        public async Task Handle_WhenDetailExists_ShouldReturnDetail()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            var detail = await TestDbContextHelper.CreateTestAstronautDetailAsync(context, person.Id, "CAPT", "Commander");
            
            var handler = new GetAstronautDetailByPersonIdHandler(context);
            var request = new GetAstronautDetailByPersonId { PersonId = person.Id };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AstronautDetail.Should().NotBeNull();
            result.AstronautDetail!.PersonId.Should().Be(person.Id);
            result.AstronautDetail.CurrentRank.Should().Be("CAPT");
            result.AstronautDetail.CurrentDutyTitle.Should().Be("Commander");
        }

        [Fact]
        public async Task Handle_WhenDetailDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            
            var handler = new GetAstronautDetailByPersonIdHandler(context);
            var request = new GetAstronautDetailByPersonId { PersonId = person.Id };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.AstronautDetail.Should().BeNull();
        }
    }
}

