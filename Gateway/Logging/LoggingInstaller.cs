﻿using Serilog;

namespace Gateway.Logging
{
    public static class LoggingInstaller
    {
        public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration) 
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            return services;
        }
    }
}
