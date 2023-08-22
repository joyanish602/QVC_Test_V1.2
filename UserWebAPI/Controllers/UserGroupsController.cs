using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserWebAPI.Model;

namespace UserWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupsController : ControllerBase
    {
        private readonly UserContext _userContext;
        public UserGroupsController(UserContext userContext)
        {
            this._userContext = userContext;
        }

        [HttpGet]
        public IActionResult GetUserGroups()
        {
            try
            {
                var userGroups = _userContext.userGroups.ToList();
                if (userGroups.Count == 0)
                {
                    return NotFound("User group not found");
                }
                return Ok(userGroups);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUserGroup(int id)
        {
            try
            {
                var userGroup = _userContext.userGroups.Find(id);
                if (userGroup == null)
                {
                    return NotFound("User group not found");
                }
                return Ok(userGroup);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddUserGroup(UserGroups userGroups)
        {
            try
            {
                if (userGroups == null)
                {
                    return BadRequest("Invalid data");
                }
                else if (_userContext.userGroups.Where(x => x.Id != userGroups.Id && x.GroupName == userGroups.GroupName).ToList().Count > 0)
                {
                    return BadRequest("Group name already exists");
                }
                _userContext.userGroups.Add(userGroups);
                _userContext.SaveChanges();
                return Ok("User group added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateUserGroup(UserGroups userGroup)
        {
            if (userGroup == null || userGroup.Id == 0)
            {
                return BadRequest("Invalid data");
            }
            else if (_userContext.userGroups.Where(x => x.Id != userGroup.Id && x.GroupName == userGroup.GroupName).ToList().Count > 0)
            {
                return BadRequest("Group name already exists");
            }

            try
            {
                var _userGroup = _userContext.userGroups.Find(userGroup.Id);
                if (_userGroup == null)
                {
                    return NotFound("User group not found");
                }
                
                _userGroup.GroupName = userGroup.GroupName;
                _userGroup.GroupMembersIds = userGroup.GroupMembersIds;
                //_userContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _userContext.SaveChanges();
                return Ok("User group updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUserGroup(int id)
        {
            try
            {
                var userGroup = _userContext.userGroups.Find(id);
                if (userGroup == null)
                {
                    return NotFound("User not found");
                }
                _userContext.userGroups.Remove(userGroup);
                _userContext.SaveChanges();
                return Ok("User group deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
