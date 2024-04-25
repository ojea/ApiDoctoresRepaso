using ApiCoreOAuthDoctores.Models;
using ApiCoreOAuthDoctores.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiCoreOAuthDoctores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private DoctorRepository repo;

        public DoctorController(DoctorRepository repo)
        {
            this.repo = repo;
        }

        //GET DOCTORES

        [HttpGet]
        public async Task<ActionResult<List<Doctor>>> GetDoctores()
        {
            return await this.repo.GetDoctoresAsync();
        }

        //FIND EMPLEADO

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> FindEmpleado(int id)
        {
            return await this.repo.FindUserAsync(id);
        }

        //PERFIL DEL DOCTOR

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Doctor>>
            PerfilDoctor()
        {
            //internamente cuando recibimos el token
            //el usuario es validado y almacena datos como
            //httpcontext.user.identity.isauthenticated
            //como hemos incluido la key de los claims,
            //automaticamente tbn tenemos dichos claims
            //como en las apps MVC
            Claim claim = HttpContext.User
                .FindFirst(x => x.Type == "UserData");
            //recuperamos el json del empleado
            string jsonDoctor = claim.Value;
            Doctor doctor =
                JsonConvert.DeserializeObject<Doctor>(jsonDoctor);
            return doctor;

        }

        //GET COMPIS DE ESPECIALIDAD

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Doctor>>>GetCompisEmpleado()
        {
            string jsonEmp = HttpContext.User
                .FindFirst(x => x.Type == "UserData").Value;
            Doctor doctor =
                JsonConvert.DeserializeObject<Doctor>(jsonEmp);
            List<Doctor> compis = await
                this.repo.GetCompisEspecialidadAsync(doctor.Especialidad);
            return compis;
        }

        //GET ESPECIALIDAD

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> Especialidad()
        {
            return await this.repo.GetEspecialidadAsync();
        }

        //GET DOCTORES POR ESPECIALIDAD

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Doctor>>>DoctoresEspecialidad([FromQuery] List<string> especialidad)
        {
            return await this.repo.GetDoctoresEspecialidadAsync(especialidad);
        }

    }
}
