using Microsoft.AspNet.Identity.EntityFramework;
namespace ALTSecurity.Web.Models.Account
{
    public class ApplicationRole : IdentityRole<int, UserRole>
    {
        public ApplicationRole() { }
        
        public ApplicationRole(string name) { Name = name; }
    }
}