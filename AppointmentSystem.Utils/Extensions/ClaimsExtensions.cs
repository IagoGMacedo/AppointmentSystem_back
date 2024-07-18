using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentSystem.Utils.Extensions
{
    public static class ClaimsExtensions
    {
        public static IEnumerable<string> GetValuesOfType(this IEnumerable<Claim> claims, string type)
        {
            return claims.Where(x => x.Type == type)
                         .Select(x => x.Value)
                         .Distinct();
        }

        public static string? GetClaimValue(this IEnumerable<Claim> claims, string claimType)
        {
            string? claimValue = null;
            var claimValues = GetValuesOfType(claims, claimType);

            if (claimValues != null)
                claimValue = claimValues.FirstOrDefault();

            return claimValue;
        }
    }
}
