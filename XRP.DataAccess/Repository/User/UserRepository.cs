using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using XRP.DataAccess.Context;
using XRP.Domain.DTO;
using XRP.Domain.Entity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace XRP.DataAccess.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly XRPContext _context;
        public UserRepository(XRPContext context)
        {
                _context = context;
        }

        public async Task<bool> BookOrderNew(int bookingAllocationId, int userId)
        {
            var bookingAllocation = await _context.BookingAllocations
                .Where(m => m.UId == bookingAllocationId)
                .FirstOrDefaultAsync();

            var user = await _context.Users
                .Where(m => m.UId == userId)
                .FirstOrDefaultAsync();

            if (bookingAllocation != null && user != null)
            {
                var completedDate = DateTime.Now.Date;

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // First update BookingAllocations
                    var updateBookingSql = @"
                    UPDATE BookingAllocations
                    SET IsCompleted = 1, CompletedDate = {0}
                    WHERE UId = {1}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, completedDate, bookingAllocationId);

                    // Then update Users
                    var updateUserSql = @"
                    UPDATE Users
                    SET BookingCount = BookingCount + 1,
                        TodayCommission = TodayCommission + {0},
                        TotalBalance = TotalBalance + {0}
                    WHERE UId = {1}";
                    await _context.Database.ExecuteSqlRawAsync(updateUserSql, bookingAllocation.Commision, userId);

                    if (user.BookingCount == 17 && bookingAllocation.Stage==1)
                    {
                        var updateUserSql18 = @"
                        UPDATE Users
                        SET TotalBalance = (TotalBalance - 10000),
                            LevelBonus = 0 
                        WHERE UId = {0}";
                        await _context.Database.ExecuteSqlRawAsync(updateUserSql18, userId);
                    }

                    // Finally update BookingAllocations with the new stage
                    await transaction.CommitAsync();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> BookOrder(BookingAllocations allocation)
        {
            if (allocation is not null )
            {
                allocation.IsCompleted = true;
                allocation.CompletedDate = DateTime.Now.Date;

                _context.Update(allocation);
               return  await _context.SaveChangesAsync() > 0;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UdpateUserInfo(Users user,BookingAllocations allocation)
        {
            if (user is not null) 
            {
                user.BookingCount = user.BookingCount + 1;
                user.TotalCommission = user.TotalCommission + allocation.Commision;
                _context.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UdpateUserInfoUserNameAndPassword(int userId, bool? gender,string? password)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (gender is not null)
                {
                    var updateBookingSql = @"
                    UPDATE Users
                    SET Gender = {0}
                    WHERE UId = {1}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, gender, userId);
                }
                if (password is not null)
                {
                    var updateBookingSql = @"
                    UPDATE Users
                    SET Password = {0}
                    WHERE UId = {1}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, password, userId);
                }
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
            

        }

        public async Task<bool> CheckUserContact(string contact)
        {
            var user = await _context.Users.Where(m => m.Contact == contact).FirstOrDefaultAsync();
            if (user != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<BookingAllocations> GetBookingAllocationsById(int userId)
        {
            var allocation = await _context.BookingAllocations.Where(m => m.UId == userId).FirstOrDefaultAsync();
            return allocation;
        }

        public async Task<BookingAllocations> GetBookingAllocationsByUserId(int userId)
        {
            var booking = await _context.BookingAllocations.Include(m=> m.Bookings).Where(m => m.UserId == userId && m.IsCompleted==false).OrderBy(m=> m.UId).FirstOrDefaultAsync();
            return booking!;
        }

        public async Task<List<BookingAllocations>> GetBookingAllocationsByUserId1(int userId)
        {
            var booking = await _context.BookingAllocations.Include(m => m.Bookings).Where(m => m.UserId == userId).OrderBy(m => m.UId).ToListAsync();
            return booking!;
        }

        public async Task<Users> GetUserById(int userId)
        {
            var user = await _context.Users.Where(m=> m.UId== userId).FirstAsync();
            return user;
        }

        public async Task<List<Users>> GetUserListAsync()
        {
            var usersList =  await _context.Users.AsNoTracking().OrderByDescending(m=> m.UId).ToListAsync();
            return usersList;
        }

        public async Task<bool> RegisterUsers(RegisterDto registerDto)
        {
            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    Users users = new Users
                    {
                        Password = registerDto.Password.Trim(),
                        InvitaionCode = registerDto.InvitaionCode.Trim(),
                        UserName = registerDto.Username.Trim(),
                        CreatedDate = DateTime.Now,
                        Active = false,
                        Contact = registerDto.MobileNumber.Trim(),
                        Level = "trial",
                        TotalDeposit = 0,
                        TotalWithdraw = 0,
                        TodayCommission = 0,
                        TotalBalance = 10000,
                        LevelBonus = 10000,
                        TotalCommission = 0,
                        PendingAmount = 0,
                        BookingCount=0
                    };

                    await _context.Users.AddAsync(users);
                    await _context.SaveChangesAsync();
                    var bookings = await _context.Bookings.ToListAsync();
                    List<BookingAllocations> bookingsList = new List<BookingAllocations>();

                    foreach (var booking in bookings)
                    {
                        var bookingAlocation = new BookingAllocations
                        {
                            BookingUId = booking.UId,
                            UserId = users.UId,
                            IsCompleted = false,
                            AllocatedDate = DateTime.Now,
                            CompletedDate = null,
                            Active = true,
                            Commision = booking.Commision,
                            Price = booking.Price,
                            Stage = 1,
                            XRate=1

                        };

                        bookingsList.Add(bookingAlocation);
                    }

                    await _context.BookingAllocations.AddRangeAsync(bookingsList);

                    var response = await _context.SaveChangesAsync();

                    // Commit the transaction
                    transactionScope.Complete();

                    return response > 0;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of error
                    return false;
                }
            }
        }
  
        public async Task<Users> UserLogin(LoginDto loginDto)
        {
            var user = await _context.Users.
                Where(m => m.Contact == loginDto.MobileNumber.Trim() && m.Password==loginDto.Password.Trim() && m.Active)
                .FirstOrDefaultAsync();

            return user!;
        }

        public async Task<decimal> TodayCommision(int userId)
        {
            var today = DateTime.Today;

            var totalCommission = await _context.BookingAllocations
            .Where(b => b.UserId == userId
                        && b.IsCompleted
                        && b.CompletedDate.HasValue
                        && b.CompletedDate.Value.Date == today)
            .SumAsync(b => (decimal?)b.Commision) ?? 0;
            return totalCommission;
        }

        public async Task<bool> ReduceUserBalanace(int userId, decimal bookingAmount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var updateBookingSql = @"
                    UPDATE Users
                    SET TotalBalance = (TotalBalance - {0}) 
                    WHERE UId = {1}";
                await _context.Database.ExecuteSqlRawAsync(updateBookingSql, bookingAmount, userId);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }

        }

        public async Task<List<UsersHistory>> GetUserHistoryByUserId(int userId)
        {
            var historyList = await _context.UsersHistory.Where(m => m.UId == userId).ToListAsync();
            return historyList;
        }

        public async Task<AdminUsers> UserLoginAdmin(string username, string password)
        {
            var user = await _context.AdminUsers.
                Where(m => m.Username == username.Trim() && m.Password == password.Trim() && m.Active)
                .FirstOrDefaultAsync();

            return user!;
        }

        public async Task<List<Users>> GetUserListAsyncActive()
        {
            var usersList = await _context.Users.Where(m=> m.Active==false).ToListAsync();
            return usersList;
        }

        public async Task<bool> ActivateUser(int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var updateBookingSql = @"
                    UPDATE Users
                    SET Active = 1 
                    WHERE UId = {0}";
                await _context.Database.ExecuteSqlRawAsync(updateBookingSql, userId);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> Deposit(int userId, decimal depositAmount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                            .Where(u => u.UId == userId)
                            .Select(u => new { u.TotalBalance })
                            .FirstOrDefaultAsync();

                Deposits deposit = new Deposits
                {
                    UserId = userId,
                    CreatedDate = DateTime.Now,
                    Active=1,
                    Amount=depositAmount
                };

                await _context.Deposits.AddAsync(deposit);
                await _context.SaveChangesAsync();

                if (user.TotalBalance < 0)
                {
                    var userHistory = await _context.UsersHistory
                               .Where(u => u.UId == userId)
                               .OrderByDescending(m => m.HistoryId)
                               .Select(u => new { u.TotalBalance })
                               .FirstOrDefaultAsync();

                    decimal total = userHistory.TotalBalance + depositAmount;

                    var updateBookingSql1 = @"
                    UPDATE Users
                    SET TotalBalance = {0}
                    WHERE UId = {1}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql1, total, userId);

                }
                else {
                    var updateBookingSql = @"
                    UPDATE Users
                    SET TotalBalance = (TotalBalance + {0}) 
                    WHERE UId = {1}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, depositAmount, userId);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> Reduce(int userId, decimal reduceAmount)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                
                var updateBookingSql = @"
                    UPDATE Users
                    SET TotalBalance = (TotalBalance - {0}) 
                    WHERE UId = {1}";
                await _context.Database.ExecuteSqlRawAsync(updateBookingSql, reduceAmount, userId);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> CreaditScore(int userId, decimal score)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var updateBookingSql = @"
                    UPDATE Users
                    SET CreditScore = {0} 
                    WHERE UId = {1}";
                await _context.Database.ExecuteSqlRawAsync(updateBookingSql, score, userId);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> Benifit(int userId, decimal score)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var updateBookingSql = @"
                    UPDATE Users
                    SET MemberBenifit = {0} 
                    WHERE UId = {1}";
                await _context.Database.ExecuteSqlRawAsync(updateBookingSql, score, userId);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> Pop(int userId, string popMessage)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var updateBookingSql = @"
                    UPDATE Users
                    SET PopupMessage = {0} 
                    WHERE UId = {1}";
                await _context.Database.ExecuteSqlRawAsync(updateBookingSql, popMessage, userId);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> Reset(int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.BookingAllocations
                            .Where(u => u.UId == userId && !u.IsCompleted).ToListAsync();

                if (user is null)
                {
                    return false;
                }

                var updateBookingSql = @"
                    UPDATE Users
                    SET BookingCount = 0, Level='next' 
                    WHERE UId = {0}";
                await _context.Database.ExecuteSqlRawAsync(updateBookingSql,userId);

                var bookings = await _context.Bookings.ToListAsync();
                List<BookingAllocations> bookingsList = new List<BookingAllocations>();

                foreach (var booking in bookings)
                {
                    var bookingAlocation = new BookingAllocations
                    {
                        BookingUId = booking.UId,
                        UserId = userId,
                        IsCompleted = false,
                        AllocatedDate = DateTime.Now,
                        CompletedDate = null,
                        Active = true,
                        Commision = booking.Commision,
                        Price = booking.Price,
                        Stage = 2

                    };

                    bookingsList.Add(bookingAlocation);
                }

                await _context.BookingAllocations.AddRangeAsync(bookingsList);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<BookingAllocations>> GetBookingAllocationsListByUserId(int userId)
        {
           var bookingList= await _context.BookingAllocations.Include(m=> m.Bookings).Where(m=> m.UserId == userId && !m.IsCompleted).ToListAsync();
            return bookingList;
        }

        public async Task<bool> UpdateAllocations(List<UpdateAllocation> request)
        {
            foreach (var item in request)
            {
                var allocation = await _context.BookingAllocations
                    .FirstOrDefaultAsync(m => m.UId == item.UId);

                if (allocation != null)
                {
                    allocation.Price = item.Price;
                    allocation.Commision = item.Commision;
                    allocation.GiftPrecentage = item.GiftPrecentage;

                    _context.BookingAllocations.Update(allocation);
                }
            }

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> LockUser(int userId)
        {
            var user = await _context.Users.Where(m => m.UId == userId).FirstOrDefaultAsync();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (user.Active)
                {
                    var updateBookingSql = @"
                    UPDATE Users
                    SET Active = 0 
                    WHERE UId = {0}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, userId);
                }
                else
                {
                    var updateBookingSql = @"
                    UPDATE Users
                    SET Active = 1
                    WHERE UId = {0}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, userId);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> ResetPassword(int userId, string password)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var updateBookingSql = @"
                    UPDATE Users
                    SET Password = {0}
                    WHERE UId = {1}";
                await _context.Database.ExecuteSqlRawAsync(updateBookingSql, password.Trim(), userId);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> IsUserHasBank(int userId)
        {
           var bankdetails = await _context.BankAccounts.Where(m => m.UserId == userId).FirstOrDefaultAsync();
            if (bankdetails is not null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> WithdrawalWithBank(int userId, WithdrawalPostDto response)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var wid = await _context.Withdrawal.Where(m=> m.UserId== userId && m.Approved==1).ToListAsync();
                if (wid.Count == 0)
                {
                    var bank = new BankAccounts
                    {
                        BankName = response.BankName,
                        AccHolder = response.AccHolder,
                        ContactNo = response.ContactNo,
                        AccNo = response.AccNo,
                        UPI = response.UPI,
                        IFSC = response.IFSC,
                        UserId = userId
                    };

                    await _context.AddAsync(bank);
                    await _context.SaveChangesAsync();

                    var withdrawal = new Withdrawal
                    {
                        UserId = userId,
                        BankId = bank.UId, // ensure `UId` is generated after SaveChanges
                        Amount = response.Amount,
                        CreatedDate = DateTime.Now,
                        Approved = 1,
                        Active = true
                    };

                    await _context.AddAsync(withdrawal);
                    await _context.SaveChangesAsync();


                    var updateBookingSql = @"
                    UPDATE Users
                    SET PendingAmount = (PendingAmount + {0}) 
                    WHERE UId = {1}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, response.Amount, userId);
                    await transaction.CommitAsync();
                    return true;
                }
                else
                { 
                    return false;
                }
                
  
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
            
        }

        public async Task<bool> WithdrawalWithoutBank(int userId, WithdrawalWithoutBankDto responce)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var wid = await _context.Withdrawal.Where(m => m.UserId == userId && m.Approved == 1).ToListAsync();
                if (wid.Count == 0)
                {
                    var bank = await _context.BankAccounts.Where(m => m.UserId == userId).FirstOrDefaultAsync();
                    var withdrawal = new Withdrawal
                    {
                        UserId = userId,
                        BankId = bank.UId, // ensure `UId` is generated after SaveChanges
                        Amount = responce.Amount,
                        CreatedDate = DateTime.Now,
                        Approved = 1,
                        Active = true
                    };

                    await _context.AddAsync(withdrawal);
                    await _context.SaveChangesAsync();


                    var updateBookingSql = @"
                    UPDATE Users
                    SET PendingAmount = (PendingAmount + {0}) 
                    WHERE UId = {1}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, responce.Amount, userId);
                    await transaction.CommitAsync();
                    return true;

                }
                else {
                    return false;
                }
              }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<Withdrawal>> GetUserListAsyncWithBankActive()
        {
            var userList = await _context.Withdrawal.Include(m => m.BankAccount).OrderBy(m=> m.UId).ToListAsync();
            return userList;
        }

        public async Task<bool> Rejected(int withdrawalId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var withdrawal = await _context.Withdrawal
                .Where(m => m.UId == withdrawalId)
                .FirstOrDefaultAsync();

                if (withdrawal != null)
                {
                    withdrawal.Approved = 3;
                    _context.Withdrawal.Update(withdrawal); // ✅ No await here
                    await _context.SaveChangesAsync();

                    var updateBookingSql = @"
                    UPDATE Users
                    SET PendingAmount = 0
                    WHERE UId = {0}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, withdrawal.UserId);

                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }
            
        }
        public async Task<bool> Approved(int withdrawalId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var withdrawal = await _context.Withdrawal
                .Where(m => m.UId == withdrawalId)
                .FirstOrDefaultAsync();

                if (withdrawal != null)
                {
                    withdrawal.Approved = 2;
                    _context.Withdrawal.Update(withdrawal); // ✅ No await here
                    await _context.SaveChangesAsync();

                    var updateBookingSql = @"
                    UPDATE Users
                    SET PendingAmount = 0,
                    TotalBalance = (TotalBalance - {0}) 
                    WHERE UId = {1}";
                    await _context.Database.ExecuteSqlRawAsync(updateBookingSql, withdrawal.Amount,withdrawal.UserId);

                    await transaction.CommitAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                await transaction.RollbackAsync();
                return false;
            }

        }

        public async Task<List<Withdrawal>> GetWithdrawals(int userId)
        {
            var deposit= await _context.Withdrawal
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.UId)
                .ToListAsync();
            return deposit;
        }

        
    }
}
