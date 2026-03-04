using CarAssistant.Data;
using CarAssistant.Features.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace CarAssistant.Features.Auth.Commands;

public record RegisterUserCommand(string Email, string Password);

public class RegisterUserCommandHandler
{
    private readonly ApplicationDbContext _db;

    public RegisterUserCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<(bool Success, string? Error, User? User)> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = command.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);
        if (exists)
        {
            return (false, "Пользователь с такой почтой уже существует.", null);
        }

        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = PasswordHasher.Hash(command.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return (true, null, user);
    }
}
