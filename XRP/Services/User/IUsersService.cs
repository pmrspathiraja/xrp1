using XRP.Domain.DTO;
using XRP.Domain.Entity;

namespace XRP.Services.User
{
    public interface IUsersService
    {
        Task<List<Users>> GetUserListAsync();

        Task<Users> UserLogin(LoginDto loginDto);

        Task<bool> RegisterUsers(RegisterDto registerDto);
        Task<bool> CheckUserContact(string contact);
        Task<Users> GetUserById(int userId);
        Task<BookingAllocations> GetBookingAllocationsByUserId(int userId);
        Task<List<BookingAllocations>> GetBookingAllocationsByUserId1(int userId);
        Task<bool> BookOrder(int bookingAllocationId, int userId);
        Task<bool> BookOrderNew(int bookingAllocationId, int userId);
        Task<bool> UdpateUserInfoUserNameAndPassword(int userId, bool? gender, string? password);
        Task<bool> ReduceUserBalanace(int userId, decimal bookingAmount);
        Task<decimal> TodayCommision(int userId);
        Task<List<UsersHistory>> GetUserHistoryByUserId(int userId);
        Task<bool> IsUserHasBank(int userId);
        Task<bool> WithdrawalWithBank(int userId, WithdrawalPostDto responce);
        Task<bool> WithdrawalWithoutBank(int userId, WithdrawalWithoutBankDto responce);
        Task<List<Withdrawal>> GetWithdrawals(int userId);
    }
}
