using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.UserRoles;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Services;

public class UserRoleService : IUserRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserRoleService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    private IRepository<UserRole> UserRoles => _unitOfWork.Repository<UserRole>();

    public async Task<Result<IReadOnlyList<UserRoleDto>>> GetByUserAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result<IReadOnlyList<UserRoleDto>>.NotFound("Kullanıcı bulunamadı.");
        }

        var assignments = await UserRoles.GetWhereAsync(x => x.UserId == userId, cancellationToken);
        var roles = await _unitOfWork.Repository<Role>().GetAllAsync(cancellationToken);

        // Entity'lerde navigation property olmadığı için ad alanları burada birleştiriliyor.
        var items = assignments
            .Select(assignment => new UserRoleDto(
                assignment.Id,
                assignment.UserId,
                user.UserName,
                assignment.RoleId,
                roles.FirstOrDefault(role => role.Id == assignment.RoleId)?.Name ?? "(silinmiş rol)",
                assignment.AssignedAt))
            .OrderBy(item => item.RoleName)
            .ToList();

        return Result<IReadOnlyList<UserRoleDto>>.Success(items);
    }

    public async Task<Result<IReadOnlyList<Guid>>> GetRoleIdsAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var assignments = await UserRoles.GetWhereAsync(x => x.UserId == userId, cancellationToken);

        IReadOnlyList<Guid> roleIds = assignments.Select(x => x.RoleId).Distinct().ToList();

        return Result<IReadOnlyList<Guid>>.Success(roleIds);
    }

    public async Task<Result> AssignAsync(UserRoleAssignDto dto, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Repository<User>().AnyAsync(x => x.Id == dto.UserId, cancellationToken))
        {
            return Result.NotFound("Kullanıcı bulunamadı.");
        }

        var requestedRoleIds = (dto.RoleIds ?? Array.Empty<Guid>()).Distinct().ToList();
        var roles = await _unitOfWork.Repository<Role>().GetAllAsync(cancellationToken);

        // Var olmayan bir rol atanmaya çalışılıyorsa hiçbir değişiklik yapılmaz.
        var missingRoleId = requestedRoleIds.FirstOrDefault(id => roles.All(role => role.Id != id));

        if (missingRoleId != Guid.Empty)
        {
            return Result.Invalid(nameof(dto.RoleIds), "Atanmak istenen rollerden biri bulunamadı.");
        }

        var current = await UserRoles.GetWhereAsync(x => x.UserId == dto.UserId, cancellationToken);
        var currentRoleIds = current.Select(x => x.RoleId).ToList();

        var toAdd = requestedRoleIds.Except(currentRoleIds).ToList();
        var toRemove = currentRoleIds.Except(requestedRoleIds).ToList();

        if (toAdd.Count == 0 && toRemove.Count == 0)
        {
            return Result.Success("Değişiklik yok.");
        }

        if (toRemove.Count > 0)
        {
            await UserRoles.DeleteWhereAsync(
                x => x.UserId == dto.UserId && toRemove.Contains(x.RoleId), cancellationToken);
        }

        if (toAdd.Count > 0)
        {
            await UserRoles.AddRangeAsync(
                toAdd.Select(roleId => new UserRole { UserId = dto.UserId, RoleId = roleId }), cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Roller güncellendi.");
    }

    public async Task<Result> RemoveAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var removed = await UserRoles.DeleteWhereAsync(
            x => x.UserId == userId && x.RoleId == roleId, cancellationToken);

        if (removed == 0)
        {
            return Result.NotFound("Bu kullanıcıya ait böyle bir rol ataması yok.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Rol ataması kaldırıldı.");
    }
}
