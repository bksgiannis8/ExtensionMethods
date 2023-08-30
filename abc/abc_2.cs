using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Clave.ExtensionMethods
{
    public static class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Converts the enumerable to a ReadOnlyCollection
        /// </summary>
        public static async Task<IReadOnlyCollection<T>> ToReadOnlyCollection<T>(this Task<IEnumerable<T>> sourceTask)
        {
            var source = await sourceTask;
            return source is IReadOnlyCollection<T> collection ? collection : source.ToList();
        }

        /// <summary>
        /// Converts the enumerable to a ReadOnlyList
        /// </summary>
        public static async Task<IReadOnlyList<T>> ToReadOnlyList<T>(this Task<IEnumerable<T>> sourceTask)
        {
            var source = await sourceTask;
            return source is IReadOnlyList<T> list ? list : source.ToList();
        }
        
        /// <summary>
        /// Returns an enumerable containing only the items of the source that match the predicate
        /// </summary>
        public static async Task<IEnumerable<T>> Where<T>(this Task<IEnumerable<T>> sourceTask, Func<T, bool> predicate)
        {
            var source = await sourceTask;
            return source.Where(predicate);
        }
        
        /// <summary>
        /// Returns an enumerable containing only the items of the source that don't match the predicate
        /// </summary>
        public static async Task<IEnumerable<T>> WhereNot<T>(this Task<IEnumerable<T>> sourceTask, Func<T, bool> predicate)
        {
            var source = await sourceTask;
            return source.WhereNot(predicate);
        }
        
        /// <summary>
        /// Projects each element of a sequence into a new form
        /// </summary>
        public static async Task<IEnumerable<TResult>> Select<TIn, TResult>(this Task<IEnumerable<TIn>> sourceTask, Func<TIn, TResult> selector)
        {
            var source = await sourceTask;
            return source.Select(selector);
        }
    }
}

﻿using System;
using System.Collections.Generic;

namespace Clave.ExtensionMethods
{
    public static class Compare<T>
    {
        public static GeneralPropertyComparer<TProp> Using<TProp>(Func<T, TProp> selector)
            => new GeneralPropertyComparer<TProp>(selector);

        public class GeneralPropertyComparer<TKey> : IEqualityComparer<T>
        {
            private readonly Func<T, TKey> _expr;

            public GeneralPropertyComparer(Func<T, TKey> expr) => _expr = expr;

            public bool Equals(T x, T y) => EqualityComparer<TKey>.Default.Equals(_expr(x), _expr(y));

            public int GetHashCode(T obj) => EqualityComparer<TKey>.Default.GetHashCode(_expr(obj));
        }
    }
}


﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Clave.ExtensionMethods
{
    public static class Empty
    {
        /**
         * Get an empty ReadOnlyCollection. The same instance is returned each time, per type T
         */
        public static IReadOnlyCollection<T> ReadOnlyCollection<T>() => Singleton<T>.Instance;

        /**
         * Get an empty ReadOnlyList. The same instance is returned each time, per type T
         */
        public static IReadOnlyList<T> ReadOnlyList<T>() => Singleton<T>.Instance;

        /**
         * Get an empty array. The same instance is returned each time, per type T
         */
        public static T[] Array<T>() => Singleton<T>.Instance;

        /**
         * Get an empty ReadOnlyDictionary. The same instance is returned each time, per type TKey and TValue
         */
        public static IReadOnlyDictionary<TKey, TValue> ReadOnlyDictionary<TKey, TValue>() => Singleton<TKey, TValue>.Instance;

        /**
         * Get an empty string
         */
        public static string String => string.Empty;

        /**
         * Get an empty enumerable
         */
        public static IEnumerable<T> Enumerable<T>() => System.Linq.Enumerable.Empty<T>();

        private static class Singleton<T>
        {
            public static T[] Instance { get; } = new T[0];
        }
        private static class Singleton<TKey, TValue>
        {
            public static IReadOnlyDictionary<TKey, TValue> Instance { get; } = new Dictionary<TKey, TValue>();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Clave.ExtensionMethods
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns an enumerable containing only a single item
        /// </summary>
        public static IEnumerable<T> Only<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Returns an enumerable of the initial item and other items
        /// </summary>
        public static IEnumerable<T> And<T>(this T initial, T item)
        {
            yield return initial;
            yield return item;
        }

        /// <summary>
        /// Returns an enumerable of the initial item and other items
        /// </summary>
        public static IEnumerable<T> And<T>(this T initial, params T[] items)
        {
            yield return initial;
            foreach (var item in items)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Returns an enumerable of the initial item and other items
        /// </summary>
        public static IEnumerable<T> And<T>(this T initial, IEnumerable<T> items)
        {
            yield return initial;
            foreach (var item in items)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Returns true if the enumerable is empty
        /// </summary>
        public static bool NotAny<T>(this IEnumerable<T> source)
            => !source.Any();

        /// <summary>
        /// Returns true if the predicate does not return true for any of the items in the enumerable
        /// </summary>
        public static bool NotAny<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => !source.Any(predicate);

        /// <summary>
        /// Returns an enumerable containing only the items of the source that don't match the predicate
        /// </summary>
        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> source, Func<T, bool> predicate)
            => source.Where(x => !predicate(x));

        /// <summary>
        /// Returns an enumerable containing only non-null items
        /// </summary>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
            where T : class
            => source.WhereNot(x => x is null);

        /// <summary>
        /// Returns an enumerable containing only non-null items
        /// </summary>
        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source)
            where T : struct
            => source.WhereNot(x => x is null) as IEnumerable<T>;

        /// <summary>
        /// Returns an enumerable containing only items where the map function returns a non-null value
        /// </summary>
        public static IEnumerable<T> WhereNotNull<T, TKey>(this IEnumerable<T> source, Func<T, TKey> map)
            where TKey : class
            => source.WhereNot(x => map(x) is null);

        /// <summary>
        /// Returns an enumerable containing only items where the map function returns a non-null value
        /// </summary>
        public static IEnumerable<T> WhereNotNull<T, TKey>(this IEnumerable<T> source, Func<T, TKey?> map)
            where TKey : struct
            => source.WhereNot(x => map(x) is null);

        /// <summary>
        /// Converts the enumerable to a ReadOnlyCollection
        /// </summary>
        public static IReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
            => source is IReadOnlyCollection<T> collection ? collection : source.ToList();

        /// <summary>
        /// Converts the enumerable to a ReadOnlyList
        /// </summary>
        public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
            => source is IReadOnlyList<T> list ? list : source.ToList();

#if !NET6_0
        /// <summary>
        /// Zips together two lists returning a tuple of their values
        /// </summary>
        public static IEnumerable<(T1, T2)> Zip<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right)
            => left.Zip(right, ValueTuple.Create);
#endif

        /// <summary>
        /// Joins together two lists returning a tuple of their values using the key selectors
        /// </summary>
        public static IEnumerable<(T1, T2)> Join<T1, T2, TKey>(this IEnumerable<T1> left, IEnumerable<T2> right, Func<T1, TKey> leftKey, Func<T2, TKey> rightKey)
            => left.Join(right, leftKey, rightKey, ValueTuple.Create);

#if !NET6_0
        /// <summary>
        /// Returns only the items that have distinct values returned by the keySelector
        /// </summary>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
            => source.Distinct(Compare<T>.Using(selector));
#endif
        /// <summary>
        /// Groups by a property in the key
        /// </summary>
        public static IEnumerable<IGrouping<TKey, T>> GroupByProp<T, TKey, TProp>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<TKey, TProp> propSelector)
            => source.GroupBy(keySelector, Compare<TKey>.Using(propSelector));

        /// <summary>
        /// Returns only the items that are not in the second set, using the selector
        /// </summary>
        public static IEnumerable<T> ExceptBy<T, TKey>(this IEnumerable<T> source, IEnumerable<T> second, Func<T, TKey> selector)
            => source.Except(second, Compare<T>.Using(selector));

        /// <summary>
        /// Projects each tuple pair of a sequence into a new form
        /// </summary>
        public static IEnumerable<TResult> Select<TA, TB, TResult>(this IEnumerable<(TA A, TB B)> source, Func<TA, TB, TResult> selector)
            => source.Select(x => selector(x.A, x.B));

        /// <summary>
        /// Projects each tuple triplet of a sequence into a new form
        /// </summary>
        public static IEnumerable<TResult> Select<TA, TB, TC, TResult>(this IEnumerable<(TA A, TB B, TC C)> source, Func<TA, TB, TC, TResult> selector)
            => source.Select(x => selector(x.A, x.B, x.C));

        /// <summary>
        /// Add an item to a list
        /// </summary>
        public static void AddTo<T>(this T item, ICollection<T> list)
            => list.Add(item);

        /// <summary>
        /// Perform an action for each item in the sequence without modifying it
        /// </summary>
        public static IEnumerable<T> Tap<T>(this IEnumerable<T> source, Action<T> action)
            => source.Select(x => {
                action(x);
                return x;
            });

        /// <summary>
        /// Perform an action for each item in the sequence
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}


