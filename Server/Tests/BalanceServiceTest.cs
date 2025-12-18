using Api.Models;
using Api.Security;
using Api.Services.Interfaces;
using DataAccess;

namespace xunittests;

public class BalanceServiceTest(IBalanceService balanceService, MyDbContext context, ITestOutputHelper outputHelper, KonciousArgon2idPasswordHasher hasher)
{

    public async Task<User> CreateTestUser()
    {
        var password = "EASV2025";
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = "easv@test.com",
            Phonenumber = "12345678",
            Password = hasher.HashPassword(null, password),
            Firstname = "EASV",
            Lastname = "Test",
            Isactive = true
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
        
    }

    [Fact]
    public async Task SubmitDeposit_ValidDeposit()
    {
        User testUser = await CreateTestUser();

        var deposit = new SubmitDepositDTO
        (
            testUser.Id,
            1000, 
            "MP1000"
        );
        
        var transaction = await balanceService.SubmitDepositAsync(deposit);
        
        Assert.Equal(deposit.Amount, transaction.Amount);
        Assert.Equal(deposit.UserId, transaction.Userid);
    }

    [Fact]
    public async Task SubmitDeposit_InvalidUser()
    {
        User testUser = await CreateTestUser();
        
        var deposit = new SubmitDepositDTO
        (
            Guid.NewGuid().ToString(),
            1000, 
            "MP1000"
        );
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() => balanceService.SubmitDepositAsync(deposit));
    }

    [Fact]
    public async Task SubmitDeposit_InvalidAmount()
    {
        User testUser = await CreateTestUser();
        
        var deposit = new SubmitDepositDTO
        (
            testUser.Id,
            -1000, 
            "MP1000"
        );
        
        await Assert.ThrowsAsync<ArgumentException>(() => balanceService.SubmitDepositAsync(deposit));
    }

    [Fact]
    public async Task SubmitDeposit_MissingTransactionId()
    {
        User testUser = await CreateTestUser();
        
        var deposit = new SubmitDepositDTO
        (
            testUser.Id,
            1000, 
            null
        );
        
        await Assert.ThrowsAsync<ArgumentException>(() => balanceService.SubmitDepositAsync(deposit));
    }
    
    [Fact]
    public async Task ApproveDeposit_ValidTransaction()
    {
        User testUser = await CreateTestUser();
        
        var deposit = new SubmitDepositDTO
        (
            testUser.Id,
            1000, 
            "MP1000"
        );
        var transaction = await balanceService.SubmitDepositAsync(deposit);
        
        await balanceService.ApproveTransactionAsync(transaction.Id);
        var approvedTransaction = await balanceService.GetUserTransactionsAsync(testUser.Id);
        
        Assert.Single(approvedTransaction);
        Assert.Equal(transaction.Id, approvedTransaction.First().Id);
    }

    [Fact]
    public async Task ApproveDeposit_InvalidTransaction()
    {
        User testUser = await CreateTestUser();
        
        await Assert.ThrowsAsync<KeyNotFoundException>(() => balanceService.ApproveTransactionAsync(1));
    }

    [Fact]
    public async Task ApproveDeposit_IncorrectStatus()
    {
        User testUser = await CreateTestUser();
        
        var deposit = new SubmitDepositDTO
        (
            testUser.Id,
            1000, 
            "MP1000"
        );
        var transaction = await balanceService.SubmitDepositAsync(deposit);
        
        await balanceService.ApproveTransactionAsync(transaction.Id);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => balanceService.ApproveTransactionAsync(transaction.Id));
    }

    [Fact]
    public async Task GetPendingTransactions()
    {
        User testUser = await CreateTestUser();
        
        var deposit1 = new SubmitDepositDTO
        (
            testUser.Id,
            1000, 
            "MP1000"
        );
        await balanceService.SubmitDepositAsync(deposit1); 
        
        var deposit2 = new SubmitDepositDTO
        (
            testUser.Id,
            2000, 
            "MP2000"
        );
        await balanceService.SubmitDepositAsync(deposit2); 
        
        var pendingTransactions = await balanceService.GetPendingTransactionsAsync();
        
        Assert.Equal(2, pendingTransactions.Count);
    }
    
    [Fact]
    public async Task GetApprovedTransactions()
    {
        User testUser = await CreateTestUser();
        
        var deposit1 = new SubmitDepositDTO
        (
            testUser.Id,
            1000, 
            "MP1000"
        );
        var transaction1 = await balanceService.SubmitDepositAsync(deposit1); 
        
        var deposit2 = new SubmitDepositDTO
        (
            testUser.Id,
            2000, 
            "MP2000"
        );
        await balanceService.SubmitDepositAsync(deposit2); 
        
        await balanceService.ApproveTransactionAsync(transaction1.Id);
        
        var approvedTransactions = await balanceService.GetApprovedTransactionsAsync();
        
        Assert.Single(approvedTransactions);
    }
    
    [Fact] 
    public async Task GetAllTransactions()
    {
        User testUser = await CreateTestUser();
        
        var deposit1 = new SubmitDepositDTO
        (
            testUser.Id,
            1000, 
            "MP1000"
        );
        var transaction1 = await balanceService.SubmitDepositAsync(deposit1);
        
        var deposit2 = new SubmitDepositDTO
        (
            testUser.Id,
            2000, 
            "MP2000"
        );
        await balanceService.SubmitDepositAsync(deposit2);
        
        await balanceService.ApproveTransactionAsync(transaction1.Id);
        
        var allTransactions = await balanceService.GetAllTransactionsAsync();
        
        Assert.Equal(2, allTransactions.Count);
    }
    
    [Fact]
    public async Task GetUserTransactions()
    {
        User testUser1 = await CreateTestUser();
        
        var deposit1 = new SubmitDepositDTO
        (
            testUser1.Id,
            1000, 
            "MP1000"
        );
        await balanceService.SubmitDepositAsync(deposit1);
        
        User testUser2 = await CreateTestUser();
        
        var deposit2 = new SubmitDepositDTO
        (
            testUser2.Id,
            2000, 
            "MP2000"
        );
        await balanceService.SubmitDepositAsync(deposit2);
        
        var userTransactions = await balanceService.GetUserTransactionsAsync(testUser1.Id);
        
        Assert.Single(userTransactions);
    }
    
    [Fact]
    public async Task GetUserBalance_ValidData()
    {
        User testUser = await CreateTestUser();
        
        var deposit1 = new SubmitDepositDTO
        (
            testUser.Id,
            1000, 
            "MP1000"
        );
        var transaction = await balanceService.SubmitDepositAsync(deposit1);

        await balanceService.ApproveTransactionAsync(transaction.Id);
        
        var balance = await balanceService.GetUserBalanceAsync(testUser.Id);
        
        Assert.Equal(deposit1.Amount, balance.CurrentBalance);
    }

    [Fact]
    public async Task GetUserBalance_PendingTransaction()
    {
        User testUser = await CreateTestUser();
        
        var deposit1 = new SubmitDepositDTO
        (
            testUser.Id,
            1000, 
            "MP1000"
        );
        await balanceService.SubmitDepositAsync(deposit1);
        
        var balance = await balanceService.GetUserBalanceAsync(testUser.Id);
        
        Assert.Equal(0, balance.CurrentBalance);
        Assert.Equal(deposit1.Amount, balance.PendingDeposits);
    }
    
    
}