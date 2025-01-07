using MachineTracking.Domain.DTOs.MachineHistory;
using MachineTracking.Domain.Interfaces.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MachineTracking.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MachineHistoryController : ControllerBase
    {
        private readonly IMachineHistoryService _machineHistoryService;

        public MachineHistoryController(IMachineHistoryService machineHistoryService)
        {
            _machineHistoryService = machineHistoryService;
        }

        [HttpGet("GetLastestDataOfAllMachinesAsync")]
        public async Task<IActionResult> GetLastestDataOfAllMachinesAsync()
        {
            var response = await _machineHistoryService.GetLastestDataOfAllMachinesAsync();
            return Ok(response);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _machineHistoryService.GetByIdAsync(id);
            return response is not null ? Ok(response) : NotFound();
        }

        [HttpGet("GetMachineHistoriesAsync")]
        public async Task<IActionResult> GetMachineHistoriesAsync([FromQuery] MachineHistoryGetRequestDTO machineHistoryGetRequestDTO)
        {
            var record = await _machineHistoryService.GetMachineHistoriesAsync(machineHistoryGetRequestDTO);
            return record is not null ? Ok(record) : NotFound();
        }

        [HttpPost("InsertMachineHistory")]
        public async Task<IActionResult> Create([FromBody] MachineHistoryDTO machineHistory)
        {
            await _machineHistoryService.AddAsync(machineHistory);
            return Ok();
        }
    }
}
