using ApiCoreOAuthDoctores.Models;
using ApiCoreOAuthDoctores.Repository;
using ApiCoreOAuthEmpleados.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ApiCoreOAuthDoctores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private DoctorRepository repo;
        private HelperActionServicesOAuth helper;

        public AuthController(DoctorRepository repo,
           HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            //buscamos al empleado en nuestro repo
            Doctor doctor = await this.repo.LoginDoctorAsync
                (model.UserName, int.Parse(model.Password));
            if (doctor == null)
            {
                return Unauthorized();
            }
            else
            {
                //debemos crear unas credenciales para incluuirlas
                //dentro del token y que estaran compuestas por el 
                //secret key cifrado y el tipo de cifrado que 
                //deseemos incluir en eñ token
                SigningCredentials credentials =
                    new SigningCredentials(
                        this.helper.GetKeyToken(),
                        SecurityAlgorithms.HmacSha256);

                //convertimos el emp a json
                string jsonEmpleado =
                    JsonConvert.SerializeObject(doctor);
                //creamos un array de claims con toda la info que 
                //queramos guardar en el token
                Claim[] info = new[]
                {
                    new Claim("UserData", jsonEmpleado)
                };

                //el token se genera con una clase y debemos indicar
                //los elementos que almacenará dentro de dicho token
                JwtSecurityToken token = new JwtSecurityToken(
                    claims: info,
                    issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    signingCredentials: credentials,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    notBefore: DateTime.UtcNow
                    );
                //por ultimo devolvemos una respuesta afirmativa 
                //con un objeto anonimo en formato JSON
                return Ok(
                    new
                    {
                        response =
                        new JwtSecurityTokenHandler()
                        .WriteToken(token)
                    });
            }
        }
    }
}
