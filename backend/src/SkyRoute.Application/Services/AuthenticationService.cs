using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using SkyRoute.Application.DTOs.Auth;
using SkyRoute.Application.Exceptions;
using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Application.Services;

public sealed class AuthenticationService(
    IUserRepository userRepository,
    IPasswordHasherService passwordHasherService,
    IJwtTokenService jwtTokenService) : IAuthenticationService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        ValidateRegisterRequest(request);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingUser = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictException("A user with that email already exists.");
        }

        var user = new AppUser
        {
            FullName = request.FullName.Trim(),
            Email = normalizedEmail
        };

        user.PasswordHash = passwordHasherService.HashPassword(user, request.Password);

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        var token = jwtTokenService.IssueToken(user);
        return new AuthResponse(token.Token, token.ExpiresAtUtc, new UserProfileDto(user.Id, user.FullName, user.Email));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        ValidateLoginRequest(request);

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !passwordHasherService.VerifyPassword(user, user.PasswordHash, request.Password))
        {
            throw new UnauthorizedOperationException("Invalid email or password.");
        }

        var token = jwtTokenService.IssueToken(user);
        return new AuthResponse(token.Token, token.ExpiresAtUtc, new UserProfileDto(user.Id, user.FullName, user.Email));
    }

    private static void ValidateRegisterRequest(RegisterRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            errors[nameof(request.FullName)] = ["Full name is required."];
        }

        if (!IsValidEmail(request.Email))
        {
            errors[nameof(request.Email)] = ["A valid email address is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            errors[nameof(request.Password)] = ["Password must be at least 8 characters long."];
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }

    private static void ValidateLoginRequest(LoginRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (!IsValidEmail(request.Email))
        {
            errors[nameof(request.Email)] = ["A valid email address is required."];
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors[nameof(request.Password)] = ["Password is required."];
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }

    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var _ = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}