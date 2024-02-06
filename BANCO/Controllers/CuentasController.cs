using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BANCO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace BANCO.Controllers
{
    public class CuentasController : Controller
    {
        private readonly BancoContext _context;

        public CuentasController(BancoContext context)
        {
            _context = context;
        }

        [Authorize]
        // GET: Cuentas
        // Ver el resumen de la cuenta
        public async Task<IActionResult> ResumenCuenta()  /*NO MUESTRA AL USUARIO EN USO*/
        {

            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }

            var idUsuario = HttpContext.Session.GetString("IdUsuario");
            var cuentasUsuario = await _context.Cuentas.Where(c => c.IdUsuario == idUsuario).ToListAsync();

            ViewBag.Usuario = User.Identity.Name; // Obtener el nombre de usuario autenticado

            return View(cuentasUsuario);
        }

        [Authorize]
        public async Task<IActionResult> Transferencia()
        {
            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }

            var idUsuario = Convert.ToString(HttpContext.Session.GetString("IdUsuario"));
            var cuentasUsuario = await _context.Cuentas.Where(c => c.IdUsuario == idUsuario).ToListAsync();

            ViewBag.Usuario = User.Identity.Name; // Obtener el nombre de usuario autenticado
            ViewBag.Cuentas = cuentasUsuario;

            //Obtener el tipo de movimiento Correcto
            ViewBag.TiposMovimiento = Enum.GetValues(typeof(Tipo_Movimiento));

            var CuentaUsuarioDestino = await _context.Cuentas.Where(c => c.IdUsuario != idUsuario).ToListAsync();

            ViewBag.CuentasDestino = CuentaUsuarioDestino; //Pasar la cuenta de usuario destino

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Transferencia(string IdCuentaOrigen, string IdCuentaDestino, string concepto, double monto, Tipo_Movimiento TipoMov)
        {
            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }

            var cuentaOrigen = await _context.Cuentas.FindAsync(IdCuentaOrigen);
            //var cuentaDestino = await _context.Cuentas.FindAsync(IdCuentaDestino);

            if (cuentaOrigen == null /*|| cuentaDestino == null*/)
            {

                return RedirectToAction("ResumenCuenta", "Cuentas");
            }

            if (cuentaOrigen.SaldoActual < monto)
            {
                ViewBag.Error = "Saldo insuficiente, no se pudo realizar la transacción";
                return RedirectToAction("Transferencia");
            }

            // Verificar si la cuenta destino pertenece al mismo usuario o a otro usuario
            var cuentaDestino = await _context.Cuentas.FirstOrDefaultAsync(c => c.Numero == IdCuentaDestino);
            if (cuentaDestino == null)
            {
                ModelState.AddModelError("IdCuentaDestino", "La cuenta destino no existe o no es válida.");
                var cuentasUsuario = await _context.Cuentas.Where(c => c.IdUsuario == HttpContext.User.Identity.Name).ToListAsync();
                ViewBag.CuentasOrigen = cuentasUsuario;
                return RedirectToAction("Transferencia");
            }

            // Realiza la transferencia
            cuentaOrigen.SaldoActual -= monto;
            cuentaDestino.SaldoActual += monto;

            // Obtener el ID del usuario logueado
            var idUsuario = Convert.ToString(HttpContext.Session.GetString("IdUsuario"));

            await _context.SaveChangesAsync();

            // Crear el objeto Movimiento con el concepto adecuado
            var movimientoOrigen = new Movimiento
            {
                IdCuenta = cuentaOrigen.Id,
                Monto = -monto,
                Concepto = concepto,
                Fecha = DateTime.Now,
                Tipo = TipoMov,
                IdCuentaOrigen = cuentaOrigen.Numero,
                IdCuentaDestino = cuentaDestino.Numero

            };

            var movimientoDestino = new Movimiento
            {
                IdCuenta = cuentaDestino.Id,
                Monto = monto,
                Concepto = concepto,
                Fecha = DateTime.Now,
                Tipo = TipoMov,
                IdCuentaOrigen = cuentaDestino.Numero,
                IdCuentaDestino = cuentaOrigen.Numero

            };

            _context.Movimientos.Add(movimientoOrigen);
            _context.Movimientos.Add(movimientoDestino);
            await _context.SaveChangesAsync();


            return RedirectToAction("TransferenciaExitosa");
        }

        public IActionResult TransferenciaExitosa()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> PagoServicio()
        {
            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }

            var idUsuario = HttpContext.Session.GetString("IdUsuario");

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == idUsuario);

            var cuentas = await _context.Cuentas.Where(c => c.IdUsuario == usuario.Id).ToListAsync();

            ViewBag.Usuario = usuario.Nombre; // Pasa el nombre del usuario a la ViewBag
            ViewBag.Cuentas = cuentas;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PagoServicio(string IdCuenta, double monto, Tipo_PagoServicio tipoServicio)
        {
            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }

            var cuenta = await _context.Cuentas.FindAsync(IdCuenta);

            if (cuenta == null)
            {
                return RedirectToAction("ResumenCuenta", "Cuentas");
            }

            if (cuenta.SaldoActual < monto)
            {
                ViewBag.Error = "Saldo insuficiente. No fue posible realizar el pago";
                return RedirectToAction("PagoServicio");
            }

            // Realizar el pago
            cuenta.SaldoActual -= monto;

            await _context.SaveChangesAsync();

            // Crear el objeto Movimiento con el concepto adecuado
            var movimiento = new Movimiento
            {
                IdCuenta = cuenta.Id,
                Monto = -monto,
                Concepto = "Pago de servicio.",
                Fecha = DateTime.Now,
                Tipo = Tipo_Movimiento.Pago,
                TipoServicio = tipoServicio,
                IdCuentaOrigen = cuenta.Numero
            };

            // Agregar el movimiento a la base de datos
            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return RedirectToAction("PagoServicioExitoso");
        }


        public IActionResult PagoServicioExitoso()
        {
            return View();
        }

        // GET: Cuentas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Cuentas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuenta == null)
            {
                return NotFound();
            }

            return View(cuenta);
        }

        // GET: Cuentas/Create
        public IActionResult Create()
        {
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "Id", "Nombre");
            return View();
        }

        // POST: Cuentas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Numero,IdUsuario,SaldoActual,Tipo,TipoServicio,Id,Nombre")] Cuenta cuenta)
        {
            if (ModelState.IsValid)
            {
                var idUsuario = HttpContext.Session.GetString("IdUsuario");

                if (idUsuario == null)
                {
                    return RedirectToAction("InicioSesion", "Usuario");
                }

                cuenta.IdUsuario = idUsuario;

                _context.Add(cuenta);
                await _context.SaveChangesAsync();
                return RedirectToAction("ResumenCuenta");
            }

            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "Id", "Id", cuenta.IdUsuario);
            return View(cuenta);
        }

        // GET: Cuentas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Cuentas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuenta == null)
            {
                return NotFound();
            }

            return View(cuenta);
        }

        // POST: Cuentas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            _context.Cuentas.Remove(cuenta);
            await _context.SaveChangesAsync();
            return RedirectToAction("ResumenCuenta");
        }

        private bool CuentaExists(string id)
        {
            return _context.Cuentas.Any(e => e.Id == id);
        }
    }
}