﻿using System;

namespace Clave.ExtensionMethods
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts a string to an enum
        /// </summary>
        /// <exception cref="ArgumentException">Throws if the string can't be converted to the enum</exception>
        public static T ToEnum<T>(this string value)
            where T : struct => (T)Enum.Parse(typeof(T), value);

        /// <summary>
        /// Converts a string to an enum, and uses the fallback value if the string doesn't match any enum values
        /// </summary>
        public static T ToEnumOrDefault<T>(this string value, T fallback)
            where T : struct => Enum.TryParse<T>(value, out var result) ? result : fallback;
    }
}

﻿using System;

namespace Clave.ExtensionMethods
{
    public static class FuncExtensions
    {
        /// <summary>
        /// Call the func with the argument, useful if the argument might be null
        /// </summary>
        /// <example>
        ///   value?.Pipe(SomeFunction)
        /// </example>
        public static TResult Pipe<TArg, TResult>(this TArg arg, Func<TArg, TResult> func) => func(arg);

        /// <summary>
        /// Call the func with the two arguments, useful if the first argument might be null
        /// </summary>
        /// <example>
        ///   value?.Pipe(SomeFunction, secondArgument)
        /// </example>
        public static TResult Pipe<TArg1, TArg2, TResult>(this TArg1 arg1, Func<TArg1, TArg2, TResult> func, TArg2 arg2) => func(arg1, arg2);

        /// <summary>
        /// Call the func with the three arguments, useful if the first argument might be null
        /// </summary>
        /// <example>
        ///   value?.Pipe(SomeFunction, secondArgument, thirdArgument)
        /// </example>
        public static TResult Pipe<TArg1, TArg2, TArg3, TResult>(this TArg1 arg1, Func<TArg1, TArg2, TArg3, TResult> func, TArg2 arg2, TArg3 arg3) => func(arg1, arg2, arg3);
    }
}

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

﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Clave.ExtensionMethods
{
    public static class StringExtensions
    {
        /// <summary>
        /// Joins a list of strings with a separator
        /// </summary>
        public static string Join<T>(this IEnumerable<T> values, string separator)
            => string.Join(separator, values);

        /// <summary>
        /// Joins a list of strings, except null or empty values, with a single space
        /// </summary>
        private static string JoinWithTwoSpaces<T>(this IEnumerable<T> values)
            => values
                .Select(v => v.ToString())
                .WhereNot(string.IsNullOrWhiteSpace)
                .Select(s => s.Trim())
                .Join("  ");

        public static string JoinWithSpace<T>(this IEnumerable<T> values)
            => values
                .Select(v => v.ToString())
                .WhereNot(string.IsNullOrWhiteSpace)
                .Select(s => s.Trim())
                .Join(" ");

        /// <summary>
        /// Joins a list of strings, except null or empty values, with ", "
        /// </summary>
        public static string JoinWithComma<T>(this IEnumerable<T> values)
            => values
                .Select(v => v.ToString())
                .WhereNot(string.IsNullOrWhiteSpace)
                .Select(s => s.Trim())
                .Join(", ");

        /// <summary>
        /// Joins a list of strings, except null or empty values, with a single space
        /// </summary>
        private static string ConcatWithTwoSapces(this string initial, params string[] values)
            {
                return initial
                .And(values)
                .JoinWithSpace()
                .JoinWithTwoSpaces();
            }
        public static string ConcatWithSpace(this string initial, params string[] values)
            => initial
                .And(values)
                .JoinWithSpace();

        /// <summary>
        /// Joins a list of strings, except null or empty values, with ", "
        /// </summary>
        public static string ConcatWithComma(this string initial, params string[] values)
            => initial
                .And(values)
                .JoinWithComma()
                .ConcatWithSpace();

        /// <summary>
        /// Returns null if the string is an empty string
        /// </summary>
        public static string ToNullIfEmpty(this string value)
            => string.IsNullOrEmpty(value) ? null : value;

        /// <summary>
        /// Returns null if the string is empty or white space
        /// </summary>
        public static string ToNullIfWhiteSpace(this string value)
            => string.IsNullOrWhiteSpace(value) ? null : value;

        /// <summary>
        /// Returns the substring after the prefix, if the string starts with the prefix
        /// </summary>
        public static string SkipPrefix(this string value, string prefix)
            => value.StartsWith(prefix) ? value.Substring(prefix.Length) : value;

        /// <summary>
        /// Converts a string to an int, and returns the fallback value if the conversion fails
        /// </summary>
        public static int ToInt(this string value, int fallback = 0)
            => int.TryParse(value, out var result) ? result : fallback;

        /// <summary>
        /// Converts a string to a decimal, and returns the fallback value if the conversion fails.
        /// Uses the invariant culture for parsing
        /// </summary>
        public static decimal ToDecimal(this string value, decimal fallback = 0)
            => decimal.TryParse(value, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out var result) ? result : fallback;
    }
}


﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Clave.ExtensionMethods.Magic
{
    public static class AsyncMagic
    {
        public static TaskAwaiter<T[]> GetAwaiter<T>(this IEnumerable<Task<T>> manyTasks)
            => Task.WhenAll(manyTasks).GetAwaiter();
    }
}

﻿using System.Collections;
using System.Collections.Generic;

namespace Clave.ExtensionMethods.Magic
{
    public static class CollectionMagic
    {
        public static void Add<T>(this ICollection<T> list, IEnumerable<T> items)
        {
            if(items == null)
                return;

            foreach(var item in items)
            {
                list.Add(item);
            }
        }
        
        public static void Add(this IList list, IEnumerable items)
        {
            if(items == null)
                return;

            foreach(var item in items)
            {
                list.Add(item);
            }
        }

        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if(items == null)
                return;

            foreach (var item in items)
            {
                dictionary[item.Key] = item.Value;
            }
        }
    }
}


// <autogenerated />
using System;
using System.Reflection;
[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName = ".NET Framework 4.6")]


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;

[assembly: System.Reflection.AssemblyCompanyAttribute("Clave Consulting")]
[assembly: System.Reflection.AssemblyConfigurationAttribute("Debug")]
[assembly: System.Reflection.AssemblyDescriptionAttribute("A collection of useful extension methods")]
[assembly: System.Reflection.AssemblyFileVersionAttribute("0.0.1.0")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute("0.0.1")]
[assembly: System.Reflection.AssemblyProductAttribute("Clave.ExtensionMethods")]
[assembly: System.Reflection.AssemblyTitleAttribute("Clave.ExtensionMethods")]
[assembly: System.Reflection.AssemblyVersionAttribute("0.0.1.0")]
[assembly: System.Reflection.AssemblyMetadataAttribute("RepositoryUrl", "https://github.com/ClaveConsulting/ExtensionMethods")]

// Generated by the MSBuild WriteCodeFragment class.



// <autogenerated />
using System;
using System.Reflection;
[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(".NETCoreApp,Version=v6.0", FrameworkDisplayName = ".NET 6.0")]


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;

