using System.Reflection;
using ApprovalFlow.Data;
using ApprovalFlow.Integrations;
using ApprovalFlow.Workflow;

namespace ApprovalFlow.Extensions
{
    public static class ServiceCollectionExtensions
    {


        public static void RegisterDependencies(this IServiceCollection services)
        {
             
  

            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract &&
                       (t.Name.EndsWith("Repository") || t.Name.EndsWith("Service")));

            foreach (var type in types)
            {
                var interfaceType = type.GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"I{type.Name}");

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, type);
                }
            }
        }
    }
}
