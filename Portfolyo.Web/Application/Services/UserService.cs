using AutoMapper;
using FluentValidation;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.Users;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Services;

public class UserService : Service<User, UserDto, UserCreateDto, UserUpdateDto>, IUserService
{
    private const int MinimumPasswordLength = 8;

    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IEnumerable<IValidator<UserCreateDto>> createValidators,
        IEnumerable<IValidator<UserUpdateDto>> updateValidators)
        : base(unitOfWork, mapper, createValidators, updateValidators)
        => _passwordHasher = passwordHasher;

    protected override string NotFoundMessage => "Kullanıcı bulunamadı.";

    public async Task<Result<UserDto>> GetByUserNameAsync(
        string userName, CancellationToken cancellationToken = default)
    {
        var normalized = (userName ?? string.Empty).Trim();

        var user = await Repository.GetFirstOrDefaultAsync(
            x => string.Equals(x.UserName, normalized, StringComparison.OrdinalIgnoreCase), cancellationToken);

        return user is null
            ? Result<UserDto>.NotFound(NotFoundMessage)
            : Result<UserDto>.Success(Mapper.Map<UserDto>(user));
    }

    public async Task<Result> ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await Repository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        if (!_passwordHasher.Verify(user.PasswordHash, currentPassword))
        {
            return Result.Invalid(nameof(currentPassword), "Mevcut parola hatalı.");
        }

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < MinimumPasswordLength)
        {
            return Result.Invalid(nameof(newPassword), $"Yeni parola en az {MinimumPasswordLength} karakter olmalıdır.");
        }

        user.PasswordHash = _passwordHasher.Hash(newPassword);

        await Repository.UpdateAsync(user, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Parola güncellendi.");
    }

    public async Task<Result<UserDto>> VerifyPasswordAsync(
        string userNameOrEmail, string password, CancellationToken cancellationToken = default)
    {
        var value = (userNameOrEmail ?? string.Empty).Trim();

        var user = await Repository.GetFirstOrDefaultAsync(
            x => string.Equals(x.UserName, value, StringComparison.OrdinalIgnoreCase)
                 || string.Equals(x.Email, value, StringComparison.OrdinalIgnoreCase),
            cancellationToken);

        // Kullanıcı yok ve parola yanlış durumları aynı mesajı döner;
        // aksi halde hangi kullanıcı adlarının kayıtlı olduğu dışarıdan öğrenilebilir.
        if (user is null || !_passwordHasher.Verify(user.PasswordHash, password))
        {
            return Result<UserDto>.Invalid(nameof(password), "Kullanıcı adı veya parola hatalı.");
        }

        if (!user.IsActive)
        {
            return Result<UserDto>.Forbidden("Hesabınız devre dışı bırakılmış.");
        }

        return Result<UserDto>.Success(Mapper.Map<UserDto>(user));
    }

    public async Task<Result> RecordLoginAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await Repository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.NotFound(NotFoundMessage);
        }

        user.LastLoginAt = DateTime.UtcNow;

        await Repository.UpdateAsync(user, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    protected override async Task<Result> OnCreatingAsync(
        UserCreateDto dto, User entity, CancellationToken cancellationToken)
    {
        var uniqueness = await EnsureUniqueAsync(dto.UserName, dto.Email, null, cancellationToken);

        if (uniqueness.IsFailure)
        {
            return uniqueness;
        }

        // Parola yalnızca burada hash'lenir; mapping profili PasswordHash'i bilinçli olarak yok sayar.
        entity.PasswordHash = _passwordHasher.Hash(dto.Password);

        return Result.Success();
    }

    protected override Task<Result> OnUpdatingAsync(
        UserUpdateDto dto, User entity, CancellationToken cancellationToken)
        => EnsureUniqueAsync(dto.UserName, dto.Email, dto.Id, cancellationToken);

    /// <summary>Kullanıcı adı ve e-posta başka bir kayıtta kullanılıyor mu?</summary>
    private async Task<Result> EnsureUniqueAsync(
        string userName, string email, Guid? excludedId, CancellationToken cancellationToken)
    {
        var users = await Repository.GetAllAsync(cancellationToken);

        foreach (var user in users)
        {
            if (excludedId.HasValue && user.Id == excludedId.Value)
            {
                continue;
            }

            if (string.Equals(user.UserName, userName, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Conflict("Bu kullanıcı adı zaten kullanılıyor.");
            }

            if (string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Conflict("Bu e-posta adresi zaten kullanılıyor.");
            }
        }

        return Result.Success();
    }
}
