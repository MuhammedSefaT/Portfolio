using AutoMapper;
using FluentValidation;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.UserRoles;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Services;

public class UserRoleService
    : Service<UserRole, UserRoleDto, UserRoleCreateDto, UserRoleUpdateDto>, IUserRoleService
{
    public UserRoleService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEnumerable<IValidator<UserRoleCreateDto>> createValidators,
        IEnumerable<IValidator<UserRoleUpdateDto>> updateValidators)
        : base(unitOfWork, mapper, createValidators, updateValidators)
    {
    }

    protected override string NotFoundMessage => "Rol ataması bulunamadı.";

    public async Task<Result<IReadOnlyList<UserRoleDetailDto>>> GetByUserAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await UnitOfWork.Repository<User>().GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result<IReadOnlyList<UserRoleDetailDto>>.NotFound("Kullanıcı bulunamadı.");
        }

        var assignments = await Repository.GetWhereAsync(x => x.UserId == userId, cancellationToken);
        var roles = await UnitOfWork.Repository<Role>().GetAllAsync(cancellationToken);

        IReadOnlyList<UserRoleDetailDto> items = assignments
            .Select(assignment => new UserRoleDetailDto(
                assignment.Id,
                assignment.UserId,
                user.UserName,
                assignment.RoleId,
                roles.FirstOrDefault(role => role.Id == assignment.RoleId)?.Name ?? "(silinmiş rol)",
                assignment.AssignedAt))
            .OrderBy(item => item.RoleName)
            .ToList();

        return Result<IReadOnlyList<UserRoleDetailDto>>.Success(items);
    }

    public async Task<Result<IReadOnlyList<Guid>>> GetRoleIdsAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var assignments = await Repository.GetWhereAsync(x => x.UserId == userId, cancellationToken);

        IReadOnlyList<Guid> roleIds = assignments.Select(x => x.RoleId).Distinct().ToList();

        return Result<IReadOnlyList<Guid>>.Success(roleIds);
    }

    public async Task<Result> AssignAsync(UserRoleAssignDto dto, CancellationToken cancellationToken = default)
    {
        if (!await UnitOfWork.Repository<User>().AnyAsync(x => x.Id == dto.UserId, cancellationToken))
        {
            return Result.NotFound("Kullanıcı bulunamadı.");
        }

        var requestedRoleIds = (dto.RoleIds ?? Array.Empty<Guid>()).Distinct().ToList();

        // Var olmayan bir rol atanmaya çalışılıyorsa hiçbir değişiklik yapılmaz.
        var roles = await UnitOfWork.Repository<Role>().GetAllAsync(cancellationToken);

        if (requestedRoleIds.Any(id => roles.All(role => role.Id != id)))
        {
            return Result.Invalid(nameof(dto.RoleIds), "Atanmak istenen rollerden biri bulunamadı.");
        }

        var current = await Repository.GetWhereAsync(x => x.UserId == dto.UserId, cancellationToken);
        var currentRoleIds = current.Select(x => x.RoleId).ToList();

        var toAdd = requestedRoleIds.Except(currentRoleIds).ToList();
        var toRemove = currentRoleIds.Except(requestedRoleIds).ToList();

        if (toAdd.Count == 0 && toRemove.Count == 0)
        {
            return Result.Success("Değişiklik yok.");
        }

        if (toRemove.Count > 0)
        {
            await Repository.DeleteWhereAsync(
                x => x.UserId == dto.UserId && toRemove.Contains(x.RoleId), cancellationToken);
        }

        if (toAdd.Count > 0)
        {
            await Repository.AddRangeAsync(
                toAdd.Select(roleId => new UserRole { UserId = dto.UserId, RoleId = roleId }), cancellationToken);
        }

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Roller güncellendi.");
    }

    public async Task<Result> RemoveAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var removed = await Repository.DeleteWhereAsync(
            x => x.UserId == userId && x.RoleId == roleId, cancellationToken);

        if (removed == 0)
        {
            return Result.NotFound("Bu kullanıcıya ait böyle bir rol ataması yok.");
        }

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Rol ataması kaldırıldı.");
    }

    protected override Task<Result> OnCreatingAsync(
        UserRoleCreateDto dto, UserRole entity, CancellationToken cancellationToken)
        => EnsureAssignableAsync(dto.UserId, dto.RoleId, null, cancellationToken);

    protected override Task<Result> OnUpdatingAsync(
        UserRoleUpdateDto dto, UserRole entity, CancellationToken cancellationToken)
        => EnsureAssignableAsync(dto.UserId, dto.RoleId, dto.Id, cancellationToken);

    /// <summary>
    /// Kullanıcı ve rol gerçekten var mı, aynı çift zaten atanmış mı?
    /// Generic CRUD üzerinden gelen tek kayıtlı ekleme/güncelleme de bu kontrolden geçer.
    /// </summary>
    private async Task<Result> EnsureAssignableAsync(
        Guid userId, Guid roleId, Guid? excludedId, CancellationToken cancellationToken)
    {
        if (!await UnitOfWork.Repository<User>().AnyAsync(x => x.Id == userId, cancellationToken))
        {
            return Result.NotFound("Kullanıcı bulunamadı.");
        }

        if (!await UnitOfWork.Repository<Role>().AnyAsync(x => x.Id == roleId, cancellationToken))
        {
            return Result.NotFound("Rol bulunamadı.");
        }

        var duplicate = await Repository.AnyAsync(
            x => x.UserId == userId
                 && x.RoleId == roleId
                 && (!excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken);

        return duplicate
            ? Result.Conflict("Bu rol kullanıcıya zaten atanmış.")
            : Result.Success();
    }
}
