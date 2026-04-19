using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sekka.Application.Services.Base;
using Sekka.Core.Common;
using Sekka.Persistence.Entities.Base;

namespace Sekka.API.Controllers.Base;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
public abstract class BaseCrudController<TEntity, TDto, TCreateDto, TUpdateDto> : ControllerBase
    where TEntity : AuditableEntity<Guid>
{
    protected readonly BaseService<TEntity, TDto, TCreateDto, TUpdateDto> _service;

    protected BaseCrudController(BaseService<TEntity, TDto, TCreateDto, TUpdateDto> service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    public virtual async Task<IActionResult> GetById(Guid id)
        => ToActionResult(await _service.GetByIdAsync(id));

    [HttpPost]
    public virtual async Task<IActionResult> Create([FromBody] TCreateDto dto)
        => ToActionResult(await _service.CreateAsync(dto), StatusCodes.Status201Created);

    [HttpPut("{id:guid}")]
    public virtual async Task<IActionResult> Update(Guid id, [FromBody] TUpdateDto dto)
        => ToActionResult(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public virtual async Task<IActionResult> Delete(Guid id)
        => ToActionResult(await _service.SoftDeleteAsync(id));

    protected IActionResult ToActionResult<T>(Result<T> result, int successCode = 200)
    {
        if (result.IsSuccess)
            return StatusCode(successCode, ApiResponse<T>.Success(result.Value!));

        // Include Value as data in error response when available (e.g., bulk import partial results)
        var errorResponse = result.Value is not null
            ? new ApiResponse<T> { IsSuccess = false, Data = result.Value, Message = result.Error!.Message }
            : ApiResponse<T>.Fail(result.Error!.Message);

        return result.Error.Code switch
        {
            "NOT_FOUND" => NotFound(errorResponse),
            "UNAUTHORIZED" => Unauthorized(errorResponse),
            "CONFLICT" => Conflict(errorResponse),
            "NOT_IMPLEMENTED" => StatusCode(501, errorResponse),
            _ => BadRequest(errorResponse)
        };
    }
}
