using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text;
using UserMVC.Models;
using X.PagedList;

namespace UserMVC.Controllers
{
    public class UserController : Controller
    {
        HttpClientHandler _clientHandler = new HttpClientHandler();
        private readonly IConfiguration _config;
        private readonly HttpClient _client;
        
        public UserController(IConfiguration config)
        {
            _config = config;
            _clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; };
            _client = new HttpClient(_clientHandler);
            _client.BaseAddress = new Uri(_config.GetValue<string>("WebAPIUrl"));
        }

        [HttpGet]
        public IActionResult Index(int? page)
        {
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
                return View(users.ToPagedList(page ?? 1, 5));
            }
            return View(users.ToPagedList(page ?? 1, 5));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            try
            {
                string data = JsonConvert.SerializeObject(user);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                
                HttpResponseMessage resMessage = _client.PostAsync(_client.BaseAddress + "/users", content).Result;
                if (resMessage.IsSuccessStatusCode)
                {
                    TempData["successMessage"] = "User saved successfully";
                    return RedirectToAction("Index","User");
                }
                else
                {
                    TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            User user = new User();

            HttpResponseMessage resMessage = _client.GetAsync(_client.BaseAddress + "/users/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string resData = resMessage.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(resData);
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                return View(user);
            }
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User user)
        {
            string data = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            
            HttpResponseMessage resMessage = _client.PutAsync(_client.BaseAddress + "/users", content).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "User details updated successfully";
                return RedirectToAction("Index", "User");
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                return View(user);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            User user = new User();
            
            HttpResponseMessage resMessage = _client.GetAsync(_client.BaseAddress + "/users/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                string resData = resMessage.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<User>(resData);
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                return View(user);
            }
            return View(user);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            HttpResponseMessage resMessage = _client.DeleteAsync(_client.BaseAddress + "/users/" + id).Result;
            if (resMessage.IsSuccessStatusCode)
            {
                TempData["successMessage"] = "User deleted successfully";
                return RedirectToAction("Index", "User");
            }
            else
            {
                TempData["errorMessage"] = resMessage.Content.ReadAsStringAsync().Result;
                return View();
            }
        }
    }
}
