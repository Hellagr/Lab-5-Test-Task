using Lab5TestTask.Data;
using Lab5TestTask.Models;
using Lab5TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Lab5TestTask.Services.Implementations;

/// <summary>
/// UserService implementation.
/// Implement methods here.
/// </summary>
public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;

    public UserService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<User> GetUserAsync()
    {
        var sessions = await _dbContext.Sessions
            .GroupBy(session => session.UserId)
            .Select(g => new { UserId = g.Key, SessionCount = g.Count() })
            .OrderByDescending(x => x.SessionCount)
            .FirstOrDefaultAsync();

        var user = await _dbContext.Users
            .SingleAsync(user => user.Id == sessions.UserId);

        return user;
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var mobileUserIds = await _dbContext.Sessions
            .Where(session => session.DeviceType == Enums.DeviceType.Mobile)
            .Select(session => session.UserId)
            .Distinct()
            .ToListAsync();

        var users = await _dbContext.Users
            .Where(user => mobileUserIds.Contains(user.Id))
            .ToListAsync();

        return users;
    }
}
