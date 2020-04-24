using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ALTSecurity.Web.Models.Account
{
    public class UserRole: IdentityUserRole<int> { }

    public class UserClaim: IdentityUserClaim<int> { }

    public class UserLogin: IdentityUserLogin<int> { }



    public class ApplicationUser : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        public ApplicationUser() { }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser,int> manager, string authenticationType = DefaultAuthenticationTypes.ApplicationCookie)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}