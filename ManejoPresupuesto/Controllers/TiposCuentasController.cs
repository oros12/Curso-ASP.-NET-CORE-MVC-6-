using Dapper;
using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServiciosUsuarios serviciosUsuarios;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas
        , IServiciosUsuarios serviciosUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.serviciosUsuarios = serviciosUsuarios;
            
         }

        public async Task<IActionResult> Index()
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var tiposCuentas= await repositorioTiposCuentas.Obtener(usuarioId);
            return View(tiposCuentas);
        }

        
        public IActionResult Crear()
        {
       
            return View();
        }

        

        [HttpPost]
        //Se le indica a la accion que contendra un proceso que se trabajara de manera asincrona
        public async Task <IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }
            tipoCuenta.UsuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var validacion = await repositorioTiposCuentas.Validar(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            if (validacion)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre}" +
                    $" ya existe");
                return View(tipoCuenta);
            }
            await repositorioTiposCuentas.Crear(tipoCuenta);
            
            
            return RedirectToAction("Index");
            
        }
        [HttpGet]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioid = serviciosUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioid);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoExiste", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpGet]
        public async Task <IActionResult> Eliminar(int id)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var tipoCuenta= await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if(tipoCuenta is null)
            {
                return RedirectToAction("NoExiste", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]

        public async Task <IActionResult> EliminarTipoCuenta(int id)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            if(tipoCuenta is null)
            {
                return RedirectToAction("NoExiste", "Home");

            }
            await repositorioTiposCuentas.Eliminar(id);
            return RedirectToAction("Index");
        }

        public async Task <IActionResult> ValidadorJson(string nombre)
        {
          
            var usuarioId = 1;
            var validador = await repositorioTiposCuentas.Validar(nombre, usuarioId); 
            if(validador)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);

        }
        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            return Ok();
        }
    }
}
