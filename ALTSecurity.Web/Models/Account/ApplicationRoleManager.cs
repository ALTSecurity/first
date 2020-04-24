using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace ALTSecurity.Web.Models.Account
{ 
    public class RoleStore : RoleStore<ApplicationRole, int, UserRole>
    {
        public RoleStore(ApplicationContext context) : base(context)
        {
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole,int>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole,int> store) : base(store) { }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore(context.Get<ApplicationContext>()));
        }
    }
}