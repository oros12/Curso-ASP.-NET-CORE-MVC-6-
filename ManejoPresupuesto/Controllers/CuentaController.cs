using ManejoPresupuesto.Models;
using ManejoPresupuesto.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoPresupuesto.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServiciosUsuarios serviciosUsuarios;
        private readonly IRepositorioCuenta repositorioCuenta;
        public CuentaController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IRepositorioCuenta repositorioCuenta, IServiciosUsuarios serviciosUsuarios)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.serviciosUsuarios = serviciosUsuarios;
            this.repositorioCuenta = repositorioCuenta;
        }

        [HttpGet]
        public async Task <IActionResult> Crear()
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var modelo = new CuentaCreacionViewModel();
            modelo.TipoCuenta = await ObtenerTiposCuentas(usuarioId);
            
            return View(modelo);
        }
     
        [HttpPost]

        public async Task <IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = serviciosUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);
            if(tipoCuenta is null)
            {
                return RedirectToAction("NoExiste", "Home");
            }
            if(!ModelState.IsValid)
            {
                cuenta.TipoCuenta = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }
            await repositorioCuenta.Crear(cuenta);
            return RedirectToAction("Index");

        }
        public async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tipoCuenta = await repositorioTiposCuentas.Obtener(usuarioId);
            return tipoCuenta.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

    }
}
