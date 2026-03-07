using AutoMapper;
using Microsoft.Extensions.Logging;
using Sekka.Core.Common;
using Sekka.Core.Common.Messages;
using Sekka.Core.DTOs.Common;
using Sekka.Core.Interfaces.Persistence;
using Sekka.Core.Specifications;
using Sekka.Persistence.Entities.Base;

namespace Sekka.Application.Services.Base;

public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : AuditableEntity<Guid>
{
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IMapper _mapper;
    protected readonly ILogger _logger;
    protected readonly IGenericRepository<TEntity, Guid> _repo;

    protected BaseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _repo = unitOfWork.GetRepository<TEntity, Guid>();
    }

    public virtual async Task<Result<PagedResult<TDto>>> GetPagedAsync(
        ISpecification<TEntity> spec, PaginationDto pagination)
    {
        var items = await _repo.ListAsync(spec);
        var total = await _repo.CountAsync(spec);
        var mapped = _mapper.Map<List<TDto>>(items);
        return Result<PagedResult<TDto>>.Success(
            new PagedResult<TDto>(mapped, total, pagination.Page, pagination.PageSize));
    }

    public virtual async Task<Result<TDto>> GetByIdAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return Result<TDto>.NotFound(ErrorMessages.ItemNotFound);
        return Result<TDto>.Success(_mapper.Map<TDto>(entity));
    }

    public virtual async Task<Result<TDto>> CreateAsync(TCreateDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);
        await _repo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation("Created {Entity} with Id {Id}", typeof(TEntity).Name, entity.Id);
        return Result<TDto>.Success(_mapper.Map<TDto>(entity));
    }

    public virtual async Task<Result<TDto>> UpdateAsync(Guid id, TUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return Result<TDto>.NotFound(ErrorMessages.ItemNotFound);
        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        _repo.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return Result<TDto>.Success(_mapper.Map<TDto>(entity));
    }

    public virtual async Task<Result<bool>> SoftDeleteAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null) return Result<bool>.NotFound(ErrorMessages.ItemNotFound);

        if (entity is SoftDeletableEntity<Guid> softDeletable)
        {
            softDeletable.IsDeleted = true;
            softDeletable.DeletedAt = DateTime.UtcNow;
            _repo.Update(entity);
        }
        else
        {
            _repo.Delete(entity);
        }

        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(true);
    }
}
