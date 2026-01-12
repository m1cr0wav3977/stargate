using FluentAssertions;
using Microsoft.AspNetCore.Http;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;
using StargateAPI.Tests.Helpers;
using Xunit;

namespace StargateAPI.Tests.Business.Commands.Person
{
    public class CreatePersonPreProcessorTests
    {
        [Fact]
        public async Task Process_WhenPersonDoesNotExist_ShouldNotThrowException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            var processor = new CreatePersonPreProcessor(context);
            var request = new CreatePerson { Name = "New Person" };

            // Act
            var act = async () => await processor.Process(request, CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task Process_WhenPersonWithSameNameExists_ShouldThrowBadHttpRequestException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            await TestDbContextHelper.CreateTestPersonAsync(context, "Existing Person");
            
            var processor = new CreatePersonPreProcessor(context);
            var request = new CreatePerson { Name = "Existing Person" };

            // Act
            var act = async () => await processor.Process(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadHttpRequestException>()
                .WithMessage("Bad Request");
        }

        [Fact]
        public async Task Process_WhenPersonWithDifferentNameExists_ShouldNotThrowException()
        {
            // Arrange
            using var context = TestDbContextHelper.CreateInMemoryContext();
            await TestDbContextHelper.CreateTestPersonAsync(context, "Existing Person");
            
            var processor = new CreatePersonPreProcessor(context);
            var request = new CreatePerson { Name = "Different Person" };

            // Act
            var act = async () => await processor.Process(request, CancellationToken.None);

            // Assert
            await act.Should().NotThrowAsync();
        }
    }
}

