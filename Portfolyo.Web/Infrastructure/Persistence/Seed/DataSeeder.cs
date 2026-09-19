using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Application.DTOs.Roles;
using Portfolyo.Web.Application.DTOs.UserRoles;
using Portfolyo.Web.Application.DTOs.Users;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Infrastructure.Persistence.Seed;

/// <summary>
/// İlk çalıştırmada Admin rolünü, tüm yetkileri ve yönetici kullanıcısını oluşturur.
/// Tekrar çalıştırılabilir: var olan kayıtları bozmaz, yalnızca eksikleri tamamlar.
/// </summary>
public sealed class DataSeeder
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IUserRoleService _userRoleService;
    private readonly IRolePermissionService _rolePermissionService;
    private readonly SeedOptions _options;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        IUserService userService,
        IRoleService roleService,
        IUserRoleService userRoleService,
        IRolePermissionService rolePermissionService,
        IOptions<SeedOptions> options,
        ILogger<DataSeeder> logger)
    {
        _userService = userService;
        _roleService = roleService;
        _userRoleService = userRoleService;
        _rolePermissionService = rolePermissionService;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>Enum'da tanımlı tüm yetkiler (None hariç).</summary>
    public static IReadOnlyList<Permission> AllPermissions { get; } = Enum.GetValues<Permission>()
        .Where(permission => permission != Permission.None)
        .OrderBy(permission => permission)
        .ToList();

    public async Task<Result> SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return Result.Success("Seed kapalı.");
        }

        var role = await EnsureRoleAsync(cancellationToken);

        if (role.IsFailure)
        {
            _logger.LogError("Seed başarısız: {Mesaj}", role.Message);
            return role;
        }

        var roleId = role.Data!.Id;

        // Admin rolü her zaman tüm yetkilere sahip olmalı; yeni yetki eklendiğinde de tamamlanır.
        var permissions = await _rolePermissionService.AssignAsync(
            new RolePermissionAssignDto(roleId, AllPermissions), cancellationToken);

        if (permissions.IsFailure)
        {
            _logger.LogError("Yetkiler atanamadı: {Mesaj}", permissions.Message);
            return permissions;
        }

        var user = await EnsureUserAsync(cancellationToken);

        if (user.IsFailure)
        {
            _logger.LogError("Yönetici kullanıcısı oluşturulamadı: {Mesaj}", user.Message);
            return user;
        }

        var assignment = await _userRoleService.AssignAsync(
            new UserRoleAssignDto(user.Data!.Id, new[] { roleId }), cancellationToken);

        if (assignment.IsFailure)
        {
            _logger.LogError("Rol ataması yapılamadı: {Mesaj}", assignment.Message);
            return assignment;
        }

        return Result.Success("Seed tamamlandı.");
    }

    private async Task<Result<RoleDto>> EnsureRoleAsync(CancellationToken cancellationToken)
    {
        var existing = await _roleService.GetByNameAsync(_options.AdminRoleName, cancellationToken);

        if (existing.IsSuccess)
        {
            return existing;
        }

        if (existing.Status != ResultStatus.NotFound)
        {
            return existing;
        }

        _logger.LogInformation("{Rol} rolü oluşturuluyor.", _options.AdminRoleName);

        return await _roleService.CreateAsync(
            new RoleCreateDto(_options.AdminRoleName, "Tüm yetkilere sahip yönetici rolü."), cancellationToken);
    }

    private async Task<Result<UserDto>> EnsureUserAsync(CancellationToken cancellationToken)
    {
        var existing = await _userService.GetByUserNameAsync(_options.AdminUserName, cancellationToken);

        if (existing.IsSuccess)
        {
            // Kullanıcı varsa parolasına dokunulmaz.
            return existing;
        }

        if (existing.Status != ResultStatus.NotFound)
        {
            return existing;
        }

        var generated = string.IsNullOrWhiteSpace(_options.AdminPassword);
        var password = generated ? GeneratePassword() : _options.AdminPassword;

        var created = await _userService.CreateAsync(
            new UserCreateDto(
                _options.AdminUserName,
                _options.AdminEmail,
                _options.AdminFirstName,
                _options.AdminLastName,
                password),
            cancellationToken);

        if (created.IsSuccess && generated)
        {
            // Parola yapılandırmada verilmediği için bir kez burada gösteriliyor.
            // Giriş yaptıktan sonra değiştirilmeli.
            _logger.LogWarning(
                "Yönetici hesabı oluşturuldu. Kullanıcı adı: {KullaniciAdi} - Geçici parola: {Parola} " +
                "(giriş yaptıktan sonra değiştirin)",
                _options.AdminUserName,
                password);
        }

        return created;
    }

    /// <summary>
    /// Doğrulama kurallarını karşılayan (harf + rakam, 16 karakter) rastgele parola üretir.
    /// </summary>
    private static string GeneratePassword()
    {
        const string letters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string all = letters + digits;

        var characters = new char[16];

        // İlk iki karakter kuralları garanti eder, kalanı tamamen rastgeledir.
        characters[0] = letters[RandomNumberGenerator.GetInt32(letters.Length)];
        characters[1] = digits[RandomNumberGenerator.GetInt32(digits.Length)];

        for (var index = 2; index < characters.Length; index++)
        {
            characters[index] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }

        // Garanti karakterlerin başta olmaması için karıştırılır.
        RandomNumberGenerator.Shuffle(characters.AsSpan());

        return new string(characters);
    }
}
