using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockManagement.Business.Filters;
using StockManagement.Business.Services.Abstract;

namespace StockManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [AuditLog]
    [ExceptionHandler]
    public class OutboxEventsController : ControllerBase
    {
        readonly IOutboxEventService _outboxEventService;
        public OutboxEventsController(IOutboxEventService outboxEventService)
        {
            _outboxEventService = outboxEventService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllOutboxEvents()
        {
            var result = await _outboxEventService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("get-successful")]
        public async Task<IActionResult> GetAllOutboxEventsBySuccessfullProcess()
        {
            var result = await _outboxEventService.GetAllBySuccessfullProcessAsync();
            return Ok(result);
        }

        [HttpGet("get-error")]
        public async Task<IActionResult> GetAllOutboxEventsByErrorProcess()
        {
            var result = await _outboxEventService.GetAllByErrorProcessAsync();
            return Ok(result);
        }

        [HttpGet("get-whole-data")]
        public async Task<IActionResult> GetAllWholeOutboxEvents()
        {
            var result = await _outboxEventService.GetAllAllDataAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOutboxEventById(int id)
        {
            var result = await _outboxEventService.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("hard-delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _outboxEventService.DeleteAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("delete-multiple")]
        public async Task<IActionResult> DeleteOutboxEventById(List<int> ids)
        {
            var result = await _outboxEventService.DeleteByIdAsync(ids);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-active/{id}")]
        public async Task<IActionResult> SetActive(int id)
        {
            var result = await _outboxEventService.SetActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("set-inactive/{id}")]
        public async Task<IActionResult> SetInActive(int id)
        {
            var result = await _outboxEventService.SetInActiveAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("soft-delete/{id}")]
        public async Task<IActionResult> SetDeleted(int id)
        {
            var result = await _outboxEventService.SetDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("restore/{id}")]
        public async Task<IActionResult> SetNotDeleted(int id)
        {
            var result = await _outboxEventService.SetNotDeletedAsync(id);
            if (!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
