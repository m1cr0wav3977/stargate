using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Commands.Person
{
    public class DeletePersonHandlerTests
    {
        [Fact]
        public async Task Handle_WhenPersonExists_ShouldDeletePerson()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            
            var handler = new DeletePersonHandler(context);
            var request = new DeletePerson { Id = person.Id };

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Id.Should().Be(person.Id);
            var deletedPerson = await context.People.FirstOrDefaultAsync(p => p.Id == person.Id);
            deletedPerson.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenPersonDoesNotExist_ShouldThrowBadHttpRequestException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var handler = new DeletePersonHandler(context);
            var request = new DeletePerson { Id = 999 };

            // Act
            var act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("Bad Request");
        }

        [Fact]
        public async Task Handle_WhenPersonHasAstronautDetail_ShouldCascadeDelete()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            await TestDbContextHelper.CreateTestAstronautDetailAsync(context, person.Id);
            
            var handler = new DeletePersonHandler(context);
            var request = new DeletePerson { Id = person.Id };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var deletedPerson = await context.People.FirstOrDefaultAsync(p => p.Id == person.Id);
            deletedPerson.Should().BeNull();
            
            var deletedDetail = await context.AstronautDetails.FirstOrDefaultAsync(d => d.PersonId == person.Id);
            deletedDetail.Should().BeNull();
        }

        [Fact]
        public async Task Handle_WhenPersonHasAstronautDuties_ShouldCascadeDelete()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            await TestDbContextHelper.CreateTestAstronautDutyAsync(context, person.Id);
            await TestDbContextHelper.CreateTestAstronautDutyAsync(context, person.Id, "CAPT", "Commander", DateTime.UtcNow.AddDays(30));
            
            var handler = new DeletePersonHandler(context);
            var request = new DeletePerson { Id = person.Id };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var deletedPerson = await context.People.FirstOrDefaultAsync(p => p.Id == person.Id);
            deletedPerson.Should().BeNull();
            
            var deletedDuties = await context.AstronautDuties.Where(d => d.PersonId == person.Id).ToListAsync();
            deletedDuties.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_WhenPersonHasAllRelatedEntities_ShouldCascadeDeleteAll()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var person = await TestDbContextHelper.CreateTestPersonAsync(context, "Test Person");
            await TestDbContextHelper.CreateTestAstronautDetailAsync(context, person.Id);
            await TestDbContextHelper.CreateTestAstronautDutyAsync(context, person.Id);
            
            var handler = new DeletePersonHandler(context);
            var request = new DeletePerson { Id = person.Id };

            // Act
            await handler.Handle(request, CancellationToken.None);

            // Assert
            var deletedPerson = await context.People.FirstOrDefaultAsync(p => p.Id == person.Id);
            deletedPerson.Should().BeNull();
            
            var deletedDetail = await context.AstronautDetails.FirstOrDefaultAsync(d => d.PersonId == person.Id);
            deletedDetail.Should().BeNull();
            
            var deletedDuties = await context.AstronautDuties.Where(d => d.PersonId == person.Id).ToListAsync();
            deletedDuties.Should().BeEmpty();
        }
    }
}

