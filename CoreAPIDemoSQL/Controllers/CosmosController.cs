using CoreAPIDemo.Infrastructure;
using CoreAPIDemo.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace CoreAPIDemo.Controllers
{
    public class CosmosController : Controller
    {
        #region Field

        private readonly ILogger<SQLController> _logger;
        private readonly Container _cosmosContainer;

        #endregion

        #region Ctor
        public CosmosController(ILogger<SQLController> logger,  Container cosmosContainer)
        {
            _logger = logger;
            _cosmosContainer = cosmosContainer;
        }
        #endregion

        #region CRUD Methods With Cosmos DB

        //CREATE: Create a new user
        [HttpPost("Create")]
        public async Task<IActionResult> CosmosCreate(UserViewModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return StatusCode(500, "Please Add Required Fields");

                // Prepare user entity
                var userEntity = new EntityModels.User();
                // Map properties from view model to entity
                MapUserViewModelToEntity(user, userEntity);

                await _cosmosContainer.CreateItemAsync(userEntity, new PartitionKey(userEntity.Id.ToString()));

                return Ok(userEntity.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        // UPDATE: Update an existing user
        [HttpPost("Update")]
        public async Task<IActionResult> CosmosUpdate(UserViewModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return StatusCode(500, "Please Add Required Fields");

                var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                    .WithParameter("@id", user.Id.ToString());
                var iterator = _cosmosContainer.GetItemQueryIterator<EntityModels.User>(query);

                if (iterator.HasMoreResults)
                {
                    var results = await iterator.ReadNextAsync();
                    var userEntity = results.FirstOrDefault();

                    if (userEntity == null)
                        return StatusCode(500, "User not found");

                    // Update user entity
                    MapUserViewModelToEntity(user, userEntity);

                    await _cosmosContainer.ReplaceItemAsync(userEntity, userEntity.Id.ToString(), new PartitionKey(userEntity.Id.ToString()));

                    return Ok(user);
                }

                return StatusCode(500, "User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: Delete a user
        [HttpPost("Delete")]
        public async Task<IActionResult> CosmosDelete(UserViewModel user)
        {
            try
            {
                if (user.Id == 0)
                    return StatusCode(500, "User ID not provided");

                var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                    .WithParameter("@id", user.Id.ToString());
                var iterator = _cosmosContainer.GetItemQueryIterator<EntityModels.User>(query);

                if (iterator.HasMoreResults)
                {
                    var results = await iterator.ReadNextAsync();
                    var userEntity = results.FirstOrDefault();

                    if (userEntity == null)
                        return StatusCode(500, "User not found");

                    await _cosmosContainer.DeleteItemAsync<EntityModels.User>(userEntity.Id.ToString(), new PartitionKey(userEntity.Id.ToString()));

                    return Ok("User Successfully Removed");
                }

                return StatusCode(500, "User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        // READ: Get a user by ID
        [HttpPost("GetById")]
        public async Task<IActionResult> CosmosGetById(UserViewModel user)
        {
            try
            {
                if (user.Id == 0)
                    return StatusCode(500, "User ID not provided");

                var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                    .WithParameter("@id", user.Id.ToString());
                var iterator = _cosmosContainer.GetItemQueryIterator<EntityModels.User>(query);

                if (iterator.HasMoreResults)
                {
                    var results = await iterator.ReadNextAsync();
                    var userEntity = results.FirstOrDefault();

                    if (userEntity == null)
                        return StatusCode(500, "User not found");

                    return Ok(userEntity);
                }

                return StatusCode(500, "User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        // READ: Get the list of all users
        [HttpGet("List")]
        public async Task<IActionResult> CosmosGetListAsync()
        {
            var query = new QueryDefinition("SELECT * FROM c");
            var iterator = _cosmosContainer.GetItemQueryIterator<EntityModels.User>(query);

            var users = new List<EntityModels.User>();
            while (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync();
                users.AddRange(results);
            }

            return Ok(users);
        }

        #endregion

        #region Helper Methods

        // Map properties from UserViewModel to User entity
        private void MapUserViewModelToEntity(UserViewModel source, EntityModels.User destination)
        {
            destination.Name = source.Name;
            destination.State = source.State;
            destination.DateOfBirth = source.DateOfBirth;
            destination.Email = source.Email;
            destination.City = source.City;
            destination.FullAddress = source.FullAddress;
        }

        #endregion
    }
}
