using Microsoft.AspNetCore.Mvc;
using CoreAPIDemo.ViewModel;
using CoreAPIDemo.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CoreAPIDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SQLController : ControllerBase
    {
        #region Field
        private readonly ILogger<SQLController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        #endregion

        #region Ctor
        public SQLController(ILogger<SQLController> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
          
        }
        #endregion

        #region CRUD Methods SQL

        // CREATE: Create a new user
        [HttpPost("Create")]
        public async Task<IActionResult> Create(UserViewModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return StatusCode(500, "Please Add Required Fields");
               
                // Prepare user entity
                var userEntity = new EntityModels.User();

                // Map properties from view model to entity
                MapUserViewModelToEntity(user, userEntity);

                await _applicationDbContext.Users.AddAsync(userEntity);
                await _applicationDbContext.SaveChangesAsync();

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
        public async Task<IActionResult> Update(UserViewModel user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return StatusCode(500, "Please Add Required Fields");

                // Check if the user exists
                var userEntity = await _applicationDbContext.Users.FindAsync(user.Id);
                if (userEntity == null)
                    return StatusCode(500, "User not found");

                // Update user entity
                MapUserViewModelToEntity(user, userEntity);

                _applicationDbContext.Users.Update(userEntity);
                await _applicationDbContext.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: Delete a user
        [HttpPost("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0)
                    return StatusCode(500, "User ID not provided");

                var userEntity = await _applicationDbContext.Users.FindAsync(id);
                if (userEntity == null)
                    return StatusCode(500, "User not found");

                _applicationDbContext.Users.Remove(userEntity);
                await _applicationDbContext.SaveChangesAsync();

                return Ok("User Successfully Removed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        // READ: Get a user by ID
        [HttpPost("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id == 0)
                    return StatusCode(500, "User ID not provided");

                var userEntity = await _applicationDbContext.Users.FindAsync(id);
                if (userEntity == null)
                    return StatusCode(500, "User not found");

                return Ok(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(500, ex.Message);
            }
        }

        // READ: Get the list of all users
        [HttpGet("List")]
        public async Task<IActionResult> GetListAsync()
        {
            var users = await _applicationDbContext.Users.ToListAsync();
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
