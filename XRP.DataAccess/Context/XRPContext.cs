using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using XRP.Domain.Entity;

namespace XRP.DataAccess.Context
{
    public class XRPContext :DbContext
    {
        public XRPContext(DbContextOptions<XRPContext> options)
       : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>().HasKey(m => m.UId);

            modelBuilder.Entity<BookingAllocations>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.HasOne(e => e.Bookings)
                      .WithMany() 
                      .HasForeignKey(e => e.BookingUId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Users)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Bookings>(entity =>
            {
                entity.HasKey(e => e.UId);
            });

            modelBuilder.Entity<UsersHistory>().HasKey(m => m.HistoryId);

            modelBuilder.Entity<AdminUsers>().HasKey(m => m.UId);

            modelBuilder.Entity<BankAccounts>().HasKey(m => m.UId);

            modelBuilder.Entity<Withdrawal>()
             .HasOne(w => w.BankAccount)
             .WithMany() // or `.WithMany(b => b.Withdrawals)` if you have a navigation property in `BankAccounts`
             .HasForeignKey(w => w.BankId);

            modelBuilder.Entity<Deposits>()
            .HasOne(w => w.Users)
            .WithMany() // or `.WithMany(b => b.Withdrawals)` if you have a navigation property in `BankAccounts`
            .HasForeignKey(w => w.UserId);
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        public DbSet<BookingAllocations> BookingAllocations { get; set; }
        public DbSet<UsersHistory> UsersHistory { get; set; }
        public DbSet<AdminUsers> AdminUsers { get; set; }
        public DbSet<BankAccounts> BankAccounts { get; set; }
        public DbSet<Withdrawal> Withdrawal { get; set; }
        public DbSet<Deposits> Deposits { get; set; }
    }
}
