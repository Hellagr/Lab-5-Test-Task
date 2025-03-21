using Lab5TestTask.Controllers;
using Lab5TestTask.Data;
using Lab5TestTask.Models;
using Lab5TestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lab5TestTask.Services.Implementations;

/// <summary>
/// SessionService implementation.
/// Implement methods here.
/// </summary>
public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _dbContext;

    public SessionService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Session> GetSessionAsync()
    {
        var earliestDesctopSession = await _dbContext.Sessions.FirstOrDefaultAsync(session => session.DeviceType == Enums.DeviceType.Desktop);
        return earliestDesctopSession;
    }

    public async Task<List<Session>> GetSessionsAsync()
    {
        var activeUsers = await _dbContext.Users
            .Where(user => user.Status == Enums.UserStatus.Active)
            .Select(user => user.Id)
            .ToListAsync();

        var userSessionsBefore2025 = await _dbContext.Sessions
            .Where(sess => sess.EndedAtUTC.Year < 2025)
            .ToListAsync();

        var filteredSessions = userSessionsBefore2025
            .Where(sessions => activeUsers.Contains(sessions.UserId))
            .ToList();

        return filteredSessions;
    }
}
