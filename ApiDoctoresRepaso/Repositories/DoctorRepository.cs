using ApiCoreOAuthDoctores.Data;
using ApiCoreOAuthDoctores.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCoreOAuthDoctores.Repository
{
    public class DoctorRepository
    {
        private DoctoresContext context;

        public DoctorRepository(DoctoresContext context)
        {
            this.context = context;
        }

        //GET DOCTORES
        public async Task<List<Doctor>> GetDoctoresAsync()
        {
            return await this.context.Doctores.ToListAsync();
        }

        //FIND DOCTORES
        public async Task<Doctor> FindUserAsync(int iddoctor)
        {
            var consulta = from datos in this.context.Doctores
                           where datos.IdDoctor == (iddoctor)
                           select datos;
            Doctor doctor = await consulta.FirstOrDefaultAsync();
            return doctor;
        }

        //GET MAX ID
        private async Task<int> GetMaxIdUsuarioAsync()
        {
            if (this.context.Doctores.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await
                    this.context.Doctores.MaxAsync(z => z.IdDoctor) + 1;
            }
        }

        //GET COMPIS HOSPITAL
        public async Task<List<Doctor>> GetCompisEspecialidadAsync(string especialidad)
        {
            return await this.context.Doctores
                .Where(z => z.Especialidad == especialidad)
                .ToListAsync();
        }

        //LOGIN EMPLEADO
        public async Task<Doctor>LoginDoctorAsync(string apellido, int idDoctor)
        {
            return await this.context.Doctores
                .Where(x => x.Apellido == apellido
                && 
                x.IdDoctor == idDoctor) .FirstOrDefaultAsync();
        }

        //GET ESPECIALIDAD
        public async Task<List<string>> GetEspecialidadAsync()
        {
            var consulta = (from datos in this.context.Doctores
                            select datos.Especialidad).Distinct();
            return await consulta.ToListAsync();
        }
        public async Task<List<Doctor>>GetDoctoresEspecialidadAsync(List<string> especialidad)
        {
            var consulta = from datos in this.context.Doctores
                           where especialidad.Contains(datos.Especialidad)
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}
