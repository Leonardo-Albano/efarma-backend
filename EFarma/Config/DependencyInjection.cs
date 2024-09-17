using EFarma.Business.Interfaces;

namespace EFarma.Config
{
    public class DependencyInjection
    {
        public static void AddBusiness(WebApplicationBuilder builder)
        {
            var assembly = typeof(Program).Assembly;
            var businessInterfaceType = typeof(IPatientBusiness).Namespace; // Change this to your business namespace

            foreach (var type in assembly.GetTypes())
            {
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (@interface.Namespace == businessInterfaceType && @interface.Name.StartsWith("I") && type.Name.EndsWith("Business"))
                    {
                        builder.Services.AddScoped(@interface, type);
                    }
                }
            }
        }
    }
}
