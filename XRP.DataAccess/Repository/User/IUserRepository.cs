using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRP.Domain.DTO;
using XRP.Domain.Entity;

namespace XRP.DataAccess.Repository.User
{
    public interface IUserRepository
    {
        Task<List<Users>> GetUserListAsync();

        Task<List<Users>> GetUserListAsyncActive();

        Task<Users> UserLogin(LoginDto loginDto);

        Task<bool> RegisterUsers(RegisterDto registerDto);

        Task<bool> CheckUserContact(string contact);

        Task<Users> GetUserById(int userId);

        Task<BookingAllocations> GetBookingAllocationsByUserId(int userId);

        Task<bool> BookOrder(BookingAllocations allocation);
        Task<bool> UdpateUserInfo(Users user,BookingAllocations allocation);
        Task<bool> UdpateUserInfoUserNameAndPassword(int userId, bool? gender, string? password);
        Task<BookingAllocations> GetBookingAllocationsById(int userId);

        Task<List<BookingAllocations>> GetBookingAllocationsByUserId1(int userId);
        Task<bool> BookOrderNew(int bookingAllocationId, int userId);
        Task<decimal> TodayCommision(int userId);
        Task<bool> ReduceUserBalanace(int userId, decimal bookingAmount);
        Task<List<UsersHistory>> GetUserHistoryByUserId(int userId);
        Task<AdminUsers> UserLoginAdmin(string username, string password);
        Task<bool> ActivateUser(int userId);
        Task<bool> Deposit(int userId, decimal depositAmount);
        Task<bool> Reduce(int userId, decimal depositAmount);
        Task<bool> CreaditScore(int userId, decimal score);

        Task<bool> Benifit(int userId, decimal score);
        Task<bool> Pop(int userId, string popMessage);
        Task<bool> Reset(int userId);
        Task<List<BookingAllocations>> GetBookingAllocationsListByUserId(int userId);
        Task<bool> UpdateAllocations(List<UpdateAllocation> request);
        Task<bool> LockUser(int userId);
        Task<bool> ResetPassword(int userId,string password);
        Task<bool> IsUserHasBank(int userId);
        Task<bool> WithdrawalWithBank(int userId, WithdrawalPostDto responce);

        Task<bool> WithdrawalWithoutBank(int userId, WithdrawalWithoutBankDto responce);

        Task<List<Withdrawal>> GetUserListAsyncWithBankActive();

        Task<bool> Rejected(int withdrawalId);
        Task<bool> Approved(int withdrawalId);

        Task<List<Withdrawal>> GetWithdrawals(int userId);

    }
    
}
