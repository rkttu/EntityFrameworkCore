using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;

namespace ClouDeveloper.EntityFrameworkCore.Contrib
{
    /// <summary>
    /// A factory class that helps create a DbContext from a JSON configuration file. This class can not be used directly and must be inherited and redefined.
    /// </summary>
    /// <remarks>
    /// Original code comes from https://www.benday.com/2017/02/17/ef-core-migrations-without-hard-coding-a-connection-string-using-idbcontextfactory/
    /// Thanks to Benjamin Day. (info@benday.com)
    /// </remarks>
    /// <typeparam name="TDbContext">Specifies the data context class that you want to use.</typeparam>
    public abstract class SmartDataContextFactory<TDbContext> : IDbContextFactory<TDbContext>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Default hosting environment variable name to determine current runtime environment.
        /// </summary>
        protected virtual string HostingEnvironmentVariableName
            => "Hosting:Environment";

        /// <summary>
        /// Default configuration json file name to use. (Environment agnostic)
        /// </summary>
        protected virtual string AppSettingsJsonFileName
            => "appsettings.json";

        /// <summary>
        /// Default configuraiton json file name to use.
        /// </summary>
        /// <param name="environmentName">The environment name of current runtime.</param>
        /// <returns>Inferred file name.</returns>
        protected virtual string GetAppSettingsJsonFileName(string environmentName)
            => $"appsettings.{environmentName}.json";

        /// <summary>
        /// Default connection string entry name to use.
        /// </summary>
        protected virtual string DefaultConnectionStringEntryName
            => "(default)";

        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <returns>An instance of TContext.</returns>
        public TDbContext Create()
        {
            var environmentName = Environment.GetEnvironmentVariable(HostingEnvironmentVariableName);
            var basePath = AppContext.BaseDirectory;
            return Create(basePath, environmentName);
        }

        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <param name="options">Information about the environment an application is running in.</param>
        /// <returns>An instance of TContext.</returns>
        public TDbContext Create(DbContextFactoryOptions options)
        {
            return Create(options.ContentRootPath, options.EnvironmentName);
        }

        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <param name="basePath">The base path of configuration file.</param>
        /// <param name="environmentName">The environment name of current runtime.</param>
        /// <returns>An instance of TContext.</returns>
        protected TDbContext Create(string basePath, string environmentName)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(AppSettingsJsonFileName)
                .AddJsonFile(GetAppSettingsJsonFileName(environmentName), true)
                .AddEnvironmentVariables();

            var config = builder.Build();
            var connectionString = config.GetConnectionString(DefaultConnectionStringEntryName);

            if (String.IsNullOrWhiteSpace(connectionString) == true)
                throw new InvalidOperationException("Could not find a connection string named '(default)'.");
            
            return Create(connectionString);
        }

        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>An instance of TContext.</returns>
        protected abstract TDbContext Create(string connectionString);
    }
}
