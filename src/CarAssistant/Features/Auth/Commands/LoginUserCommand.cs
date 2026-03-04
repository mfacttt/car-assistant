using CarAssistant.Data;
using CarAssistant.Features.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Auth.Commands;

public record LoginUserCommand(string Email, string Password);

public class LoginUserCommandHandler
{
    private readonly ApplicationDbContext _db;

    public LoginUserCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<User?> Handle(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();
        var passwordHash = PasswordHasher.Hash(command.Password);

        return await _db.Users
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail && u.PasswordHash == passwordHash, cancellationToken);
    }
}
