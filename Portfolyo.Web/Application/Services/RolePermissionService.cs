using AutoMapper;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.RolePermissions;
using Portfolyo.Web.Core.Entities;
using Portfolyo.Web.Core.Enums;

namespace Portfolyo.Web.Application.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RolePermissionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IRepository<RolePermission> RolePermissions => _unitOfWork.Repository<RolePermission>();

    public async Task<Result<IReadOnlyList<RolePermissionDto>>> GetByRoleAsync(
        Guid roleId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Repository<Role>().AnyAsync(x => x.Id == roleId, cancellationToken))
        {
            return Result<IReadOnlyList<RolePermissionDto>>.NotFound("Rol bulunamadı.");
        }

        var items = await RolePermissions.GetWhereAsync(x => x.RoleId == roleId, cancellationToken);

        return Result<IReadOnlyList<RolePermissionDto>>.Success(
            _mapper.Map<List<RolePermissionDto>>(items.OrderBy(x => x.Permission)));
    }

    public async Task<Result> AssignAsync(RolePermissionAssignDto dto, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Repository<Role>().AnyAsync(x => x.Id == dto.RoleId, cancellationToken))
        {
            return Result.NotFound("Rol bulunamadı.");
        }

        var requested = (dto.Permissions ?? Array.Empty<Permission>()).Distinct().ToList();

        // Enum'da tanımlı olmayan bir değer gönderilmişse hiçbir şey yazılmaz.
        var undefined = requested.FirstOrDefault(permission => !Enum.IsDefined(permission));

        if (undefined != default)
        {
            return Result.Invalid(nameof(dto.Permissions), "Tanımlı olmayan bir yetki gönderildi.");
        }

        var current = await RolePermissions.GetWhereAsync(x => x.RoleId == dto.RoleId, cancellationToken);
        var currentPermissions = current.Select(x => x.Permission).ToList();

        var toAdd = requested.Except(currentPermissions).ToList();
        var toRemove = currentPermissions.Except(requested).ToList();

        if (toAdd.Count == 0 && toRemove.Count == 0)
        {
            return Result.Success("Değişiklik yok.");
        }

        if (toRemove.Count > 0)
        {
            await RolePermissions.DeleteWhereAsync(
                x => x.RoleId == dto.RoleId && toRemove.Contains(x.Permission), cancellationToken);
        }

        if (toAdd.Count > 0)
        {
            await RolePermissions.AddRangeAsync(
                toAdd.Select(permission => new RolePermission { RoleId = dto.RoleId, Permission = permission }),
                cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Yetkiler güncellendi.");
    }

    public async Task<Result<IReadOnlyList<Permission>>> GetByUserAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var assignments = await _unitOfWork.Repository<UserRole>()
            .GetWhereAsync(x => x.UserId == userId, cancellationToken);

        if (assignments.Count == 0)
        {
            return Result<IReadOnlyList<Permission>>.Success(Array.Empty<Permission>());
        }

        var roleIds = assignments.Select(x => x.RoleId).ToHashSet();

        var rolePermissions = await RolePermissions
            .GetWhereAsync(x => roleIds.Contains(x.RoleId), cancellationToken);

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
}
