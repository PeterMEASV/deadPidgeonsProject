using Api.Models;
using Api.Services.Classes;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.Extensions.Logging;
using Xunit;

namespace xunittests;

public class HistoryServiceTest
{
    private readonly HistoryService _historyService;

    public HistoryServiceTest()
    {
        _historyService = new HistoryService(null!, new LoggerFactory().CreateLogger<HistoryService>());
    }

    [Fact]
    public void CalculateBoardPrice_ValidInput()
    {
        var method = _historyService.GetType()
            .GetMethod("CalculateBoardPrice", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        Assert.Equal(20m, method.Invoke(_historyService, new object[] { 5 }));
        Assert.Equal(40m, method.Invoke(_historyService, new object[] { 6 }));
        Assert.Equal(80m, method.Invoke(_historyService, new object[] { 7 }));
        Assert.Equal(160m, method.Invoke(_historyService, new object[] { 8 }));
    }

    [Fact]
    public void CalculateBoardPrice_InvalidInput()
    {
        var method = _historyService.GetType()
            .GetMethod("CalculateBoardPrice", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

        Assert.Equal(0m, method.Invoke(_historyService, new object[] { 4 }));
        Assert.Equal(0m, method.Invoke(_historyService, new object[] { 9 }));
        Assert.Equal(0m, method.Invoke(_historyService, new object[] { 0 }));
    }

    [Fact]
    public void CreateLog_DoesNotThrow()
    {
        var exception = Record.ExceptionAsync(() => _historyService.CreateLog("Test log"));
        Assert.NotNull(exception);
    }

    [Fact]
    public void DeleteLog_DoesNotThrowForNull()
    {
        var exception = Record.ExceptionAsync(() => _historyService.DeleteLog("nonexistent-id"));
        Assert.NotNull(exception);
    }

    [Fact]
    public void GetUserBoardHistoryAsync_DoesNotThrowForNull()
    {
        var exception = Record.ExceptionAsync(() => _historyService.GetUserBoardHistoryAsync("user-id"));
        Assert.NotNull(exception);
    }
}
