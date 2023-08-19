using System.Security.Claims;

namespace ScriptRunner.Helpers
{
    public static class ExtensionMethods
    {
        public static bool IsInAllowedRoles(this ClaimsPrincipal claimsPrincipal, IEnumerable<string> allowedRoles)
        {
            if (claimsPrincipal.Identity == null)
                return false;

            foreach (string role in allowedRoles)
            {
                if (((ClaimsIdentity)claimsPrincipal.Identity).Claims.Any(x => x.Type == "permissions" && x.Value == role))
                    return true;
            }

            return false;
        }
    }
}
