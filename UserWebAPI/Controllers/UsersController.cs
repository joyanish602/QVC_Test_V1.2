using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserWebAPI.Model;

namespace UserWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _userContext;
        public UsersController(UserContext userContext)
        {
            this._userContext = userContext;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _userContext.users.ToList();
                if (users.Count == 0)
                {
                    return NotFound("User not found");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                var users = _userContext.users.Find(id);
                if (users == null)
                {
                    return NotFound("User not found");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddUser(Users user)
        {
            try
            {
                if (user == null)
                {
                    return BadRequest("Invalid data");
                }
                else if (_userContext.users.Where(x => x.Email == user.Email).ToList().Count > 0)
                {
                    return BadRequest("Email already exists");
                }
                _userContext.users.Add(user);
                _userContext.SaveChanges();
                return Ok("User added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateUser(Users user)
        {
            if(user == null || user.Id == 0)
            {
                return BadRequest("Invalid data");
            }
            else if (_userContext.users.Where(x => x.Id != user.Id && x.Email == user.Email).ToList().Count > 0)
            {
                return BadRequest("Email already exists");
            }
            try
            {
                var _user = _userContext.users.Find(user.Id);
                if (_user == null)
                {
                    return NotFound("User not found");
                }
                _user.FirstName = user.FirstName;
                _user.LastName = user.LastName;
                _user.Age = user.Age;
                _user.Email = user.Email;
                //_userContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _userContext.SaveChanges();
                return Ok("User updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _userContext.users.Find(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }
                _userContext.users.Remove(user);
                _userContext.SaveChanges();
                return Ok("User deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
