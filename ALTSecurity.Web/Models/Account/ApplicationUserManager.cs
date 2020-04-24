using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace ALTSecurity.Web.Models.Account
{
    public class UserStore : UserStore<ApplicationUser, ApplicationRole, int, UserLogin, UserRole, UserClaim>
    {
        public UserStore(ApplicationContext context) : base(context)
        {
        }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser, int>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser,int> store) : base(store) { }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            ApplicationContext db = context.Get<ApplicationContext>();
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore(db));
            
            return manager;
        }
    }
}