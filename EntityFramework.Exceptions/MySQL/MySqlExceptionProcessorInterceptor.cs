using EntityFramework.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

#if DEVART
using Devart.Data.MySql;
using DbExceptionClassifier.MySQL.Devart;

namespace EntityFramework.Exceptions.MySQL.Devart;
#else
using MySql.Data.MySqlClient;
using DbExceptionClassifier.MySQL;

namespace EntityFramework.Exceptions.MySQL;
#endif

class MySqlExceptionProcessorInterceptor() : ExceptionProcessorInterceptor<MySqlException>(new MySQLExceptionClassifier());

public static class ExceptionProcessorExtensions
{
    public static DbContextOptionsBuilder UseExceptionProcessor(this DbContextOptionsBuilder self)
        => self.AddInterceptors(new MySqlExceptionProcessorInterceptor());

    public static DbContextOptionsBuilder<TContext> UseExceptionProcessor<TContext>(this DbContextOptionsBuilder<TContext> self) where TContext : DbContext
        => self.AddInterceptors(new MySqlExceptionProcessorInterceptor());
}