[assembly: System.Reflection.AssemblyCompanyAttribute("Clave Consulting")]
[assembly: System.Reflection.AssemblyConfigurationAttribute("Debug")]
[assembly: System.Reflection.AssemblyDescriptionAttribute("A collection of useful extension methods")]
[assembly: System.Reflection.AssemblyFileVersionAttribute("0.0.1.0")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute("0.0.1")]
[assembly: System.Reflection.AssemblyProductAttribute("Clave.ExtensionMethods")]
[assembly: System.Reflection.AssemblyTitleAttribute("Clave.ExtensionMethods")]
[assembly: System.Reflection.AssemblyVersionAttribute("0.0.1.0")]
[assembly: System.Reflection.AssemblyMetadataAttribute("RepositoryUrl", "https://github.com/ClaveConsulting/ExtensionMethods")]

// Generated by the MSBuild WriteCodeFragment class.



// <autogenerated />
using System;
using System.Reflection;
[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(".NETStandard,Version=v2.0", FrameworkDisplayName = ".NET Standard 2.0")]


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;

[assembly: System.Reflection.AssemblyCompanyAttribute("Clave Consulting")]
[assembly: System.Reflection.AssemblyConfigurationAttribute("Debug")]
[assembly: System.Reflection.AssemblyDescriptionAttribute("A collection of useful extension methods")]
[assembly: System.Reflection.AssemblyFileVersionAttribute("0.0.1.0")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute("0.0.1")]
[assembly: System.Reflection.AssemblyProductAttribute("Clave.ExtensionMethods")]
[assembly: System.Reflection.AssemblyTitleAttribute("Clave.ExtensionMethods")]
[assembly: System.Reflection.AssemblyVersionAttribute("0.0.1.0")]
[assembly: System.Reflection.AssemblyMetadataAttribute("RepositoryUrl", "https://github.com/ClaveConsulting/ExtensionMethods")]

// Generated by the MSBuild WriteCodeFragment class.



﻿using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;

namespace Clave.ExtensionMethods.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void TestOnly()
        {
            "something".Only().ShouldContain("something");
        }

        [Test]
        public void TestAnd()
        {
            var list = "some".And("thing");

            list.ShouldContain("some");
            list.ShouldContain("thing");
        }

        [Test]
        public void TestAndMany()
        {
            var list = "some".And("thing", "else");

            list.ShouldContain("some");
            list.ShouldContain("thing");
            list.ShouldContain("else");
        }

        [Test]
        public void TestNotAny()
        {
            Empty.Array<string>().NotAny().ShouldBeTrue();
            "something".Only().NotAny().ShouldBeFalse();
        }

        [Test]
        public void TestNotAnyPredicate()
        {
            Empty.Array<string>().NotAny(string.IsNullOrEmpty).ShouldBeTrue();
            "something".Only().NotAny(string.IsNullOrEmpty).ShouldBeTrue();
            "something".And("").NotAny(string.IsNullOrEmpty).ShouldBeFalse();
        }

        [Test]
        public void TestWhereNot()
        {
            var result = 1.And(2, 3, 4).WhereNot(n => n % 2 == 0).ToReadOnlyCollection();

            result.Count().ShouldBe(2);
            result.ShouldContain(1);
            result.ShouldContain(3);
        }

        [Test]
        public void TestWhereNotNull()
        {
            var result = "a".And(null, "b", null).WhereNotNull().ToReadOnlyList();

            result.Count().ShouldBe(2);
            result.ShouldContain("a");
            result.ShouldContain("b");
        }

        [Test]
        public void TestWhereNotNullKeySelector()
        {
            var result = new Foo("1").And(new Foo(null), new Foo("b"), new Foo(null)).WhereNotNull(s => s.Prop);

            result.Count().ShouldBe(2);
            result.ShouldContain(s => s.Prop == "1");
            result.ShouldContain(s => s.Prop == "b");
        }

        [Test]
        public void TestDistinctBy()
        {
            var result = new Foo("1").And(new Foo("1"), new Foo("b"), new Foo("b")).DistinctBy(s => s.Prop);
            result.Count().ShouldBe(2);
            result.ShouldContain(s => s.Prop == "1");
            result.ShouldContain(s => s.Prop == "b");
        }

        [Test]
        public void TestGroupByProp()
        {
            var result = new Foo("12").And(new Foo("13"), new Foo("22"), new Foo("21")).GroupByProp(s => s.Prop, s => s[0]);

            result.Count().ShouldBe(2);
            result.First().Key.ShouldBe("12");
            result.Last().Key.ShouldBe("22");
        }

        [Test]
        public void TestExceptBy()
        {
            var second = new Foo("13").Only();
            var result = new Foo("12").And(new Foo("13"), new Foo("22"), new Foo("21")).ExceptBy(second, s => s.Prop);

            result.Count().ShouldBe(3);
            result.First().Prop.ShouldBe("12");
            result.Last().Prop.ShouldBe("21");
        }

        [Test]
        public void TestSelectTuple()
        {
            var result = ("A", 1).And(("B", 2)).Select(Map).Join(",");

            result.ShouldBe("A1,B2");
        }

        [Test]
        public void TestCompare()
        {
            var yearComparer = Compare<DateTime>.Using(date => date.Year);

            yearComparer.Equals(DateTime.Parse("2019-02-03"), DateTime.Parse("2019-08-19")).ShouldBeTrue();

            var dictionary = new Dictionary<DateTime, string>(yearComparer)
            {
                [DateTime.Parse("2019-02-06")] = "year 1",
                [DateTime.Parse("2020-08-12")] = "year 2",
            };

            dictionary[DateTime.Parse("2019-06-13")].ShouldBe("year 1");
        }

        private string Map(string a, int b) => a + b;

        public class Foo
        {
            public string Prop { get; }

            public Foo(string prop)
            {
                Prop = prop;
            }
        }
    }
}

