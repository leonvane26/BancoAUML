using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BANCO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using BANCO.Models.Extension_InicioSesion;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace BANCO.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly BancoContext _context;

        public UsuarioController(BancoContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region Registro e InicioSesion

        // Registrar un nuevo usuario
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Encriptar la contraseña en texto plano
                usuario.Contraseña = EncriptarContraseña(usuario.Contraseña);

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction("InicioSesion");
            }

            return View(usuario);
        }

        private string EncriptarContraseña(string contraseña)
        {
            // Generar una sal aleatoria
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Generar el hash de la contraseña utilizando bcrypt
            var hashedContraseña = new Rfc2898DeriveBytes(contraseña, salt, 10000).GetBytes(20);

            // Concatenar la sal y el hash en una sola cadena
            var saltedHash = new byte[36];
            Array.Copy(salt, 0, saltedHash, 0, 16);
            Array.Copy(hashedContraseña, 0, saltedHash, 16, 20);

            // Convertir el resultado a una cadena base64 para su almacenamiento
            var contraseñaEncriptada = Convert.ToBase64String(saltedHash);

            return contraseñaEncriptada;
        }

        // Iniciar Sesion
        public IActionResult InicioSesion()
        {
            // Reiniciar el contador de intentos y eliminar la variable de sesión de tiempo de espera
            HttpContext.Session.SetInt32("IntentosRestantes", 3);
            HttpContext.Session.Remove("TiempoEspera");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InicioSesion(string username, string contraseña)
        {
            // Verificar si el usuario está bloqueado
            var bloqueado = HttpContext.Session.GetBool("Bloqueado") ?? false;

            if (bloqueado)
            {
                // Obtener el tiempo de desbloqueo
                var tiempoDesbloqueo = HttpContext.Session.GetDateTime("TiempoDesbloqueo") ?? DateTime.MinValue;

                // Verificar si aún no ha pasado el tiempo de espera
                if (DateTime.Now < tiempoDesbloqueo)
                {
                    // Redirigir a la vista de AccesoDenegado
                    ViewBag.TiempoEspera = (int)(tiempoDesbloqueo - DateTime.Now).TotalSeconds;
                    return View("AccesoDenegado");
                }
                else
                {
                    // Restablecer el estado de bloqueo
                    HttpContext.Session.SetBool("Bloqueado", false);
                    HttpContext.Session.Remove("TiempoDesbloqueo");
                }
            }

            #region Autenticacion_Usuario
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Username == username);

            if (usuario != null && VerificarContraseña(contraseña, usuario.Contraseña))
            {
                // Autenticar al usuario
                var claims = new List<Claim>
                 {
                //Claims
                new Claim(ClaimTypes.Name, usuario.Username),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                HttpContext.Session.SetString("IdUsuario", usuario.Id.ToString());
                HttpContext.Session.SetInt32("SesionIniciada", 1);
                #endregion
                // Restablecer el estado de bloqueo y los intentos restantes
                HttpContext.Session.SetBool("Bloqueado", false);
                HttpContext.Session.SetInt32("IntentosRestantes", 3);

                return RedirectToAction("ResumenCuenta", "Cuentas");
            }
            else
            {
                // Decrementar el contador de intentos restantes
                var intentosRestantes = HttpContext.Session.GetInt32("IntentosRestantes") ?? 3;
                intentosRestantes--;

                // Verificar si se agotaron los intentos
                if (intentosRestantes <= 0)
                {
                    // Bloquear el usuario por un tiempo determinado
                    var tiempoDesbloqueo = DateTime.Now.AddSeconds(30);
                    HttpContext.Session.SetBool("Bloqueado", true);
                    HttpContext.Session.SetDateTime("TiempoDesbloqueo", tiempoDesbloqueo);

                    // Redirigir a la vista de AccesoDenegado
                    ViewBag.TiempoEspera = 30;
                    return View("AccesoDenegado");
                }

                // Actualizar el contador de intentos restantes
                HttpContext.Session.SetInt32("IntentosRestantes", intentosRestantes);

                ViewBag.Error = "Usuario o contraseña incorrectos.";

                return View();
            }

        }

        private bool VerificarContraseña(string contraseña, string contraseñaEncriptada)
        {
            // Convertir la cadena base64 de la contraseña encriptada a bytes
            var saltedHash = Convert.FromBase64String(contraseñaEncriptada);

            // Extraer la sal y el hash de la cadena
            var salt = new byte[16];
            var hash = new byte[20];
            Array.Copy(saltedHash, 0, salt, 0, 16);
            Array.Copy(saltedHash, 16, hash, 0, 20);

            // Generar el hash de la contraseña ingresada utilizando la sal almacenada
            var hashedContraseña = new Rfc2898DeriveBytes(contraseña, salt, 10000).GetBytes(20);

            // Comparar los hashes
            for (int i = 0; i < 20; i++)
            {
                if (hash[i] != hashedContraseña[i])
                {
                    return false;
                }
            }

            return true;
        }


        public IActionResult AccesoDenegado()
        {
            return View();
        }

        // Cerrar Sesion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CerrarSesion()
        {
            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }
            var userName = User.Identity.Name; //Obtener el nombre de usuario logueado
            ViewBag.Usuario = userName;

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

     #endregion

        #region Actualizar Informacion
        //Vista para actualizar la contraseña del usuario
        public IActionResult ActualizarPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ActualizarPassword(string password, string nuevapassword, string confirmarpassword)
        {
            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }

            // Obtener usuario actual
            var idUsuario = HttpContext.Session.GetString("IdUsuario");
            var usuario = await _context.Usuarios.FindAsync(idUsuario);

            // Verificar que la contraseña antigua sea correcta
            if (!VerificarContraseña(password, usuario.Contraseña))
            {
                ModelState.AddModelError("password", "La contraseña antigua no es correcta.");
                return View();
            }

            // Verificar que la nueva contraseña y la confirmación sean las mismas
            if (nuevapassword != confirmarpassword)
            {
                ModelState.AddModelError("confirmarpassword", "La nueva contraseña no coincide con la confirmación de la contraseña");
                return View();
            }

            // Encriptar y actualizar la contraseña del usuario
            usuario.Contraseña = EncriptarContraseña(nuevapassword);
            await _context.SaveChangesAsync();

            return RedirectToAction("ActualizacionContraseñaExitosa");
        }

        public IActionResult ActualizarDatos()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ActualizarDatos(string telefono, string correo)
        {
            var sesionIniciada = HttpContext.Session.GetInt32("SesionIniciada");

            if (sesionIniciada == null || sesionIniciada == 0)
            {
                return RedirectToAction("InicioSesion", "Usuario");
            }
            //Obtener usuario actual
            var idUsuario = HttpContext.Session.GetString("IdUsuario");
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            //Validaciones de datos vacios
            if (string.IsNullOrEmpty(telefono))
            {
                telefono = usuario.Telefono;

            }

            if (string.IsNullOrEmpty(correo))
            {
                correo = usuario.CorreoElectronico;
            }
            //Validacion del correo
            var email = new EmailAddressAttribute();
            if (!email.IsValid(correo))
            {
                ModelState.AddModelError("correo", "El formato del correo no es valido");
                return View();
            }
            //Actualizar datos del telefono y correo electronico de usuario
            usuario.Telefono = telefono;
            usuario.CorreoElectronico = correo;

            await _context.SaveChangesAsync();

            return RedirectToAction("ActualizacionExitosa");

        }
        public IActionResult ActualizacionExitosa()
        {
            // Obtener el identificador del usuario actual desde alguna fuente, como el contexto de la sesión o el token de autenticación
            var idUsuario = HttpContext.Session.GetString("IdUsuario");
            var usuarioActualizado = _context.Usuarios.FirstOrDefault(u => u.Id == idUsuario);

            return View(usuarioActualizado);
        }

        public IActionResult ActualizacionContraseñaExitosa()
        {
            return View();
        }
        #endregion

        #region Contraseña Temporal

        public IActionResult OlvideContraseña()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OlvideContraseña(string correoElectronico)
        {
            // Verificar si el correo electrónico existe en la base de datos
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.CorreoElectronico == correoElectronico);

            if (usuario != null)
            {
                // Generar una contraseña temporal o un token
                var contraseñaTemporal = GenerarContraseñaTemporal(); // Implementa tu lógica para generar una contraseña temporal

                // Enviar el correo electrónico con la contraseña temporal o el token
                EnviarCorreoContraseñaTemporal(usuario.CorreoElectronico, contraseñaTemporal);

                // Encriptar y actualizar la contraseña del usuario en la base de datos
                usuario.Contraseña = EncriptarContraseña(contraseñaTemporal);
                await _context.SaveChangesAsync();

                // Redirigir a una vista de confirmación de restablecimiento de contraseña
                return RedirectToAction("ContraseñaTemporalEnviada");
            }

            // Si el correo electrónico no existe, redirigir a una vista de error o mostrar un mensaje de error en la vista
            ViewBag.Error = "El correo electrónico no está registrado en nuestro sistema.";
            return View();
        }
        private string GenerarContraseñaTemporal()
        {
            // Define los caracteres permitidos para la contraseña temporal
            string caracteresPermitidos = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";

            // Define la longitud deseada para la contraseña temporal
            int longitudContraseña = 10;

            // Genera una contraseña temporal aleatoria utilizando los caracteres permitidos
            Random random = new Random();
            StringBuilder contraseñaTemporal = new StringBuilder();

            for (int i = 0; i < longitudContraseña; i++)
            {
                int indiceCaracter = random.Next(caracteresPermitidos.Length);
                contraseñaTemporal.Append(caracteresPermitidos[indiceCaracter]);
            }

            return contraseñaTemporal.ToString();
        }

        private void EnviarCorreoContraseñaTemporal(string correoElectronico, string contraseñaTemporal)
        {
            // Configura la información del servidor SMTP
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUsername = "bancoauml@gmail.com";
            var smtpPassword = "qqudfzgjnhjhpyde";

            // Configura el mensaje de correo electrónico
            var remitente = "bancoauml@gmail.com";
            var destinatario = correoElectronico;
            var asunto = "Restablecimiento de contraseña";
            var mensaje = $"Tu nueva contraseña temporal es: {contraseñaTemporal}";

            // Envía el correo electrónico utilizando la librería SmtpClient de .NET
            using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

                using (var mailMessage = new MailMessage(remitente, destinatario, asunto, mensaje))
                {
                    smtpClient.Send(mailMessage);
                }
            }
        }

        public IActionResult ContraseñaTemporalEnviada()
        {
            return View();
        }
        #endregion

    }
}