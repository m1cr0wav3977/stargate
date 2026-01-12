using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Commands.AstronautDuty
{
    public class AddAstronautDutyHandlerTests
    {
        [Fact]
        public async Task Handle_WhenFirstDutyIsSpaceman_ShouldCreateDutyAndAstronautDetail()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            
            var handler = new AddAstronautDutyHandler(context);
            var request = new AddAstronautDuty
            {
                Name = "Test Person",
                PersonId = person.Id,
                Rank = "CPT",
                DutyTitle = "Spaceman",
                DutyStartDate = DateTime.UtcNow.Date
            };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Id.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            
            var duty = await context.AstronautDuties.FirstOrDefaultAsync(d => d.Id == result.Id);
            duty.Should().NotBeNull();
            duty!.DutyTitle.Should().Be("Spaceman");
            duty.Rank.Should().Be("CPT");
            
            var detail = await context.AstronautDetails.FirstOrDefaultAsync(d => d.PersonId == person.Id);
            detail.Should().NotBeNull();
            detail!.CurrentDutyTitle.Should().Be("Spaceman");
            detail.CurrentRank.Should().Be("CPT");
        }

        [Fact]
        public async Task Handle_WhenFirstDutyIsNotSpaceman_ShouldThrowBadHttpRequestException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            
            var handler = new AddAstronautDutyHandler(context);
            var request = new AddAstronautDuty
            {
                Name = "Test Person",
                PersonId = person.Id,
                Rank = "CPT",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.UtcNow.Date
            };

            // Act
            var act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("To enroll in the astronaut program, the first duty title must be 'Spaceman'");
        }

        [Fact]
        public async Task Handle_WhenAddingSecondDuty_ShouldUpdatePreviousDutyEndDate()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            var firstDuty = await TestDbContextHelper.CreateTestAstronautDutyAsync(
                context, person.Id, "CPT", "Spaceman", DateTime.UtcNow.Date);
            
            var handler = new AddAstronautDutyHandler(context);
            var newDutyStartDate = DateTime.UtcNow.AddDays(30).Date;
            var request = new AddAstronautDuty
            {
                Name = "Test Person",
                PersonId = person.Id,
                Rank = "CAPT",
                DutyTitle = "Commander",
                DutyStartDate = newDutyStartDate
            };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var updatedFirstDuty = await context.AstronautDuties.FirstOrDefaultAsync(d => d.Id == firstDuty.Id);
            updatedFirstDuty.Should().NotBeNull();
            updatedFirstDuty!.DutyEndDate.Should().Be(newDutyStartDate.AddDays(-1));
            
            var newDuty = await context.AstronautDuties
                .FirstOrDefaultAsync(d => d.PersonId == person.Id && d.DutyEndDate == null);
            newDuty.Should().NotBeNull();
            newDuty!.DutyTitle.Should().Be("Commander");
        }

        [Fact]
        public async Task Handle_WhenAddingRetiredDuty_ShouldSetCareerEndDate()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            await TestDbContextHelper.CreateTestAstronautDutyAsync(context, person.Id, "CPT", "Spaceman");
            await TestDbContextHelper.CreateTestAstronautDetailAsync(context, person.Id);
            
            var handler = new AddAstronautDutyHandler(context);
            var retiredDate = DateTime.UtcNow.AddDays(365).Date;
            var request = new AddAstronautDuty
            {
                Name = "Test Person",
                PersonId = person.Id,
                Rank = "CAPT",
                DutyTitle = "RETIRED",
                DutyStartDate = retiredDate
            };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var detail = await context.AstronautDetails.FirstOrDefaultAsync(d => d.PersonId == person.Id);
            detail.Should().NotBeNull();
            detail!.CareerEndDate.Should().Be(retiredDate.AddDays(-1));
        }

        [Fact]
        public async Task Handle_WhenAddingDuty_ShouldUpdateAstronautDetail()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            await TestDbContextHelper.CreateTestAstronautDutyAsync(context, person.Id, "CPT", "Spaceman");
            await TestDbContextHelper.CreateTestAstronautDetailAsync(context, person.Id, "CPT", "Spaceman");
            
            var handler = new AddAstronautDutyHandler(context);
            var request = new AddAstronautDuty
            {
                Name = "Test Person",
                PersonId = person.Id,
                Rank = "CAPT",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.UtcNow.AddDays(30).Date
            };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var detail = await context.AstronautDetails.FirstOrDefaultAsync(d => d.PersonId == person.Id);
            detail.Should().NotBeNull();
            detail!.CurrentRank.Should().Be("CAPT");
            detail.CurrentDutyTitle.Should().Be("Commander");
        }
    }
}

