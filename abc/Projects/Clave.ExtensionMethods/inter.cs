using FluentValidation;
using Iwcp.Infrastructure.Constants;
using Iwcp.Infrastructure.Impl.DIExtensions;
using Iwcp.Infrastructure.Impl.Encryption;
using Iwcp.Infrastructure.Impl.Services.Users;
using Iwcp.Infrastructure.Services.Identity;
using Iwcp.Infrastructure.Web.Extensions;
using Iwcp.Rest.Application.Exceptions;
using Iwcp.Rest.Application.Validation;
using Iwcp.Rest.Application.Validation.Base;
using Iwcp.Rest.Application.Validation.Services;
using Iwcp.Rest.Domain.Exceptions;
using Iwcp.Rest.Infrastructure.Context;
using JsonApiDotNetCore.Configuration;
using JsonApiDotNetCore.Resources.Annotations;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Iwcp.Rest.Infrastructure.DIExtensions
{
    public static class ServiceCollectionExtensions
    {
        private static Assembly DomainAssembly => typeof(JsonApiBaseException).Assembly;
        private static Assembly ApplicationAssembly => typeof(ResourceNotFoundException).Assembly;

        /// <summary>
        /// Inserts all necessary implmentations for the service's execution
        /// </summary>
        /// <param name="services">DI Service Collection</param>
        /// <returns></returns>
        public static IServiceCollection AddApplicationInfrastructure(this IServiceCollection services, IConfiguration configuration, string restAssemblyName)
        {
            return services
                .AddEncryptionServices(configuration)
                .AddApplicationDbContext(configuration)
                .AddJsonApiSpecification(restAssemblyName)
                .AddInfrastructure()
                .AddRestAuthentication()
                .AddIdentityProviderServices()
                .AddEntityValidators()
                .AddMediatRHandlers();
        }

        /// <summary>
        /// Configures Authentication services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>A <see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddRestAuthentication(this IServiceCollection services)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddBearer(options => options.PlainSecret = Jwt.PlainSecret);

            return services;
        }

        /// <summary>
        /// Configures Identity provider services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>A <see cref="IServiceCollection"/>.</returns>
        private static IServiceCollection AddIdentityProviderServices(this IServiceCollection services)
        {
            return services
                .AddScoped<IIdentityProvider, IdentityProvider>()
                .AddScoped<IOrganizationIdentityProvider, OrganizationIdentityProvider>()
                .AddScoped<IUserIdentityProvider, WebUserIdentityProvider>();
        }

        private static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetApplicationConnectionString());
                });
        }

        private static IServiceCollection AddJsonApiSpecification(this IServiceCollection services, string restAssemblyName)
        {
            services.AddJsonApi<ApplicationDbContext>(
                options: o => o.AddJsonApiOptions(),
                discovery: d => d.AddJsonApiAssemblies(restAssemblyName));

            return services;
        }

        public static JsonApiOptions AddJsonApiOptions(this JsonApiOptions options)
        {
            options.AllowUnknownQueryStringParameters = true;
            options.DefaultPageSize = new PageSize(25);
            options.MaximumPageNumber = new PageNumber(200);
            options.MaximumPageSize = new PageSize(100);
            options.RelationshipLinks = LinkTypes.None;
            options.ResourceLinks = LinkTypes.None;
            options.TopLevelLinks = LinkTypes.None;
            options.MaximumIncludeDepth = 2;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

            return options;
        }

        public static ServiceDiscoveryFacade AddJsonApiAssemblies(this ServiceDiscoveryFacade discovery, string restAssemblyName)
        {
            return discovery.AddCurrentAssembly()
                .AddAssembly(Assembly.Load(restAssemblyName))
                .AddAssembly(DomainAssembly)
                .AddAssembly(ApplicationAssembly);
        }

        private static IServiceCollection AddEntityValidators(this IServiceCollection services)
        {
            return services
                .AddScoped(typeof(IValidationService<,>), typeof(ValidationService<,>))
                .AddValidatorsFromAssemblyContaining<IBaseValidator>();
        }

        private static IServiceCollection AddMediatRHandlers(this IServiceCollection services)
        {
            return services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(ApplicationAssembly);
                config.NotificationPublisherType = typeof(TaskWhenAllPublisher);
            });
        }


        private static IServiceCollection AddEncryptionServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetApplicationConnectionString();
            return services
                .AddSingleton(configuration.GetOptions<EncryptionOptions>("Encryption"))
                .AddSingleton<IAesEncryption, AesEncryption>()
                .AddSingleton<IQueriesFactory, QueriesFactory>()
                .AddScoped<IConnectionManager>(s => new ConnectionManager(connectionString, s.GetRequiredService<IQueriesFactory>()));
        }
    }
}