﻿using NUnit.Framework;

namespace Clave.ExtensionMethods.Tests
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [TestCase("One", ExpectedResult = Foo.One)]
        [TestCase("Two", ExpectedResult = Foo.Two)]
        [TestCase("Three", ExpectedResult = Foo.Three)]
        public Foo TestToEnum(string key)
            => key.ToEnum<Foo>();

        [TestCase("One", ExpectedResult = Foo.One)]
        [TestCase("Two", ExpectedResult = Foo.Two)]
        [TestCase("Three", ExpectedResult = Foo.Three)]
        [TestCase("blabla", ExpectedResult = Foo.Unknown)]
        public Foo TestToEnumOrDefault(string key)
            => key.ToEnumOrDefault(Foo.Unknown);

        public enum Foo
        {
            Unknown,
            One,
            Two,
            Three
        }
    }
}

﻿using NUnit.Framework;
using Shouldly;

namespace Clave.ExtensionMethods.Tests
{
    [TestFixture]
    public class FuncExtensionsTests
    {
        private static readonly string MightBeNull = null;

        [Test]
        public void TestPipe()
        {
            var result = "something".Pipe(Func);

            result.ShouldBe("result");
        }

        [Test]
        public void TestPipeWithNull()
        {
            var result = MightBeNull?.Pipe(Func);

            result.ShouldBeNull();
        }

        [Test]
        public void TestPipe2()
        {
            var result = "something".Pipe(Func, "arg2");

            result.ShouldBe("result2");
        }

        [Test]
        public void TestPipeLambda()
        {
            var result = "something".Pipe((s, s1) => $"{s} {s1}", "arg2");

            result.ShouldBe("something arg2");
        }

        public static string Func(string arg)
        {
            return "result";
        }

        public static string Func(string arg, string arg2)
        {
            return "result2";
        }
    }
}

﻿using NUnit.Framework;
using Shouldly;

