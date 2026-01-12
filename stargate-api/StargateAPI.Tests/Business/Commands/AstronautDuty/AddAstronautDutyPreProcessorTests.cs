using FluentAssertions;
using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Commands.AstronautDuty
{
    public class AddAstronautDutyPreProcessorTests
    {
        [Fact]
        public async Task Process_WhenPersonDoesNotExist_ShouldThrowBadHttpRequestException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var processor = new AddAstronautDutyPreProcessor(context);
            var request = new AddAstronautDuty
            {
                Name = "Non Existent",
                PersonId = 999,
                Rank = "CPT",
                DutyTitle = "Spaceman",
                DutyStartDate = DateTime.UtcNow
            };

            // Act
            var act = async () => await processor.Process(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("Person not found");
        }

        [Fact]
        public async Task Process_WhenFirstDutyIsSpaceman_ShouldNotThrowException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            
            var processor = new AddAstronautDutyPreProcessor(context);
            var request = new AddAstronautDuty
            {
                Name = "Test Person",
                PersonId = person.Id,
                Rank = "CPT",
                DutyTitle = "Spaceman",
                DutyStartDate = DateTime.UtcNow
            };

            // Act
            var act = async () => await processor.Process(request, CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Process_WhenFirstDutyIsNotSpaceman_ShouldThrowBadHttpRequestException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            
            var processor = new AddAstronautDutyPreProcessor(context);
            var request = new AddAstronautDuty
            {
                Name = "Test Person",
                PersonId = person.Id,
                Rank = "CPT",
                DutyTitle = "Commander",
                DutyStartDate = DateTime.UtcNow
            };

            // Act
            var act = async () => await processor.Process(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("To enroll in the astronaut program, the first duty title must be 'Spaceman'");
        }

        [Fact]
        public async Task Process_WhenDutyAlreadyExists_ShouldThrowBadHttpRequestException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            var dutyDate = DateTime.UtcNow.Date;
            await TestDbContextHelper.CreateTestAstronautDutyAsync(context, person.Id, "CPT", "Spaceman", dutyDate);
            
            var processor = new AddAstronautDutyPreProcessor(context);
            var request = new AddAstronautDuty
            {
                Name = "Test Person",
                PersonId = person.Id,
                Rank = "CPT",
                DutyTitle = "Spaceman",
                DutyStartDate = dutyDate
            };

            // Act
            var act = async () => await processor.Process(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("Duty already exists for this person and date");
        }
    }
}

