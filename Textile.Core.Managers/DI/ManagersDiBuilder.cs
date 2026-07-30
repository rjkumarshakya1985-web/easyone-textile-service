using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Textile.Core.Managers.Behaviors;     // your ValidationBehavior namespace
using FluentValidation;

namespace Textile.Core.Managers.DI
{
    public static class ManagersDiBuilder
    {
        public static void AddCQRS(this IServiceCollection services)
        {
            // Register MediatR (MediatR v12+ syntax)
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(ManagersDiBuilder).Assembly);
            });

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(typeof(ManagersDiBuilder).Assembly);

            // Register Validation Behavior
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }
    }
}
