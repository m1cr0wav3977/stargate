using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;

namespace StargateAPI.Tests.Helpers
{
    public static class TestDbContextHelper
    {
        public static StargateContext CreateInMemoryContext(string? databaseName = null)
        {
            // Use temporary file-based SQLite for Dapper compatibility
            // This works better than in-memory for Dapper queries
            var dbName = databaseName ?? $"{Guid.NewGuid()}.db";
            var connectionString = $"Data Source={dbName}";
            var options = new DbContextOptionsBuilder<StargateContext>()
                .UseSqlite(connectionString)
                .Options;

            var context = new StargateContext(options);
            context.Database.EnsureCreated();
            
            return context;
        }

        public static async Task<Person> CreateTestPersonAsync(StargateContext context, string name = "TestPerson")
        {
            var person = new Person { Name = name };
            context.People.Add(person);
            await context.SaveChangesAsync();
            return person;
        }

        public static async Task<AstronautDetail> CreateTestAstronautDetailAsync(
            StargateContext context, 
            int personId, 
            string rank = "CPT", 
            string dutyTitle = "Spaceman")
        {
            var detail = new AstronautDetail
            {
                PersonId = personId,
                CurrentRank = rank,
                CurrentDutyTitle = dutyTitle,
                CareerStartDate = DateTime.UtcNow.Date
            };
            context.AstronautDetails.Add(detail);
            await context.SaveChangesAsync();
            return detail;
        }

        public static async Task<AstronautDuty> CreateTestAstronautDutyAsync(
            StargateContext context,
            int personId,
            string rank = "CPT",
            string dutyTitle = "Spaceman",
            DateTime? dutyStartDate = null)
        {
            var duty = new AstronautDuty
            {
                PersonId = personId,
                Rank = rank,
                DutyTitle = dutyTitle,
                DutyStartDate = dutyStartDate ?? DateTime.UtcNow.Date,
                DutyEndDate = null
            };
            context.AstronautDuties.Add(duty);
            await context.SaveChangesAsync();
            return duty;
        }
    }
}

