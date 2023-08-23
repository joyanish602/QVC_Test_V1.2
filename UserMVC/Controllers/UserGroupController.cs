using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using UserMVC.Models;
using X.PagedList;

namespace UserMVC.Controllers
{
    public class UserGroupController : Controller
    {
        HttpClientHandler _clientHandler = new HttpClientHandler();
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public UserGroupController(IConfiguration config)
        {
            _config = config;
            _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };
            _client = new HttpClient(_clientHandler);
            _client.BaseAddress = new Uri(_config.GetValue<string>("WebAPIUrl"));


        }
        public IActionResult Index(int? page)
        {
            List<UserGroup> userGroups = new List<UserGroup>();

            HttpResponseMessage resMessage = _client.GetAsync(_client.BaseAddress + "/userGroups").Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string resData = resMessage.Content.ReadAsStringAsync().Result;
                userGroups = JsonConvert.DeserializeObject<List<UserGroup>>(resData);
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                return View(userGroups.ToPagedList(page ?? 1, 5));
            }

            List<User> users = new List<User>();
            resMessage = _client.GetAsync(_client.BaseAddress + "/users").Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string resData = resMessage.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(resData);
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                return View(userGroups.ToPagedList(page ?? 1, 5));
            }

            foreach (var userGrp in userGroups)
            {
                var ids = userGrp.GroupMembersIds.Split(',').Select(s => int.Parse(s));
                var tmpUsers = users.Where(x => ids.Contains(x.Id)).ToList();
                userGrp.GroupMembersNames = string.Join(",", tmpUsers.Select(x => $"{x.FirstName + ' ' + x.LastName}"));
            }

            return View(userGroups.ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        public IActionResult Create()
        {
            UserGroupResp userGroupResp = new UserGroupResp();

            List<User> users = new List<User>();
            HttpResponseMessage resMessage = _client.GetAsync(_client.BaseAddress + "/users").Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string resData = resMessage.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(resData);
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                userGroupResp.GroupMembersIds = new List<long>().ToArray();
                return View(userGroupResp);
            }

            userGroupResp.GroupMembers = users.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();

            return View(userGroupResp);
        }

        [HttpPost]
        public IActionResult Create(UserGroupResp userGroup)
        {
            try
            {
                UserGroup userGrp = new UserGroup();
                userGrp.GroupName = userGroup.GroupName;
                userGrp.GroupMembersIds = string.Join(",", userGroup.GroupMembersIds);
                string data = JsonConvert.SerializeObject(userGrp);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                
                HttpResponseMessage resMessage = _client.PostAsync(_client.BaseAddress + "/userGroups", content).Result;
                if (resMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User group created successfully";
                    return RedirectToAction("Index", "UserGroup");
                }
                else
                {
                    TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                    return RedirectToAction("Create", "UserGroup");
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Create", "UserGroup");
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            UserGroupResp userGrpResp = new UserGroupResp();

            UserGroup userGroup = new UserGroup();
            
            HttpResponseMessage resMessage = _client.GetAsync(_client.BaseAddress + "/userGroups/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string resData = resMessage.Content.ReadAsStringAsync().Result;
                userGroup = JsonConvert.DeserializeObject<UserGroup>(resData);
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                userGrpResp.GroupMembersIds = new List<long>().ToArray();
                return View(userGrpResp);
            }

            List<User> users = new List<User>();
            resMessage = _client.GetAsync(_client.BaseAddress + "/users").Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string resData = resMessage.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(resData);
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                userGrpResp.GroupMembersIds = new List<long>().ToArray();
                return View(userGrpResp);
            }

            List<long> groupMembersIds = new List<long>();
            userGroup.GroupMembersIds.Split(",").ToList().ForEach(x => groupMembersIds.Add(long.Parse(x)));

            userGrpResp.Id = userGroup.Id;
            userGrpResp.GroupName = userGroup.GroupName;
            userGrpResp.GroupMembers = users.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).ToList();
            userGrpResp.GroupMembersIds = groupMembersIds.ToArray();

            return View(userGrpResp);
        }

        [HttpPost]
        public IActionResult Edit(UserGroupResp userGroup)
        {
            try
            {
                UserGroup userGrp = new UserGroup();
                userGrp.Id = userGroup.Id;
                userGrp.GroupName = userGroup.GroupName;
                userGrp.GroupMembersIds = string.Join(",", userGroup.GroupMembersIds);
                string data = JsonConvert.SerializeObject(userGrp);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage resMessage = _client.PutAsync(_client.BaseAddress + "/userGroups", content).Result;
                if (resMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User group updated successfully";
                    return RedirectToAction("Index", "UserGroup");
                }
                else
                {
                    TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                    userGroup.GroupMembersIds = new List<long>().ToArray();
                    return View(userGroup);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(userGroup);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            UserGroup userGroup = new UserGroup();

            HttpResponseMessage resMessage = _client.GetAsync(_client.BaseAddress + "/userGroups/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string resData = resMessage.Content.ReadAsStringAsync().Result;
                userGroup = JsonConvert.DeserializeObject<UserGroup>(resData);
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                return View(userGroup);
            }

            List<User> users = new List<User>();
            resMessage = _client.GetAsync(_client.BaseAddress + "/users").Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string resData = resMessage.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<User>>(resData);
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                return View(userGroup);
            }

            var ids = userGroup.GroupMembersIds.Split(',').Select(s => int.Parse(s));
            users = users.Where(x => ids.Contains(x.Id)).ToList();
            userGroup.GroupMembersNames = string.Join(",", users.Select(x => $"{x.FirstName + ' ' + x.LastName}"));
            return View(userGroup);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            HttpResponseMessage resMessage = _client.DeleteAsync(_client.BaseAddress + "/userGroups/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "User group deleted successfully";
                return RedirectToAction("Index", "UserGroup");
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                return View();
            }
        }
    }
}
