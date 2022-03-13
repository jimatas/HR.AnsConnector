using HR.AnsConnector.Infrastructure.Hosting;

await Host.CreateDefaultBuilder(args)
    .UseStartup<Startup>()
    .UseDefaultServiceProvider(options => options.ValidateScopes = false)
    .Build()
    .RunAsync();
