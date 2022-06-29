using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    //Se usan interfaces para usar una clase que contiene metodos los cuales se piensen  implemente
    //en diferentes lugares, con mas o menos implementaciones per sin la necesidad de tener que 
    //invocar a la clase o a sus metodos cada que se necesiste
    public interface IRepositorioTiposCuentas
    {
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Validar(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Eliminar(int id);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString; 
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        //Se le indica async y task siendo async la indicacion de que sera un proceso 
        //asincrono, es decir, podremos realizar otras tareas aun cuando este no acabe
        //tendremos un task como valor de regreso para tenerlo como indicador de que se podra 
        //realizar el proceso de manera asincrona
        public async Task Crear (TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            //Se le  indica que regrese un valor de ese query, tal que el valor sea un int
            //Se va a extraer un dato de tipo entero, despues se realizar un insert
            //Para realizar la devolucion, se realiza un select, al final del query  
            //se indica con await la operacion que se piensar que tardara 
             var id = await connection.QuerySingleAsync<int>(@"INSERT INTO TiposCuentas (Nombre, UsuarioId, orden)
                                                    values (@Nombre, @UsuarioId, 0);
                                                    SELECT SCOPE_IDENTITY();", tipoCuenta);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Validar(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(
                                                       @"select 1
                                                       from TiposCuentas
                                                       where Nombre = @Nombre AND UsuarioId = @UsuarioId;",
                                                       new { nombre, usuarioId });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener( int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"select Id,  Nombre, UsuarioId, Orden
                                                from TiposCuentas
                                                where UsuarioId = @UsuarioId;"
                                               , new { usuarioId });
        }

        public async Task Actulizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"Update TipoCuentas
                                          set Nombre=@Nombre
                                          where Id=@Id AND UsuarioId=@UsuarioId", new { tipoCuenta });
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden, UsuarioId
                                                                            FROM TiposCuentas
                                                                            WHERE Id = @Id AND UsuarioId = @UsuarioId;",
                                                                            new { id, usuarioId });
        }
        public async Task Eliminar(int id )
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"delete TiposCuentas
                                            where Id=@Id;",
                                            new { id });
        }
    }


}
