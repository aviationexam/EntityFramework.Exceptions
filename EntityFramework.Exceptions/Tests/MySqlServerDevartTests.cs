using System;
using System.Threading.Tasks;
using Devart.Data.MySql;
using EntityFramework.Exceptions.MySQL.Devart;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MariaDb;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

[Collection("MySql Test Collection")]
public class MySqlServerDevartTests : DatabaseTests, IClassFixture<MySqlDevartDemoContextFixture>
{
    public MySqlServerDevartTests(MySqlDevartDemoContextFixture fixture) : base(SkipWithoutLicense(fixture).DemoContext)
    {
    }

    // Runs before the base constructor, which needs the DemoContext that the fixture creates only with a license.
    private static MySqlDevartDemoContextFixture SkipWithoutLicense(MySqlDevartDemoContextFixture fixture)
    {
        Assert.SkipWhen(MySqlDevartDemoContextFixture.LicenseKey is null, "Set DEVART_LICENSE to run the Devart tests.");

        return fixture;
    }
}

public class MySqlDevartDemoContextFixture : DemoContextFixture<MariaDbContainer>
{
    internal static readonly string LicenseKey = Environment.GetEnvironmentVariable("DEVART_LICENSE") is { Length: > 0 } licenseKey ? licenseKey : null;

    static MySqlDevartDemoContextFixture()
    {
        if (LicenseKey is not null)
        {
            Container = new MariaDbBuilder("mariadb:12.2").Build();
        }
    }

    // Without a license Devart cannot connect, so the tests skip and the container does not start.
    public override ValueTask InitializeAsync() => LicenseKey is null ? ValueTask.CompletedTask : base.InitializeAsync();

    private static string AddLicenceKeyToTheConnectionString(
        string connectionString) =>
        new MySqlConnectionStringBuilder(connectionString)
        {
            LicenseKey = LicenseKey
        }.ConnectionString;

    protected override DbContextOptionsBuilder<DemoContext> BuildDemoContextOptions(
        DbContextOptionsBuilder<DemoContext> builder, string connectionString)
    {
        builder.UseMySql(AddLicenceKeyToTheConnectionString(connectionString));

        return builder.UseExceptionProcessor();
    }

    protected override DbContextOptionsBuilder BuildSameNameIndexesContextOptions(
        DbContextOptionsBuilder builder, string connectionString)
        => builder.UseMySql(AddLicenceKeyToTheConnectionString(connectionString)).UseExceptionProcessor();
}
