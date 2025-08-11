using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using XRP.Models;
using XRP.Services.User;
using XRP.Domain.DTO;
using XRP.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Net;

namespace XRP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUsersService _usersService;

        public HomeController(ILogger<HomeController> logger, IUsersService usersService)
        {
            _logger = logger;
            _usersService= usersService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("UId");

            if (userId is not null)
            {
                var user = await _usersService.GetUserById(int.Parse(userId!));
                var todayCommision = await _usersService.TodayCommision(int.Parse(userId));

                user.TodayCommission = todayCommision;
                return View(user);
            }
            else
            {
                Users user = new Users();
                return View(user);
            }
            
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginDto request)
        {
            try
            {
                //using (WebClient client = new WebClient())
                //{
                //    string flag = client.DownloadString("https://raw.githubusercontent.com/XXXXXXXXXXXXXX/xpt/main/hash.txt").Trim();
                //    if (flag != "e8965-4768a-e8965-4768axx")
                //    {
                //        return Json(new { success = false, message = "System is in maintenance mode" });
                //    }
                //}
            
                if (request == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }

                var user = await _usersService.UserLogin(request);
                if (user != null)
                {
                    // Set claims
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Contact),
                    new Claim(ClaimTypes.NameIdentifier, user.UId.ToString()),
                    new Claim("LoggedInAt", DateTime.UtcNow.ToString("o"))
                };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    HttpContext.Session.SetString("UId", user.UId.ToString());
                    HttpContext.Session.SetString("UserName", user.UserName.ToString());
                    HttpContext.Session.SetString("Benifit", (user.MemberBenifit ?? 0).ToString());

                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                }
                else
                {

                    return Json(new { success = false, message = "Invalid mobile number or password." });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message + "----" + ex.InnerException });
            }
           
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { success = false, message = "Invalid request." });
                }
                if (request.InvitaionCode.Trim() != "AGENT_2025_MAERSKT1_ELDF" && request.InvitaionCode.Trim() != "AGENT_2025_MAERSKT2S_ELDF")
                {
                    return Json(new { success = false, message = "Invalid invitation code." });
                }

                else
                {

                    if (await _usersService.CheckUserContact(request.MobileNumber.Trim()))
                    {
                        return Json(new { success = false, redirectUrl = Url.Action("Index", "Home"), message = "Mobile Number exist in the Application" });
                    }
                    if (request.ConfPassword.Trim() == request.Password.Trim())
                    {
                        bool isRegisterd = await _usersService.RegisterUsers(request);
                        if (isRegisterd)
                        {
                            return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                        }
                        else 
                        {
                            return Json(new { success = false, redirectUrl = Url.Action("Index", "Home"), message = "Oops! Registration failed"});
                        }                       
                    }
                    else
                    {
                        return Json(new { success = false, redirectUrl = Url.Action("Index", "Home"), message = "Oops! Both password fields must match. Please check and try again" });
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error - bad request" });
            }
           
        }

        public async Task<IActionResult> Logout()
        {
            // 🔥 Clear session
            HttpContext.Session.Clear();

            // 🔐 Sign the user out from the cookie scheme
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // 🚀 Redirect to login page or homepage
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[Authorize]
        public IActionResult About()
        {
            return View();
        }

        [Authorize]
        public IActionResult Work()
        {
            return View();
        }

        [Authorize]
        public IActionResult Contact()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Wallet()
        {
            var userId = HttpContext.Session.GetString("UId");

            if (userId is not null)
            {
                var user = await _usersService.GetUserById(int.Parse(userId!));
                var allocations = await _usersService.GetBookingAllocationsByUserId1(int.Parse(userId!));

                int allocationCount = allocations.Count / 18;
                int remainder = allocations.Count % 18;

                if (remainder > 0) {
                    user.Level = "True";
                } else
                {
                    user.Level = "False";
                }

                var todayCommision = await _usersService.TodayCommision(int.Parse(userId));

                user.TodayCommission = todayCommision;
                return View(user);
            }
            else
            {
                Users user = new Users();
                return View(user);
            }
        }

        [Authorize]
        public async Task<IActionResult> Withdrawals()
        {
            var userId = HttpContext.Session.GetString("UId");

            if (userId is not null)
            {
                var user = await _usersService.GetUserById(int.Parse(userId!));
                var todayCommision = await _usersService.TodayCommision(int.Parse(userId));

                user.TodayCommission = todayCommision;

                var widthdrawal = await _usersService.GetWithdrawals(int.Parse(userId!));
                HttpContext.Session.SetString("Withdrawals", JsonConvert.SerializeObject(widthdrawal));
                return View(user);
            }
            else
            {
                Users user = new Users();
                return View(user);
            }
        }

        [Authorize]
        public async Task<JsonResult> IsUserHasBank([FromBody] RegisterDto request)
        {
            
            var userId = HttpContext.Session.GetString("UId");

            if (userId is not null)
            {
                var user = await _usersService.IsUserHasBank(int.Parse(userId!));

                return Json(new { success = true, hasbank = user });
            }
            else
            {
                return Json(new { success = false, hasbank = false });
            }
        }

        [Authorize]
        public IActionResult Help()
        {
            return View();
        }

        [Authorize]
        public IActionResult Booking()
        {
            var userId = HttpContext.Session.GetString("UId");
            if (userId is  null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpGet]
        public async Task<JsonResult> GetBookings()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UId");

                if (userId is not null)
                {
                    var booking = await _usersService.GetBookingAllocationsByUserId(int.Parse(userId));
                    var user = await _usersService.GetUserById(int.Parse(userId));
                    var todayCommision = await _usersService.TodayCommision(int.Parse(userId));


                    user.TodayCommission = todayCommision;
                    if (booking is not null)
                    {
                        if (user.TotalBalance >= booking.Price && user.TotalBalance>=0)
                        {
                            return Json(new { success = true, message = "Booking Found", data = booking, users = user });
                        }
                        else
                        {
                            if (user.TotalBalance >= 0)
                            {
                                //Minus user balance 
                                await _usersService.ReduceUserBalanace(int.Parse(userId), booking.Price);
                            }
                            
                            return Json(new { success = false, message = "Total balance is not enough", data = "No Booking", users = user });
                        }
                        
                    }
                    else 
                    {
                        return Json(new { success = false, message = "No Booking Allocated", data = "No Booking", users= user });
                    }
                    
                }
                else
                {
                    return Json(new { success = false, message = "Server Error" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error - bad request" });
            }

        }


        [HttpPost]
        public async Task<JsonResult> BookOrder([FromBody] BookOrderDto request)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UId");
                var isBooked = await _usersService.BookOrderNew(request.BookingAllocationId, int.Parse(userId!));
                if (isBooked && userId is not null)
                {
                   return Json(new { success = true, message = "Booked", data = "Booked" });
 
                }
                else
                {
                    return Json(new { success = false, message = "Server Error" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error - bad request" });
            }

        }


        [HttpPost]
        public async Task<JsonResult> WithdrawalWithBank([FromBody] WithdrawalPostDto request)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UId");
                var isBooked = await _usersService.WithdrawalWithBank(int.Parse(userId!), request);
                if (isBooked && userId is not null)
                {
                    return Json(new { success = true, message = "Booked", data = "Withdrawed" });

                }
                else
                {
                    return Json(new { success = false, message = "Server Error" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [HttpPost]
        public async Task<JsonResult> WithdrawalWithOutBank([FromBody] WithdrawalWithoutBankDto request)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UId");
                var isBooked = await _usersService.WithdrawalWithoutBank(int.Parse(userId!), request);
                if (isBooked && userId is not null)
                {
                    return Json(new { success = true, message = "Booked", data = "Withdrawed" });

                }
                else
                {
                    return Json(new { success = false, message = "Server Error" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [Authorize]
        public IActionResult MyAccount()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> BookingHistory()
        {
            var userId = HttpContext.Session.GetString("UId");
            var historyList = await _usersService.GetUserHistoryByUserId(int.Parse(userId!));
            return View(historyList);
        }

        [HttpGet]
        public async Task<JsonResult> GetUserInfo()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UId");

                if (userId is not null)
                {
                    var user = await _usersService.GetUserById(int.Parse(userId));
                    if (user is not null)
                    {
                        return Json(new { success = true, message = "Booking Found", user = user });
                    }
                    else
                    {
                        return Json(new { success = false, message = "No Booking Allocated", data = "No Booking", user = user });
                    }

                }
                else
                {
                    return Json(new { success = false, message = "Server Error" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error - bad request" });
            }

        }


        [HttpPost]
        public async Task<JsonResult> UdpateUserInfoUserNameAndPassword([FromBody] UserUpdateDto request)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UId");
                var isUpdated = await _usersService.UdpateUserInfoUserNameAndPassword(int.Parse(userId!), request.Gender, request.Password);
                if (isUpdated && userId is not null)
                {
                    return Json(new { success = true, message = "updated", data = "updated" });

                }
                else
                {
                    return Json(new { success = false, message = "Server Error" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error - bad request" });
            }

        }

        [Authorize]
        public async Task<IActionResult> Level()
        {
            var userId = HttpContext.Session.GetString("UId");

            if (userId is not null)
            {
                var user = await _usersService.GetUserById(int.Parse(userId!));
                var todayCommision = await _usersService.TodayCommision(int.Parse(userId));

                user.TodayCommission = todayCommision;
                return View(user);
            }
            else
            {
                Users user = new Users();
                return View(user);
            }
            
        }


        [Authorize]
        public async Task<IActionResult> MembershipBenifits()
        {
            return View();
        }
    }
}
