using Microsoft.AspNetCore.Http;
using System.Globalization;
using System;

namespace BANCO.Models.Extension_InicioSesion
{
    // Clase de extensiones para la variable de sesión
    public static class SessionExtensions
    {
        public static void SetBool(this ISession session, string key, bool value)
        {
            session.SetString(key, value.ToString());
        }

        public static bool? GetBool(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value != null ? bool.Parse(value) : (bool?)null;
        }

        public static void SetDateTime(this ISession session, string key, DateTime value)
        {
            session.SetString(key, value.ToString("o"));
        }

        public static DateTime? GetDateTime(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value != null ? DateTime.ParseExact(value, "o", CultureInfo.InvariantCulture) : (DateTime?)null;
        }
    }

}
