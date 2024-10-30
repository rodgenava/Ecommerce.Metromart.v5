// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PriceInventoryUpdatesConsoleApp;
using Serilog.Events;
using Serilog;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Email;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Amazon.Runtime;
using Amazon.S3;
using Polly;
using Infrastructure.Data;
using Data.Common.Contracts;
using Data.Projections;
using Application;
using PriceInventoryUpdatesConsoleApp.Infrastructure.External.AmazonS3;

void ConfigureServicesDevelopment(HostBuilderContext context, IServiceCollection services)
{
    services.AddLogging(
        builder =>
        {
            IConfiguration configuration = context.Configuration;
            
            string mailProfile = configuration["Application:Notification:Crash:UseMailingProfile"];
            
            IConfiguration mailConfiguration = configuration.GetRequiredSection($"Application:Environment:Mailing:Profiles:{mailProfile}");
            
            builder.AddSerilog(
                logger: new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.File(
                    path: configuration["Application:Environment:Paths:ErrorLogs"],
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger(),
                dispose: true);
        });

    services.AddTransient<IAsyncPolicy>(
        s =>
        {
            var configuration = context.Configuration;

            return Policy.Handle<Exception>().WaitAndRetryAsync(
                retryCount: configuration.GetValue(key: "Infrastructure:AWS:HandlingPolicy:Retries", defaultValue: 1),
                sleepDurationProvider: retryCount => configuration.GetValue(
                    key: "Infrastructure:AWS:HandlingPolicy:PauseDuration", 
                    defaultValue: TimeSpan.FromMilliseconds(500)).Multiply(retryCount));
        });

    services.AddScoped(s =>
    {
        IConfiguration configuration = context.Configuration;

        return new AmazonS3Client(
                credentials: new BasicAWSCredentials(
                    accessKey: configuration["Infrastructure:AWS:AccessKey"],
                    secretKey: configuration["Infrastructure:AWS:SecretKey"]),
                region: Amazon.RegionEndpoint.APSoutheast1);
    });
    
    services.AddTransient(
        s => new MetromartPriceInventoryUpdateByWarehouseQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce"),
            commandTimeout: context.Configuration.GetValue<int>(
                key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient(
        s => new ConsignmentMetromartPriceInventoryUpdateByWarehouseQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce"),
            commandTimeout: context.Configuration.GetValue<int>(key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient<IAsyncQuery<IReadOnlyDictionary<Warehouse, MetromartStore>>>(
        s => new AllSnrWarehouseToMetromartStoresMappingQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce.Apps"),
            commandTimeout: context.Configuration.GetValue<int>(key: "Infrastructure:Data:Sql:CommandTimeout")));
    
    services.AddTransient<IMetromartUpdateFileWriter>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:MetromartPriceInventoryUpdates:Writer:Use"];

            return useValue switch
            {
                "v1" => new MetromartUpdateFileWriter(),
                "v2" => new MetromartUpdateFileWriter20221123(),
                "v3" => new MetromartUpdateFileWriter20221124(),
                "v4" => new MetromartUpdateFileWriter20221125(),
                "v5" => new MetromartUpdateFileWriter20221220(),
                "v6" => new MetromartUpdateFileWriter20230113(),
                _ => throw new Exception($"Invalid configuration value @ \"Application:MetromartPriceInventoryUpdates:Writer:Use\"; expecting values: v1, v2, v3, v4, v5 or v6; received \"{useValue}\".")
            };
        });

    services.AddTransient<IDispatchService>(
        s => new AmazonS3DispatchService(
            client: s.GetRequiredService<AmazonS3Client>(),
            bucketName: context.Configuration[""],
            asyncPolicy: s.GetRequiredService<IAsyncPolicy>()));

    services.AddTransient<IDispatchService>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:MetromartPriceInventoryUpdates:Disptach:Use"];

            return useValue switch
            {
                "Dump" => new DiskDispatchService(),
                "AWSS3" => new AmazonS3DispatchService(
                    client: s.GetRequiredService<AmazonS3Client>(),
                    bucketName: context.Configuration["Application:MetromartPriceInventoryUpdates:Disptach:AWSS3Method:BucketName"],
                    asyncPolicy: s.GetRequiredService<IAsyncPolicy>()),
                _ => throw new Exception($"Invalid configuration value @ \"Application:MetromartPriceInventoryUpdates:Disptach:Use\"; expecting values: Dump, or AWSS3; received \"{useValue}\".")
            };
        });


    services.AddTransient<IMetromartUpdatePathConventionFactory>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:Use"];

            return useValue switch
            {
                "CurrentDate" => new CurrentDateMetromartUpdatePathConventionFactory(format: configuration["Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:CurrentDatePathConvention:Format"]),
                "ConsolePrompt" => new ConsolePromptMetromartUpdatePathConventionFactory(prompt: configuration["Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:ConsolePromptPathConvention:Message"]),
                "StaticPrefixValue" => new StaticPrefixValueMetromartUpdatePathConventionFactory(prefix: configuration["Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:StaticPrefixValuePathConvention:Prefix"]),
                _ => throw new Exception($"Invalid configuration value @ \"Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:Use\"; expecting values: CurrentDate, ConsolePrompt, or StaticPrefixValue; received \"{useValue}\".")
            };
        });

    services.AddScoped<ISendUpdatesServiceFactory, SendUpdatesServiceFactory>();

    //------------------------------- Panda mart ------------------------------
    services.AddTransient(
        s => new PandamartPriceInventoryUpdateByWarehouseQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce"),
            commandTimeout: context.Configuration.GetValue<int>(
                key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient(
        s => new ConsignmentPandamartPriceInventoryUpdateByWarehouseQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce"),
            commandTimeout: context.Configuration.GetValue<int>(key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient<IAsyncQuery<IReadOnlyDictionary<Warehouse, PandamartStore>>>(
        s => new AllSnrWarehouseToPandamartStoresMappingQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce.Apps"),
            commandTimeout: context.Configuration.GetValue<int>(key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient<IPandamartUpdateFileWriter>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:PandamartPriceInventoryUpdates:Writer:Use"];

            return useValue switch
            {
                "v1" => new PandamartUpdateFileWriter(),
                _ => throw new Exception($"Invalid configuration value @ \"Application:PandamartPriceInventoryUpdates:Writer:Use\"; expecting values: v1; received \"{useValue}\".")
            };
        });

    services.AddTransient<IDispatchService>(
        s => new AmazonS3DispatchService(
            client: s.GetRequiredService<AmazonS3Client>(),
            bucketName: context.Configuration[""],
            asyncPolicy: s.GetRequiredService<IAsyncPolicy>()));

    services.AddTransient<IDispatchService>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:PandamartPriceInventoryUpdates:Disptach:Use"];

            return useValue switch
            {
                "Dump" => new DiskDispatchService(),
                "AWSS3" => new AmazonS3DispatchService(
                    client: s.GetRequiredService<AmazonS3Client>(),
                    bucketName: context.Configuration["Application:PandamartPriceInventoryUpdates:Disptach:AWSS3Method:BucketName"],
                    asyncPolicy: s.GetRequiredService<IAsyncPolicy>()),
                _ => throw new Exception($"Invalid configuration value @ \"Application:PandamartPriceInventoryUpdates:Disptach:Use\"; expecting values: Dump, or AWSS3; received \"{useValue}\".")
            };
        });


    services.AddTransient<IPandamartUpdatePathConventionFactory>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:Use"];

            return useValue switch
            {
                "CurrentDate" => new CurrentDatePandamartUpdatePathConventionFactory(format: configuration["Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:CurrentDatePathConvention:Format"]),
                "ConsolePrompt" => new ConsolePromptPandamartUpdatePathConventionFactory(prompt: configuration["Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:ConsolePromptPathConvention:Message"]),
                "StaticPrefixValue" => new StaticPrefixValuePandamartUpdatePathConventionFactory(prefix: configuration["Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:StaticPrefixValuePathConvention:Prefix"]),
                _ => throw new Exception($"Invalid configuration value @ \"Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:Use\"; expecting values: CurrentDate, ConsolePrompt, or StaticPrefixValue; received \"{useValue}\".")
            };
        });

    services.AddScoped<ISendPandaMartUpdatesServiceFactory, SendPandaMartUpdatesServiceFactory>();
    services.AddScoped<ISFTPPandamart, SFTPPandamart>();
    services.AddScoped<ILocalDataPath, LocalDataPath>();
}

