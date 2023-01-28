using luanvanthacsi.Api.Repositores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace luanvanthacsi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScientistController : ControllerBase
    {
        private readonly IScientistRepository _scientistRepository;

        public ScientistController(IScientistRepository scientistRepository)
        {
            _scientistRepository = scientistRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var scientists = await _scientistRepository.GetScientistsAsync();
            return Ok(scientists);
        }

    }
}
