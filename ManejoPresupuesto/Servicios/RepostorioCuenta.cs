using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCuenta
    {
        Task Crear(Cuenta cuenta);
    }
    public class RepostorioCuenta : IRepositorioCuenta
    {
        private readonly string connectionString;
        public RepostorioCuenta(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                                     @"INSERT INTO Cuentas(Nombre,TipoCuentaId, Balance, Descripcion)
                                      VALUES(@Nombre,@TipoCuentaId,@Balance,@Descripcion)
                                      SELECT SCOPE_IDENTITY();",cuenta);                                                     
            cuenta.Id = id;
        }
    }
}