namespace Clave.ExtensionMethods.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void TestJoin()
        {
            new[] { "a", "b", "c" }.Join(" ").ShouldBe("a b c");
        }

        [Test]
        public void TestJoinInts()
        {
            new[] { 1, 2, 3 }.Join(" ").ShouldBe("1 2 3");
        }

        [Test]
        public void TestConcatWithSpace()
        {
            "a".ConcatWithSpace("b", "c").ShouldBe("a b c");
        }

        [Test]
        public void TestConcatWithSpaceWithSpacyStrings()
        {
            "a ".ConcatWithSpace("  b  ", "  ", " c").ShouldBe("a b c");
        }

        [Test]
        public void TestCommaSeparate()
        {
            "a".ConcatWithComma("b", "c").ShouldBe("a, b, c");
        }

        [Test]
        public void TestCommaSeparateWithSpacyStrings()
        {
            "a ".ConcatWithComma("  b  ", "  ", " c").ShouldBe("a, b, c");
        }

        [TestCase(null, ExpectedResult = null)]
        [TestCase("", ExpectedResult = null)]
        [TestCase("a", ExpectedResult = "a")]
        public string TestToNullIfEmpty(string input)
            => input.ToNullIfEmpty();

        [TestCase(null, ExpectedResult = null)]
        [TestCase("", ExpectedResult = null)]
        [TestCase(" ", ExpectedResult = null)]
        [TestCase("\n", ExpectedResult = null)]
        [TestCase("\t", ExpectedResult = null)]
        [TestCase("a", ExpectedResult = "a")]
        public string TestToNullIfWhiteSpace(string input)
            => input.ToNullIfWhiteSpace();

        [TestCase("something", "some", ExpectedResult = "thing")]
        [TestCase("something", "any", ExpectedResult = "something")]
        public string TestSkipPrefix(string value, string prefix)
            => value.SkipPrefix(prefix);

        [TestCase("123", ExpectedResult = 123)]
        [TestCase("blabla", ExpectedResult = 0)]
        public int TestToInt(string value)
            => value.ToInt();

        [TestCase("123", 75, ExpectedResult = 123)]
        [TestCase("blabla", 75, ExpectedResult = 75)]
        public int TestToIntWithFallback(string value, int fallback)
            => value.ToInt(fallback);

        [TestCase("123", ExpectedResult = 123)]
        [TestCase("blabla", ExpectedResult = 0)]
        public decimal TestToDecimal(string value)
            => value.ToDecimal();

        [TestCase("123", 75, ExpectedResult = 123)]
        [TestCase("blabla", 75, ExpectedResult = 75)]
        public decimal TestToDecimalWithFallback(string value, decimal fallback)
            => value.ToDecimal(fallback);
    }
}

// <autogenerated />
using System;
using System.Reflection;
[assembly: global::System.Runtime.Versioning.TargetFrameworkAttribute(".NETCoreApp,Version=v6.0", FrameworkDisplayName = ".NET 6.0")]


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Reflection;

[assembly: System.Reflection.AssemblyCompanyAttribute("Clave.ExtensionMethods.Tests")]
[assembly: System.Reflection.AssemblyConfigurationAttribute("Debug")]
[assembly: System.Reflection.AssemblyFileVersionAttribute("1.0.0.0")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute("1.0.0")]
[assembly: System.Reflection.AssemblyProductAttribute("Clave.ExtensionMethods.Tests")]
[assembly: System.Reflection.AssemblyTitleAttribute("Clave.ExtensionMethods.Tests")]
[assembly: System.Reflection.AssemblyVersionAttribute("1.0.0.0")]

// Generated by the MSBuild WriteCodeFragment class.



��/ /   < a u t o - g e n e r a t e d >   T h i s   f i l e   h a s   b e e n   a u t o   g e n e r a t e d .   < / a u t o - g e n e r a t e d >  
 u s i n g   S y s t e m ;  
 [ M i c r o s o f t . V i s u a l S t u d i o . T e s t P l a t f o r m . T e s t S D K A u t o G e n e r a t e d C o d e ]  
 c l a s s   A u t o G e n e r a t e d P r o g r a m   { s t a t i c   v o i d   M a i n ( s t r i n g [ ]   a r g s ) { } }  
 