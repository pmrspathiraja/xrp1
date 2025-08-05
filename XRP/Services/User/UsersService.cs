using Azure;
using System.Reflection;
using XRP.DataAccess.Repository.User;
using XRP.Domain.DTO;
using XRP.Domain.Entity;

namespace XRP.Services.User
{
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        public UsersService(IUserRepository userRepository)
        {
            _userRepository=userRepository;
        }

        public async Task<Users> UserLogin(LoginDto loginDto)
        {
            return await _userRepository.UserLogin(loginDto);
        }

        public async Task<List<Users>> GetUserListAsync()
        {
            return await _userRepository.GetUserListAsync();
        }

        public async Task<bool> RegisterUsers(RegisterDto registerDto)
        {
            return await _userRepository.RegisterUsers(registerDto);
        }

        public async Task<Users> GetUserById(int userId)
        {
            return await _userRepository.GetUserById(userId);
        }

        public async Task<bool> CheckUserContact(string contact)
        {
            return await _userRepository.CheckUserContact(contact);
        }

        public async Task<BookingAllocations> GetBookingAllocationsByUserId(int userId)
        {
            return await _userRepository.GetBookingAllocationsByUserId(userId);
        }

        public async Task<List<BookingAllocations>> GetBookingAllocationsByUserId1(int userId)
        {
            return await _userRepository.GetBookingAllocationsByUserId1(userId);
        }


        public async Task<bool> BookOrder(int bookingAllocationId, int userId)
        {
            var allocation = await _userRepository.GetBookingAllocationsById(bookingAllocationId);
            var user = await _userRepository.GetUserById(userId);

            bool upadted = await _userRepository.BookOrder(allocation);
            if (upadted)
            {
                bool userinfo = await _userRepository.UdpateUserInfo(user, allocation);
                if (userinfo)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> BookOrderNew(int bookingAllocationId, int userId)
        {
            var responce = await _userRepository.BookOrderNew(bookingAllocationId, userId);
            return responce;
        }

        public async Task<bool> UdpateUserInfoUserNameAndPassword(int userId, bool? gender, string? password)
        {
            var responce = await _userRepository.UdpateUserInfoUserNameAndPassword(userId, gender, password);
            return responce;
        }

        public async Task<decimal> TodayCommision(int userId)
        {
            var responce = await _userRepository.TodayCommision(userId);
            return responce;
        }

        public async Task<bool> ReduceUserBalanace(int userId, decimal bookingAmount)
        {
            var responce = await _userRepository.ReduceUserBalanace(userId, bookingAmount);
            return responce;
        }

        public async Task<List<UsersHistory>> GetUserHistoryByUserId(int userId)
        {
            var responce = await _userRepository.GetUserHistoryByUserId(userId);
            return responce;
        }

        public async Task<bool> IsUserHasBank(int userId)
        {
            var responce = await _userRepository.IsUserHasBank(userId);
            return responce;
        }

        public async Task<bool> WithdrawalWithBank(int userId, WithdrawalPostDto responce)
        {
            var responcea = await _userRepository.WithdrawalWithBank(userId, responce);
            return responcea;
        }

        public async Task<bool> WithdrawalWithoutBank(int userId, WithdrawalWithoutBankDto responce)
        {
            var responcea = await _userRepository.WithdrawalWithoutBank(userId, responce);
            return responcea;
        }

        public async Task<List<Withdrawal>> GetWithdrawals(int userId)
        {
            var responcea = await _userRepository.GetWithdrawals(userId);
            return responcea;
        }
    }
}
