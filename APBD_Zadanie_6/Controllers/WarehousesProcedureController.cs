using APBD_Task_6.Models;
using APBD_Zadanie_6.Services;
using Microsoft.AspNetCore.Mvc;

namespace APBD_Zadanie_6.Controllers
{
    [ApiController]
    [Route("api/warehouses2")]
    public class WarehousesProcedureController : ControllerBase
    {
        private readonly IWarehausesProcedureService _warehausesProcedureService;

        public WarehousesProcedureController(IWarehausesProcedureService warehausesProcedureService) 
        {
            _warehausesProcedureService = warehausesProcedureService;
        }

        [HttpPost]
        public async Task<IActionResult> AddProductToWarehouseAsync(ProductWarehouse productWarehouse)
        {
            int idProductWarehouse = await _warehausesProcedureService.AddProductToWarehouseAsync(productWarehouse);

            return Ok(idProductWarehouse);
        }
    }
}