void ConfigureServicesProduction(HostBuilderContext context, IServiceCollection services)
{
    services.AddLogging(
        builder =>
        {
            IConfiguration configuration = context.Configuration;

            string mailProfile = configuration["Application:Notification:Crash:UseMailingProfile"];

            IConfiguration mailConfiguration = configuration.GetRequiredSection($"Application:Environment:Mailing:Profiles:{mailProfile}");

            builder.AddSerilog(
                logger: new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console()
                .WriteTo.File(
                    path: configuration["Application:Environment:Paths:ErrorLogs"],
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day)
                .WriteTo.Email(new EmailConnectionInfo()
                {
                    FromEmail = mailConfiguration["Credentials:Email"],
                    EnableSsl = mailConfiguration.GetValue<bool>("EnableSSL"),
                    EmailSubject = configuration["Application:Notification:Crash:Title"],
                    IsBodyHtml = false,
                    MailServer = mailConfiguration["Host"],
                    NetworkCredentials = new NetworkCredential(
                        userName: mailConfiguration["Credentials:Email"],
                        password: mailConfiguration["Credentials:Password"]),
                    Port = mailConfiguration.GetValue<int>("Port"),
                    ToEmail = configuration["Application:Notification:Crash:Recipients"],
                    ServerCertificateValidationCallback = (object senderX, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors) => true
                })
                .CreateLogger(),
                dispose: true);
        });

    services.AddTransient<IAsyncPolicy>(
        s =>
        {
            var configuration = context.Configuration;

            return Policy.Handle<Exception>().WaitAndRetryAsync(
                retryCount: configuration.GetValue(key: "Infrastructure:AWS:HandlingPolicy:Retries", defaultValue: 1),
                sleepDurationProvider: retryCount => configuration.GetValue(
                    key: "Infrastructure:AWS:HandlingPolicy:PauseDuration",
                    defaultValue: TimeSpan.FromMilliseconds(500)).Multiply(retryCount));
        });

    services.AddScoped(s =>
    {
        IConfiguration configuration = context.Configuration;
        
        return new AmazonS3Client(
                credentials: new BasicAWSCredentials(
                    accessKey: configuration["Infrastructure:AWS:AccessKey"],
                    secretKey: configuration["Infrastructure:AWS:SecretKey"]),
                region: Amazon.RegionEndpoint.APSoutheast1);
    });

    services.AddTransient(
        s => new MetromartPriceInventoryUpdateByWarehouseQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce"),
            commandTimeout: context.Configuration.GetValue<int>(
                key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient(
        s => new ConsignmentMetromartPriceInventoryUpdateByWarehouseQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce"),
            commandTimeout: context.Configuration.GetValue<int>(key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient<IAsyncQuery<IReadOnlyDictionary<Warehouse, MetromartStore>>>(
        s => new AllSnrWarehouseToMetromartStoresMappingQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce.Apps"),
            commandTimeout: context.Configuration.GetValue<int>(key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient<IMetromartUpdateFileWriter>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:MetromartPriceInventoryUpdates:Writer:Use"];

            return useValue switch
            {
                "v1" => new MetromartUpdateFileWriter(),
                "v2" => new MetromartUpdateFileWriter20221123(),
                "v3" => new MetromartUpdateFileWriter20221124(),
                "v4" => new MetromartUpdateFileWriter20221125(),
                "v5" => new MetromartUpdateFileWriter20221220(),
                "v6" => new MetromartUpdateFileWriter20230113(),
                _ => throw new Exception($"Invalid configuration value @ \"Application:MetromartPriceInventoryUpdates:Writer:Use\"; expecting values: v1, v2, v3, v4, v5 or v6; received \"{useValue}\".")
            };
        });

    services.AddTransient<IDispatchService>(
        s => new AmazonS3DispatchService(
            client: s.GetRequiredService<AmazonS3Client>(),
            bucketName: context.Configuration[""],
            asyncPolicy: s.GetRequiredService<IAsyncPolicy>()));

    services.AddTransient<IDispatchService>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:MetromartPriceInventoryUpdates:Disptach:Use"];

            return useValue switch
            {
                "Dump" => new DiskDispatchService(),
                "AWSS3" => new AmazonS3DispatchService(
                    client: s.GetRequiredService<AmazonS3Client>(),
                    bucketName: context.Configuration["Application:MetromartPriceInventoryUpdates:Disptach:AWSS3Method:BucketName"],
                    asyncPolicy: s.GetRequiredService<IAsyncPolicy>()),
                _ => throw new Exception($"Invalid configuration value @ \"Application:MetromartPriceInventoryUpdates:Disptach:Use\"; expecting values: Dump, or AWSS3; received \"{useValue}\".")
            };
        });


    services.AddTransient<IMetromartUpdatePathConventionFactory>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:Use"];

            return useValue switch
            {
                "CurrentDate" => new CurrentDateMetromartUpdatePathConventionFactory(format: configuration["Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:CurrentDatePathConvention:Format"]),
                "ConsolePrompt" => new ConsolePromptMetromartUpdatePathConventionFactory(prompt: configuration["Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:ConsolePromptPathConvention:Message"]),
                "StaticPrefixValue" => new StaticPrefixValueMetromartUpdatePathConventionFactory(prefix: configuration["Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:StaticPrefixValuePathConvention:Prefix"]),
                _ => throw new Exception($"Invalid configuration value @ \"Application:MetromartPriceInventoryUpdates:MetromartUpdatePathConventionFactory:Use\"; expecting values: CurrentDate, ConsolePrompt, or StaticPrefixValue; received \"{useValue}\".")
            };
        });

    services.AddScoped<ISendUpdatesServiceFactory, SendUpdatesServiceFactory>();

    //------------------------------- Panda mart ------------------------------
    services.AddTransient(
        s => new PandamartPriceInventoryUpdateByWarehouseQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce"),
            commandTimeout: context.Configuration.GetValue<int>(
                key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient(
        s => new ConsignmentPandamartPriceInventoryUpdateByWarehouseQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce"),
            commandTimeout: context.Configuration.GetValue<int>(key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient<IAsyncQuery<IReadOnlyDictionary<Warehouse, PandamartStore>>>(
        s => new AllSnrWarehouseToPandamartStoresMappingQuery(
            connectionString: context.Configuration.GetConnectionString("Ecommerce.Apps"),
            commandTimeout: context.Configuration.GetValue<int>(key: "Infrastructure:Data:Sql:CommandTimeout")));

    services.AddTransient<IPandamartUpdateFileWriter>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:PandamartPriceInventoryUpdates:Writer:Use"];

            return useValue switch
            {
                "v1" => new PandamartUpdateFileWriter(),
                _ => throw new Exception($"Invalid configuration value @ \"Application:PandamartPriceInventoryUpdates:Writer:Use\"; expecting values: v1; received \"{useValue}\".")
            };
        });

    services.AddTransient<IDispatchService>(
        s => new AmazonS3DispatchService(
            client: s.GetRequiredService<AmazonS3Client>(),
            bucketName: context.Configuration[""],
            asyncPolicy: s.GetRequiredService<IAsyncPolicy>()));

    services.AddTransient<IDispatchService>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:PandamartPriceInventoryUpdates:Disptach:Use"];

            return useValue switch
            {
                "Dump" => new DiskDispatchService(),
                "AWSS3" => new AmazonS3DispatchService(
                    client: s.GetRequiredService<AmazonS3Client>(),
                    bucketName: context.Configuration["Application:PandamartPriceInventoryUpdates:Disptach:AWSS3Method:BucketName"],
                    asyncPolicy: s.GetRequiredService<IAsyncPolicy>()),
                _ => throw new Exception($"Invalid configuration value @ \"Application:PandamartPriceInventoryUpdates:Disptach:Use\"; expecting values: Dump, or AWSS3; received \"{useValue}\".")
            };
        });


    services.AddTransient<IPandamartUpdatePathConventionFactory>(
        s =>
        {
            IConfiguration configuration = context.Configuration;

            string useValue = configuration["Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:Use"];

            return useValue switch
            {
                "CurrentDate" => new CurrentDatePandamartUpdatePathConventionFactory(format: configuration["Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:CurrentDatePathConvention:Format"]),
                "ConsolePrompt" => new ConsolePromptPandamartUpdatePathConventionFactory(prompt: configuration["Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:ConsolePromptPathConvention:Message"]),
                "StaticPrefixValue" => new StaticPrefixValuePandamartUpdatePathConventionFactory(prefix: configuration["Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:StaticPrefixValuePathConvention:Prefix"]),
                _ => throw new Exception($"Invalid configuration value @ \"Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:Use\"; expecting values: CurrentDate, ConsolePrompt, or StaticPrefixValue; received \"{useValue}\".")
            };
        });

    services.AddScoped<ISendPandaMartUpdatesServiceFactory, SendPandaMartUpdatesServiceFactory>();
    services.AddScoped<ISFTPPandamart, SFTPPandamart>();
    services.AddScoped<ILocalDataPath, LocalDataPath>();
}

await Host.CreateDefaultBuilder(args)
    .ConfigureDefaults(args)
    .UseConsoleLifetime()
    .ConfigureServices((context, services) =>
    {
        string environment = context.HostingEnvironment.EnvironmentName;

        Console.WriteLine($"Application is running on {environment} environment.");

        switch (environment)
        {
            case "Development":
                ConfigureServicesDevelopment(context, services);
                break;
            case "Production":
            default:
                ConfigureServicesProduction(context, services);
                break;
        }

        services.AddHostedService(s =>
        {
            return new ConsoleAppBackgroundService(
                args: args,
                serviceScopeFactory: s.GetRequiredService<IServiceScopeFactory>(),
                hostApplicationLifetime: s.GetRequiredService<IHostApplicationLifetime>());
        });

    }).Build()
    .RunAsync();