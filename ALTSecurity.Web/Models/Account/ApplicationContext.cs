using Microsoft.AspNet.Identity.EntityFramework;

namespace ALTSecurity.Web.Models.Account
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser,ApplicationRole, int, UserLogin, UserRole, UserClaim>
    {
        public ApplicationContext() : base("AltSecurityDb") { }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
    }
}