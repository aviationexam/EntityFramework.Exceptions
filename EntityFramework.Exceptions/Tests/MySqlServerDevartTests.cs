using System;
using Devart.Data.MySql;
using EntityFramework.Exceptions.MySQL.Devart;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MariaDb;
using Xunit;

namespace EntityFramework.Exceptions.Tests;

[Collection("MySql Test Collection")]
public class MySqlServerDevartTests : DatabaseTests, IClassFixture<MySqlDevartDemoContextFixture>
{
    public MySqlServerDevartTests(MySqlDevartDemoContextFixture fixture) : base(fixture.DemoContext)
    {
    }
}

public class MySqlDevartDemoContextFixture : DemoContextFixture<MariaDbContainer>
{
    static MySqlDevartDemoContextFixture()
    {
        Container = new MariaDbBuilder().WithImage("mariadb:12.2").Build();
    }

    private static string AddLicenceKeyToTheConnectionString(
        string connectionString) =>
        new MySqlConnectionStringBuilder(connectionString)
        {
            LicenseKey = Environment.GetEnvironmentVariable("DEVART_LICENSE")
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
