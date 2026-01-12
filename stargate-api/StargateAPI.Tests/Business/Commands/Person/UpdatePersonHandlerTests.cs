using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Commands.Person
{
    public class UpdatePersonHandlerTests
    {
        [Fact]
        public async Task Handle_WhenPersonExists_ShouldUpdateName()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Original Name");
            
            var handler = new UpdatePersonHandler(context);
            var request = new UpdatePerson 
            { 
                Id = person.Id, 
                Name = "Updated Name" 
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Id.Should().Be(person.Id);
            var updatedPerson = await context.People.FirstOrDefaultAsync(p => p.Id == person.Id);
            updatedPerson!.Name.Should().Be("Updated Name");
        }

        [Fact]
        public async Task Handle_WhenPersonDoesNotExist_ShouldThrowBadHttpRequestException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new UpdatePersonHandler(context);
            var request = new UpdatePerson { Id = 999, Name = "Non Existent" };

            // Act
            var act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("Bad Request");
        }

        [Fact]
        public async Task Handle_WhenUpdatingAstronautDetail_ShouldUpdateFields()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            await TestDbContextHelper.CreateTestAstronautDetailAsync(context, person.Id, "CPT", "Spaceman");
            
            var handler = new UpdatePersonHandler(context);
            var request = new UpdatePerson 
            { 
                Id = person.Id,
                CurrentRank = "CAPT",
                CurrentDutyTitle = "Commander"
            };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var detail = await context.AstronautDetails.FirstOrDefaultAsync(d => d.PersonId == person.Id);
            detail.Should().NotBeNull();
            detail!.CurrentRank.Should().Be("CAPT");
            detail.CurrentDutyTitle.Should().Be("Commander");
        }

        [Fact]
        public async Task Handle_WhenAstronautDetailDoesNotExist_ShouldThrowBadHttpRequestException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            
            var handler = new UpdatePersonHandler(context);
            var request = new UpdatePerson 
            { 
                Id = person.Id,
                CurrentRank = "CAPT"
            };

            // Act
            var act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("Bad Request - AstronautDetail does not exist");
        }
    }
}

