using BANCO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BANCO.Controllers
{
    public class MovimientoController : Controller
    {
        private readonly BancoContext _context;
        public MovimientoController(BancoContext context)
        {
            _context = context;
        }
        #region Hmovimientos

        // Ver el historial de movimientos
        [Authorize]
        public async Task<IActionResult> HistorialMovimientos(string idCuenta, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }

            var idUsuario = HttpContext.Session.GetString("IdUsuario");


            // Obtener todas las cuentas del usuario
            var cuentasUsuario = await _context.Cuentas.Where(c => c.IdUsuario == idUsuario).ToListAsync();

            var movimientosCuentaUser = new List<Movimiento>();
            foreach (var cuenta in cuentasUsuario)
            {
                var lista = _context.Movimientos
                    .Where(m => m.IdCuenta == cuenta.Id).ToList();

                movimientosCuentaUser.AddRange(lista);
            }

            ViewBag.Cuentas = new SelectList(cuentasUsuario, "Id", "Numero"); //Viewbag de todas las cuentas de usuario

            //ViewBag.IdCuentas = idCuenta;
            // Filtro de Historial + Display de Historial

            //var query = _context.Movimientos.AsQueryable();
            var query = movimientosCuentaUser.AsQueryable();

            var filtroCuenta = await _context.Cuentas.FindAsync(idCuenta);

            if (!string.IsNullOrEmpty(idCuenta))
            {

                query = query.Where(m => m.IdCuenta == idCuenta);

            }

            if (fechaDesde != null)
            {
                query = query.Where(m => m.Fecha >= fechaDesde);
            }

            if (fechaHasta != null)
            {
                // Agregar un día adicional a la fecha seleccionada en "Hasta"
                var fechaHastaInclusive = fechaHasta.Value.AddDays(1);
                query = query.Where(m => m.Fecha < fechaHastaInclusive);
            }

            var movimientosCuenta = query.ToList();

            return View(movimientosCuenta);
        }

        // Ver los movimientos por número de cuenta
        [Authorize]
        public async Task<IActionResult> MovimientosPorCuenta(string idCuenta, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }

            // Obtener el ID del usuario logueado
            var idUsuario = HttpContext.Session.GetString("IdUsuario");

            // Obtener todas las cuentas del usuario
            var cuentasUsuario = await _context.Cuentas.Where(c => c.IdUsuario == idUsuario).ToListAsync();

            if (!string.IsNullOrEmpty(idCuenta))
            {
                // Filtrar los movimientos por número de cuenta especificado y fechas
                var query = _context.Movimientos.Where(m => m.IdCuenta == idCuenta);

                if (fechaDesde.HasValue)
                    query = query.Where(m => m.Fecha.Date >= fechaDesde.Value.Date);

                if (fechaHasta.HasValue)
                    query = query.Where(m => m.Fecha.Date <= fechaHasta.Value.Date);

                var movimientosCuenta = await query.ToListAsync();

                ViewBag.SelectedCuenta = cuentasUsuario.FirstOrDefault(c => c.Id == idCuenta);
                ViewBag.SelectedMovimientos = movimientosCuenta;

                return View();
            }
            else
            {
                // Mostrar los movimientos por cada cuenta del usuario
                var movimientosPorCuenta = new List<Movimiento>();

                foreach (var cuenta in cuentasUsuario)
                {
                    var movimientosCuenta = await _context.Movimientos
                        .Where(m => m.IdCuenta == cuenta.Id)
                        .ToListAsync();

                    movimientosPorCuenta.AddRange(movimientosCuenta);
                }

                return View(movimientosPorCuenta);
            }
        }

        #endregion

        #region CRUD
        // GET: Cuentas/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Movimientos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuenta == null)
            {
                return NotFound();
            }

            return View(cuenta);
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movimiento = await _context.Movimientos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }

        // POST: Cuentas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);
            _context.Movimientos.Remove(movimiento);
            await _context.SaveChangesAsync();
            return RedirectToAction("HistorialMovimientos");
        }
        #endregion

        #region PDF
        public IActionResult PrintPDF(string idCuenta)
        {

            var idUsuario = HttpContext.Session.GetString("IdUsuario");

            // Obtener la cuenta del usuario
            var cuentaUsuario = _context.Cuentas.FirstOrDefault(c => c.IdUsuario == idUsuario && c.Id == idCuenta);

            // Manejo del caso de que no exista una cuenta
            if (cuentaUsuario == null)
            {
                return NotFound();
            }

            //Obtener los movimientos de la cuenta
            var movimientosCuenta = _context.Movimientos.Where(m => m.IdCuenta == idCuenta).ToList();

            //Manejo del caso de que no existan movimientos

            if (movimientosCuenta.Count == 0)
            {
                // Maneja el caso cuando no hay movimientos en la cuenta
                var model1 = new PrintPDFModel
                {
                    Cuenta = cuentaUsuario,
                    Movimientos = null
                };

                return new ViewAsPdf("PrintPDF", model1)
                {
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    FileName = "Movimientos_de_cuenta.pdf",
                    PageMargins = new Rotativa.AspNetCore.Options.Margins(20, 20, 20, 20)
                };
            }

            var model = new PrintPDFModel
            {
                Cuenta = cuentaUsuario,
                Movimientos = movimientosCuenta
            };

            return new ViewAsPdf("PrintPDF", model)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                FileName = "Movimientos_de_cuenta.pdf",
                PageMargins = new Rotativa.AspNetCore.Options.Margins(20, 20, 20, 20)
            };


        }
        #endregion
    }
}
