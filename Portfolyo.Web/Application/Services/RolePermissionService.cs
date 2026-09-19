using AutoMapper;
using FluentValidation;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Core.Entities;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.Services;

public class RolePermissionService
    : Service<RolePermission, RolePermissionDto, RolePermissionCreateDto, RolePermissionUpdateDto>,
      IRolePermissionService
{
    public RolePermissionService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEnumerable<IValidator<RolePermissionCreateDto>> createValidators,
        IEnumerable<IValidator<RolePermissionUpdateDto>> updateValidators)
        : base(unitOfWork, mapper, createValidators, updateValidators)
    {
    }

    protected override string NotFoundMessage => "Yetki kaydı bulunamadı.";

    public async Task<Result<IReadOnlyList<RolePermissionDto>>> GetByRoleAsync(
        Guid roleId, CancellationToken cancellationToken = default)
    {
        if (!await UnitOfWork.Repository<Role>().AnyAsync(x => x.Id == roleId, cancellationToken))
        {
            return Result<IReadOnlyList<RolePermissionDto>>.NotFound("Rol bulunamadı.");
        }

        var items = await Repository.GetWhereAsync(x => x.RoleId == roleId, cancellationToken);

        return Result<IReadOnlyList<RolePermissionDto>>.Success(
            Mapper.Map<List<RolePermissionDto>>(items.OrderBy(x => x.Permission)));
    }

    public async Task<Result> AssignAsync(RolePermissionAssignDto dto, CancellationToken cancellationToken = default)
    {
        if (!await UnitOfWork.Repository<Role>().AnyAsync(x => x.Id == dto.RoleId, cancellationToken))
        {
            return Result.NotFound("Rol bulunamadı.");
        }

        var requested = (dto.Permissions ?? Array.Empty<Permission>()).Distinct().ToList();

        // Enum'da tanımlı olmayan bir değer gönderilmişse hiçbir şey yazılmaz.
        if (requested.Any(permission => !Enum.IsDefined(permission)))
        {
            return Result.Invalid(nameof(dto.Permissions), "Tanımlı olmayan bir yetki gönderildi.");
        }

        var current = await Repository.GetWhereAsync(x => x.RoleId == dto.RoleId, cancellationToken);
        var currentPermissions = current.Select(x => x.Permission).ToList();

        var toAdd = requested.Except(currentPermissions).ToList();
        var toRemove = currentPermissions.Except(requested).ToList();

        if (toAdd.Count == 0 && toRemove.Count == 0)
        {
            return Result.Success("Değişiklik yok.");
        }

        if (toRemove.Count > 0)
        {
            await Repository.DeleteWhereAsync(
                x => x.RoleId == dto.RoleId && toRemove.Contains(x.Permission), cancellationToken);
        }

        if (toAdd.Count > 0)
        {
            await Repository.AddRangeAsync(
                toAdd.Select(permission => new RolePermission { RoleId = dto.RoleId, Permission = permission }),
                cancellationToken);
        }

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Yetkiler güncellendi.");
    }

    public async Task<Result<IReadOnlyList<Permission>>> GetByUserAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var assignments = await UnitOfWork.Repository<UserRole>()
            .GetWhereAsync(x => x.UserId == userId, cancellationToken);

        if (assignments.Count == 0)
        {
            return Result<IReadOnlyList<Permission>>.Success(Array.Empty<Permission>());
        }

        var roleIds = assignments.Select(x => x.RoleId).ToHashSet();

        var rolePermissions = await Repository.GetWhereAsync(x => roleIds.Contains(x.RoleId), cancellationToken);

        IReadOnlyList<Permission> permissions = rolePermissions
            .Select(x => x.Permission)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        return Result<IReadOnlyList<Permission>>.Success(permissions);
    }

    public async Task<Result<bool>> UserHasPermissionAsync(
        Guid userId, Permission permission, CancellationToken cancellationToken = default)
    {
        var permissions = await GetByUserAsync(userId, cancellationToken);

        return permissions.IsFailure
            ? permissions.CarryFailure<bool>()
            : Result<bool>.Success(permissions.Data!.Contains(permission));
    }

    protected override Task<Result> OnCreatingAsync(
        RolePermissionCreateDto dto, RolePermission entity, CancellationToken cancellationToken)
        => EnsureAssignableAsync(dto.RoleId, dto.Permission, null, cancellationToken);

    protected override Task<Result> OnUpdatingAsync(
        RolePermissionUpdateDto dto, RolePermission entity, CancellationToken cancellationToken)
        => EnsureAssignableAsync(dto.RoleId, dto.Permission, dto.Id, cancellationToken);

    /// <summary>
    /// Rol var mı, aynı yetki role zaten verilmiş mi?
    /// Generic CRUD üzerinden gelen tek kayıtlı ekleme/güncelleme de bu kontrolden geçer.
    /// </summary>
    private async Task<Result> EnsureAssignableAsync(
        Guid roleId, Permission permission, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (!await UnitOfWork.Repository<Role>().AnyAsync(x => x.Id == roleId, cancellationToken))
        {
            return Result.NotFound("Rol bulunamadı.");
        }

        var duplicate = await Repository.AnyAsync(
            x => x.RoleId == roleId
                 && x.Permission == permission
                 && (!excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken);

        return duplicate
            ? Result.Conflict("Bu yetki role zaten verilmiş.")
            : Result.Success();
    }
}
