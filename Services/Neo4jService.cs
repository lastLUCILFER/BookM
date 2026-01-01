using Neo4j.Driver;

namespace BookM.Services
{
    public class Neo4jService
    {
        private readonly IDriver _driver;

        public Neo4jService(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<string> GetServerVersionAsync()
        {
            await using var session = _driver.AsyncSession();
            // This query returns the Neo4j DBMS version
            var result = await session.ExecuteReadAsync(async tx =>
            {
                var cursor = await tx.RunAsync("CALL dbms.components() YIELD name, versions, edition RETURN name, versions, edition");
                return await cursor.FetchAsync() 
                    ? $"{cursor.Current["name"]} {((List<object>)cursor.Current["versions"]).FirstOrDefault()} ({cursor.Current["edition"]})" 
                    : "Unknown";
            });

            return result;
        }

      
        public async Task BatchCreateUsersAsync(List<Dictionary<string, object>> users)
        {
            await using var session = _driver.AsyncSession();

            // UNWIND processes the list of users efficiently in one query
            var query = @"
                UNWIND $users AS user
                MERGE (u:User {id: user.UserId})
                SET u.email = user.Email, 
                    u.username = user.Username,
                    u.passwordHash = user.PasswordHash,
                    u.isAdmin = user.IsAdmin,
                    u.migratedAt = datetime()";

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { users });
            });
        }

        public async Task CreateUserAsync(BookM.Models.User user)
        {
            await using var session = _driver.AsyncSession();

            var query = @"
                MERGE (u:User {id: $UserId})
                SET u.email = $Email, 
                    u.username = $Name,
                    u.passwordHash = $PasswordHash,
                    u.isAdmin = $IsAdmin,
                    u.createdAt = datetime()";

            var parameters = new
            {
                user.UserId,
                user.Email,
                user.Name,
                user.PasswordHash,
                user.IsAdmin
            };

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, parameters);
            });
        }

        public async Task BatchCreateEventsAsync(List<Dictionary<string, object>> events)
        {
            await using var session = _driver.AsyncSession();

            var query = @"
                UNWIND $events AS eventData
                MERGE (c:Category {id: eventData.CategoryId})
                SET c.name = eventData.CategoryName
                
                MERGE (e:Event {id: eventData.EventId})
                SET e.title = eventData.Title,
                    e.description = eventData.Description,
                    e.date = datetime(eventData.EventDate),
                    e.location = eventData.Location,
                    e.capacity = eventData.Capacity,
                    e.imageUrl = eventData.ImageUrl
                
                MERGE (e)-[:BELONGS_TO]->(c)
            ";

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { events });
            });
        }

        public async Task BatchCreateCategoriesAsync(List<Dictionary<string, object>> categories)
        {
            await using var session = _driver.AsyncSession();

            var query = @"
                UNWIND $categories AS category
                MERGE (c:Category {id: category.CategoryId})
                SET c.name = category.Name
                
                WITH c, category
                UNWIND category.EventIds AS eventId
                MERGE (e:Event {id: eventId})
                MERGE (e)-[:BELONGS_TO]->(c)
            ";

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { categories });
            });
        }

        public async Task CreateCategoryAsync(BookM.Models.Category category)
        {
            await using var session = _driver.AsyncSession();

            var query = @"
                MERGE (c:Category {id: $CategoryId})
                SET c.name = $Name
            ";

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { category.CategoryId, category.Name });
            });
        }

        public async Task CreateEventAsync(BookM.Models.Event @event)
        {
            await using var session = _driver.AsyncSession();

            var query = @"
                MERGE (c:Category {id: $CategoryId})
                MERGE (e:Event {id: $EventId})
                SET e.title = $Title,
                    e.description = $Description,
                    e.date = datetime($EventDate),
                    e.location = $Location,
                    e.capacity = $Capacity,
                    e.imageUrl = $ImageUrl
                MERGE (e)-[:BELONGS_TO]->(c)
            ";

            var parameters = new
            {
                @event.EventId,
                @event.Title,
                Description = @event.Description ?? "",
                EventDate = @event.EventDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                Location = @event.Location ?? "",
                @event.Capacity,
                ImageUrl = @event.ImageUrl ?? "",
                @event.CategoryId
            };

            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, parameters);
            });
        }
    }



       
}
