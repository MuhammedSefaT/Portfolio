using AutoMapper;
using FluentValidation;
using Portfolyo.Web.Application.Abstractions.Repositories;
using Portfolyo.Web.Application.Abstractions.Services;
using Portfolyo.Web.Application.Common;
using Portfolyo.Web.Application.Common.Results;
using Portfolyo.Web.Application.DTOs.Roles;
using Portfolyo.Web.Core.Entities;

namespace Portfolyo.Web.Application.Services;

public class RoleService : Service<Role, RoleDto, RoleCreateDto, RoleUpdateDto>, IRoleService
{
    public RoleService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IEnumerable<IValidator<RoleCreateDto>> createValidators,
        IEnumerable<IValidator<RoleUpdateDto>> updateValidators)
        : base(unitOfWork, mapper, createValidators, updateValidators)
    {
    }

    protected override string NotFoundMessage => "Rol bulunamadı.";

    public async Task<Result<RoleDetailDto>> GetDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await Repository.GetByIdAsync(id, cancellationToken);

        if (role is null)
        {
            return Result<RoleDetailDto>.NotFound(NotFoundMessage);
        }

        // Yetkiler ayrı dosyada tutulduğu için burada birleştiriliyor.
        var rolePermissions = await UnitOfWork.Repository<RolePermission>()
            .GetWhereAsync(x => x.RoleId == id, cancellationToken);

        var permissions = rolePermissions
            .Select(x => x.Permission)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        return Result<RoleDetailDto>.Success(new RoleDetailDto(
            role.Id, role.Name, role.NormalizedName, role.Description, role.CreatedAt, permissions));
    }

    public async Task<Result<RoleDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalized = TextNormalizer.Normalize(name);

        var role = await Repository.GetFirstOrDefaultAsync(
            x => x.NormalizedName == normalized, cancellationToken);

        return role is null
            ? Result<RoleDto>.NotFound(NotFoundMessage)
            : Result<RoleDto>.Success(Mapper.Map<RoleDto>(role));
    }

    protected override Task<Result> OnCreatingAsync(
        RoleCreateDto dto, Role entity, CancellationToken cancellationToken)
        => EnsureUniqueNameAsync(entity.NormalizedName, null, cancellationToken);

    protected override Task<Result> OnUpdatingAsync(
        RoleUpdateDto dto, Role entity, CancellationToken cancellationToken)
        => EnsureUniqueNameAsync(entity.NormalizedName, dto.Id, cancellationToken);

    protected override async Task<Result> OnDeletingAsync(Role entity, CancellationToken cancellationToken)
    {
        // Rol silinince ona bağlı yetkiler ve kullanıcı atamaları da gitmeli.
        // Üç dosya da tek SaveChangesAsync çağrısında yazılır.
        await UnitOfWork.Repository<RolePermission>()
            .DeleteWhereAsync(x => x.RoleId == entity.Id, cancellationToken);

        await UnitOfWork.Repository<UserRole>()
            .DeleteWhereAsync(x => x.RoleId == entity.Id, cancellationToken);

        return Result.Success();
    }

    private async Task<Result> EnsureUniqueNameAsync(
        string normalizedName, Guid? excludedId, CancellationToken cancellationToken)
    {
        var exists = await Repository.AnyAsync(
            x => x.NormalizedName == normalizedName && (!excludedId.HasValue || x.Id != excludedId.Value),
            cancellationToken);

        return exists
            ? Result.Conflict("Bu isimde bir rol zaten var.")
            : Result.Success();
    }
}
