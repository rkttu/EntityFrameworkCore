# ClouDeveloper Entity Framework Core Contribution Package

This package is a utility package containing helper classes for Entity Framework Core 1.0. While using Entity Framework Core 1.0, I would like to add to the library the features that are uncomfortable or complementary to this library. I ask for your many ideas.

## SmartDataContextFactory&lt;TDbContext&gt; class

A factory class that automatically creates database contexts by reading the `(default)` connection string entries in `appsettings.json` and `appsettings.<Host:Environment>.json` files. It is compatible with the "dotnet ef" command and does not hard-code the connection string into the code.

After creating a new class that inherits from this class, then implement the `Create` method. Simply accept the connection string passed as an argument, link the actual database provider to the DbContext, create an instance, and return the instance. You can also redefine which files to read from and which criteria to choose based on your needs.

```C#
// This sample code requires Microsoft.EntityFramework.SqlServer package.

public class MyDbContext : DbContext
{
    public MyDbContext() : base() { }
    public MyDbContext(DbContextOptions options) : base(options) { }
}

public class MyDbContextFactory : SmartDataContextFactory<DbContext>
{
    protected override DbContext Create(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseSqlServer(connectionString);
        return new MyDbContext(optionsBuilder.Options);
    }
}
```

The code for this class comes from Bejamin Day's example code.
https://www.benday.com/2017/02/17/ef-core-migrations-without-hard-coding-a-connection-string-using-idbcontextfactory/

Thanks to Benjamin Day. (info@benday.com)
