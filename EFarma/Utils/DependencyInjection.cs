using EFarma.Business;
using EFarma.Business.Interfaces;

namespace EFarma.Config
{
    public class DependencyInjection
    {
        public static void AddBusiness(WebApplicationBuilder builder)
        {
            // Manual registration for problematic services
            builder.Services.AddScoped<IPersonBusiness, PersonBusiness>(); // For Person logic
            builder.Services.AddScoped<IEmployeeBusiness, EmployeeBusiness>(); // For Employee logic

            // Keep automatic registration for other business classes
            var assembly = typeof(Program).Assembly;
            var businessInterfaceType = typeof(IPatientBusiness).Namespace;

            foreach (var type in assembly.GetTypes())
            {
                var interfaces = type.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (@interface.Namespace == businessInterfaceType
                        && @interface.Name.StartsWith("I")
                        && type.Name.EndsWith("Business"))
                    {
                        // Avoid redundant registration
                        if (@interface == typeof(IPersonBusiness) || @interface == typeof(IEmployeeBusiness))
                            continue;

                        builder.Services.AddScoped(@interface, type);
                    }
                }
            }
        }

    }
}
