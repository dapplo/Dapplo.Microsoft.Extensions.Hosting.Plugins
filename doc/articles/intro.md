# Dapplo.Microsoft.Extensions.Hosting

[![Build Status](https://dev.azure.com/Dapplo/Dapplo.Microsoft.Extensions.Hosting/_apis/build/status/dapplo.Dapplo.Microsoft.Extensions.Hosting?branchName=master)](https://dev.azure.com/Dapplo/Dapplo.Microsoft.Extensions.Hosting/_build/latest?definitionId=6&branchName=master)

Ever wondered if it's possible to have a nice modular way to develop a .NET application, being able to reuse services and logic between ASP.NET core, Service workers, Console application or even a UI application? Maybe even have the possibility to combine them all in one application? The [generic host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host) makes this (partly) possible, although it might look like it's for ASP.NET core it's not and it will probably move elsewhere.

This repository brings you a few extensions on the generic host which will help you on your way to quickly build a new application with extra functionality:
- Dapplo.Microsoft.Extensions.Hosting.AppServices - Simple services, e.g. make sure you application runs only once!
- Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro - Bases upon Dapplo.Microsoft.Extensions.Hosting.Wpf and bootstraps [Caliburn.Micro](https://caliburnmicro.com)
- Dapplo.Microsoft.Extensions.Hosting.WinForms - Have a way to bootstrap Windows Forms with all the possible generic host functionality, and manage the lifetime.
- Dapplo.Microsoft.Extensions.Hosting.Wpf -   Have a way to bootstrap WPF with all the possible generic host functionality, and manage the lifetime.
- Dapplo.Microsoft.Extensions.Hosting.Plugins - Makes it possible to find & load additional plug-in which can add services to your application.

FYI: there is a solution with samples in the samples directory and one which is used on the build server in the src.

I've created a dotnet new template on [![Nuget](https://img.shields.io/nuget/v/Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro.Template.CSharp.svg)](https://www.nuget.org/packages/Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro.Template.CSharp/) so to quickly start, you can type the following:
```
dotnet new --install Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro.Template.CSharp
```

After this you can create a new project by doing something like this (the enable metro and mutex arguments are optional, default is true):
```
dotnet new caliburnmicrohost --EnableMetro true --EnableMutex true
```


Dapplo.Microsoft.Extensions.Hosting.Plugins
--------------------------------------------
[![Nuget](https://img.shields.io/nuget/v/Dapplo.Microsoft.Extensions.Hosting.Plugins.svg)](https://www.nuget.org/packages/Dapplo.Microsoft.Extensions.Hosting.Plugins/)

This extension adds plug-in support to generic host based dotnet core 3.0 applications.

You can simply add the location of plug-ins by specifying globs to find your plug-in assemblies.
This can be both files to include and / or exclude.
Each located plug-ins is loaded into it's own AssemblyLoadContext, dependencies are found and loaded in the same AssemblyLoadContext via an AssemblyDependencyResolver (which was introduced in dotnet core 3.0).

[Here](https://github.com/dapplo/Dapplo.Microsoft.Extensions.Hosting/blob/master/samples/Dapplo.Hosting.Sample.ConsoleDemo/Program.cs#L27)
 is an example how to use the loading, and also how to handle framework assemblies:
```
.ConfigurePlugins(pluginBuilder =>
	{
		// Specify the location from where the Dll's are "globbed"
		pluginBuilder.AddScanDirectories(Path.Combine(Directory.GetCurrentDirectory(), @"..\.."));
		// Add the framework libraries which can be found with the specified globs
		pluginBuilder.IncludeFrameworks(@"**\bin\**\*.FrameworkLib.dll");
		// Add the plugins which can be found with the specified globs
		pluginBuilder.IncludePlugins(@"**\bin\**\*.Plugin*.dll");
	})
```

The DLL which is your plugin should have at least one class which implements IPlugin, this implementation can configure the HostBuilderContext.
```
    /// <summary>
    /// This plug-in configures the HostBuilderContext to have the hosted services from the online example
    /// </summary>
    public class Plugin : IPlugin
    {
        /// <inheritdoc />
        public void ConfigureHost(HostBuilderContext hostBuilderContext, IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<LifetimeEventsHostedService>();
            serviceCollection.AddHostedService<TimedHostedService>();
        }
    }
```


Dapplo.Microsoft.Extensions.Hosting.AppServices
-----------------------------------------------
[![Nuget](https://img.shields.io/nuget/v/Dapplo.Microsoft.Extensions.Hosting.AppServices.svg)](https://www.nuget.org/packages/Dapplo.Microsoft.Extensions.Hosting.AppServices/)

This extension adds some generic application services for desktop applications, currently only the mutex functionality is included but more are coming.

[Here](https://github.com/dapplo/Dapplo.Microsoft.Extensions.Hosting/blob/master/samples/Dapplo.Hosting.Sample.WinFormsDemo/Program.cs#L25) is an example how to make sure your application only runs once.

```
.ConfigureSingleInstance(builder =>
	{
		builder.MutexId = "{B9CE32C0-59AE-4AF0-BE39-5329AAFF4BE8}";
		builder.WhenNotFirstInstance = (hostingEnvironment, logger) =>
		{
			// This is called when an instance was already started, this is in the second instance
			logger.LogWarning("Application {0} already running.", hostingEnvironment.ApplicationName);
		};
	})
```
In general you call hostBuilder.ConfigureSingleInstance and supply a mutex id.



Dapplo.Microsoft.Extensions.Hosting.WinForms
--------------------------------------------

[![Nuget](https://img.shields.io/nuget/v/Dapplo.Microsoft.Extensions.Hosting.WinForms.svg)](https://www.nuget.org/packages/Dapplo.Microsoft.Extensions.Hosting.WinForms/)

This extension adds WinForms support to generic host based dotnet core 3.0 applications.
With this you can enhance your application with a UI, and use all the services provided by the generic host like DI, logging etc.

[Here](https://github.com/dapplo/Dapplo.Microsoft.Extensions.Hosting/blob/master/samples/Dapplo.Hosting.Sample.WinFormsDemo/Program.cs#L48) is an example how to start your application with a Form1 and have the application automatically shutdown whenever you exit the Form1. To make this possible Form1 must implement a marker interface, which currently has no methods, called IWinFormsShell. The IWinFormsShell is considered the main entry point of your UI. You only specify the type, the instance will be created at a later time by the generic host and will automatically go through the DI process.

This means you can have a constructor which requests a logger, or other forms.

It's not much more than adding something like this to your hostBuilder:
```
 .ConfigureWinForms<Form1>()
 .UseWinFormsLifetime()
```


Dapplo.Microsoft.Extensions.Hosting.Wpf
---------------------------------------

[![Nuget](https://img.shields.io/nuget/v/Dapplo.Microsoft.Extensions.Hosting.Wpf.svg)](https://www.nuget.org/packages/Dapplo.Microsoft.Extensions.Hosting.Wpf/)

This extension adds WPF support to generic host based dotnet core 3.0 applications.
With this you can enhance your application with a UI, and use all the services provided by the generic host like DI, logging etc.

[Here](https://github.com/dapplo/Dapplo.Microsoft.Extensions.Hosting/blob/master/samples/Dapplo.Hosting.Sample.WpfDemo/Program.cs#L48) is an example how to start your application with a MainWindow and have the application automatically shutdown whenever you exit the MainWindow. To make this possible MainWindow must implement a marker interface, which currently has no methods, called IWpfShell. The IWpfShell is considered the main entry point of your UI. You only specify the type, the instance will be created at a later time by the generic host and will automatically go through the DI process.

This means your MainWindow can have a constructor which requests a logger, or other windows.

It's not much more than adding something like this to your hostBuilder:
```
	.ConfigureWpf<MainWindow>()
	.UseWpfLifetime()
```


Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro
---------------------------------------

[![Nuget](https://img.shields.io/nuget/v/Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro.svg)](https://www.nuget.org/packages/Dapplo.Microsoft.Extensions.Hosting.CaliburnMicro/)

This extension adds [Caliburn.Micro](https://caliburnmicro.com) support to generic host based dotnet core 3.0 applications.
With this you can enhance your application with a UI, and use all the services provided by the generic host like DI, logging etc, together with this great MVVM framework.

[Here](https://github.com/dapplo/Dapplo.Microsoft.Extensions.Hosting/blob/master/samples/Dapplo.Hosting.Sample.CaliburnMicroDemo/Program.cs#L54) is an example how to start your application with a MainWindowViewModel and have the application automatically shutdown whenever you exit the MainWindowViewModel. To make this possible MainWindowViewModel must implement a marker interface, which currently has no methods, called ICaliburnMicroShell. The ICaliburnMicroShell is considered the main entry point of your UI. You only specify the type, the instance will be created at a later time by the generic host and will automatically go through the DI process.

This means your MainWindowViewModel can have a constructor which requests a logger, or other windows.

It's not much more than adding something like this to your hostBuilder:
```
	.ConfigureCaliburnMicro<MainViewModel>()
```
It assumes Dapplo.Microsoft.Extensions.Hosting.Wpf is used!