using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;
using WebApiCasino.Filtros;
using WebApiCasino.Servicios;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("api/rifas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class RifasController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public RifasController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
         
        }


        [HttpGet]
        //[ServiceFilter(typeof(FiltroDeAccion))]
        public async Task<ActionResult<List<RifaDTO>>> Get()
        {

            var rifas = await dbContext.Rifas.ToListAsync();
            return mapper.Map<List<RifaDTO>>(rifas);
        }

        /*Tipos de mensajes de log:
         * Critical: Mensajes de mayor severidad para el sistema
         * Error
         * Warning
         * Information
         * Debug
         * Trace: Mensajes de menor categoria */



        [HttpGet("{id:int}", Name = "obtenerrifas")] //api/rifas/
        public async Task<ActionResult<RifaDTO>> Get(int id)
        {
            var rifa = await dbContext.Rifas.FirstOrDefaultAsync(rifaBD => rifaBD.Id == id);

            if (rifa == null)
            {
                return NotFound();
            }

            return mapper.Map<RifaDTO>(rifa);
        }



        [HttpGet("{nombre}")] //api/rifas/
        public async Task<ActionResult<List<RifaDTO>>> Get([FromRoute] string nombre)
        {
            var rifas = await dbContext.Rifas.Where(rifaBD => rifaBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<RifaDTO>>(rifas);
        }



        [HttpPost]
        public async Task<ActionResult> Post([FromBody] RifaCreacionDTO rifaCreacionDTO)
        {
            var existeRifaConNombresIguales = await dbContext.Rifas.AnyAsync(x => x.Nombre == rifaCreacionDTO.Nombre);

            if (existeRifaConNombresIguales)
            {
                return BadRequest($"Ya existe una rifa con ese nombre {rifaCreacionDTO.Nombre}");
            }

            var rifa = mapper.Map<Rifa>(rifaCreacionDTO);

            dbContext.Add(rifa);
            await dbContext.SaveChangesAsync();
            return Ok();
        }


    
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Rifas.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("La rifa a eliminar no fue encontrada.");
            }

            dbContext.Remove(new Rifa()
            {
                Id = id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}