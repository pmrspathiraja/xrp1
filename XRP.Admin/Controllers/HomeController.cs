using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using XRP.Admin.Models;
using XRP.DataAccess.Repository.User;
using XRP.Domain.Entity;
using XRP.Models;
using XRP.Domain.DTO;
using Microsoft.AspNetCore.Authorization;


namespace XRP.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserRepository _userRepository;

        public HomeController(ILogger<HomeController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }
        [Authorize]
        public async Task<IActionResult> UsersList()
        {
            var userList = await _userRepository.GetUserListAsync();
            return View(userList);
        }

        [Authorize]
        public async Task<IActionResult> WithdrawalsAdmin()
        {
            var withdrawalsList = await _userRepository.GetUserListAsyncWithBankActive();
            return View(withdrawalsList);
        }

        public async Task<IActionResult> LogoutAdmin()
        {
            // 🔥 Clear session
            HttpContext.Session.Clear();

            // 🔐 Sign the user out from the cookie scheme
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 🚀 Redirect to login page or homepage
            return RedirectToAction("IndexAdmin", "Home");
        }

        [Authorize]
        public async Task<IActionResult> UsersRequest()
        {
            var userList =  await _userRepository.GetUserListAsyncActive();
            return View(userList);
        }

        [HttpGet]
        public async Task<JsonResult> ActivateUser(int UserId)
        {
            try
            {
                if (UserId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.ActivateUser(UserId);
                if (user) 
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersRequest", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Invalid mobile number or password." });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpGet]
        public async Task<JsonResult> Deposit(int userId,decimal depositAmount)
        {
            try
            {
                if (userId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.Deposit(userId, depositAmount);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersList", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Deposit Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpGet]
        public async Task<JsonResult> Reduce(int userId, decimal reduceAmount)
        {
            try
            {
                if (userId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.Reduce(userId, reduceAmount);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersList", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Reduce Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpGet]
        public async Task<JsonResult> CreditScore(int userId, decimal score)
        {
            try
            {
                if (userId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.CreaditScore(userId, score);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersList", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Reduce Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpGet]
        public async Task<JsonResult> Benifit(int userId, decimal benifit)
        {
            try
            {
                if (userId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.Benifit(userId, benifit);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersList", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Benifit Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }


        [HttpGet]
        public async Task<JsonResult> Pop(int userId, string popMessage)
        {
            try
            {
                if (userId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.Pop(userId, popMessage);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersList", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Pop Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpGet]
        public async Task<JsonResult> Allocations(int userId)
        {
            try
            {
                if (userId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var allocation = await _userRepository.GetBookingAllocationsListByUserId(userId);
                if (allocation.Count>0)
                {
                    return Json(new { success = true, allocations = allocation });
                }
                else
                {

                    return Json(new { success = false, message = "Allocations Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpGet]
        public async Task<JsonResult> Reset(int userId)
        {
            try
            {
                if (userId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.Reset(userId);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersList", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Reset Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }


        [HttpPost]
        public async Task<JsonResult> UpdateAllocations([FromBody] List<UpdateAllocation> request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }
               
                var allocation = await _userRepository.UpdateAllocations(request);
                if (allocation)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersList", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Allocations Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpGet]
        public async Task<JsonResult> LockUser(int userId)
        {
            try
            {
                if (userId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.LockUser(userId);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersList", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Lock Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [Authorize]
        public IActionResult IndexAdmin()
        {
            return View();
        }

        public IActionResult LoginAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAdmin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var users = await _userRepository.UserLoginAdmin(model.Username, model.Password);
                if (users is not null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, users.Username),
                        new Claim(ClaimTypes.NameIdentifier, users.UId.ToString()),
                        new Claim("LoggedInAt", DateTime.UtcNow.ToString("o"))
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    HttpContext.Session.SetString("UId", users.UId.ToString());
                    HttpContext.Session.SetString("UserName", users.Username.ToString());
                    return RedirectToAction("IndexAdmin", "Home"); // or your landing page
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt");
                return View(model);
            }
            
        }

        [HttpGet]
        public async Task<JsonResult> ResetPassword(int userId, string password)
        {
            try
            {
                if (userId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }
               
                var user = await _userRepository.ResetPassword(userId, password);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("UsersList", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "reset Fail" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpGet]
        public async Task<JsonResult> Approve(int withdrawalId)
        {
            try
            {
                if (withdrawalId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.Approved(withdrawalId);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("WithdrawalsAdmin", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Invalid mobile number or password." });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpGet]
        public async Task<JsonResult> Reject(int withdrawalId)
        {
            try
            {

                if (withdrawalId == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _userRepository.Rejected(withdrawalId);
                if (user)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("WithdrawalsAdmin", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Invalid mobile number or password." });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
