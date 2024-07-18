using AppointmentSystem.Utils.UserContext;
using System.Collections;

namespace AppointmentSystem.Utils.Extensions
{
    public static class UserContextExtensions
    {
        public static string Login(this IUserContext usuarioContexto)
        {
            var login = usuarioContexto.GetClaimValue<string>("login");

            return login ?? Environment.MachineName;
        }

        public static void AddData<TValue>(this IUserContext usuarioContexto, string key, TValue data)
        {
            usuarioContexto.AdditionalData ??= new Hashtable();

            if (!usuarioContexto.AdditionalData.ContainsKey(key))
                usuarioContexto.AdditionalData.Add(key, data);
            else
                usuarioContexto.AdditionalData[key] = data;
        }

        private static TResult? GetClaimValue<TResult>(this IUserContext usuarioContexto, string key)
        {
            if (usuarioContexto?.AdditionalData is Hashtable additionalData && additionalData.ContainsKey(key))
                try { return (TResult)additionalData[key]; } catch { return default; }

            return default;
        }
    }
}
