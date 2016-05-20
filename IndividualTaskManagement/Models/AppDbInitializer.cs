using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace IndividualTaskManagement.Models
{
    public class AppDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            
            var roleAdmin = new IdentityRole { Name = "admin" };
            var roleStudent = new IdentityRole { Name = "student" };
            var roleTeacher = new IdentityRole { Name = "teacher" };

           
            roleManager.Create(roleAdmin);
            roleManager.Create(roleStudent);
            roleManager.Create(roleTeacher);

           
            var admin = new ApplicationUser { Email = "admin@gmail.com", UserName = "Admin" };
            string password = "123456";
            var result = userManager.Create(admin, password);

            
            if (result.Succeeded)
            {                
                userManager.AddToRole(admin.Id, roleAdmin.Name);
                //userManager.AddToRole(admin.Id, roleStudent.Name);
                //userManager.AddToRole(admin.Id, roleTeacher.Name);
            }

            base.Seed(context);
        }
    }
}