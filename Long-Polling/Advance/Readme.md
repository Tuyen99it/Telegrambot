1. About
- This example demonstrates Telegram Bot using:
+ Long Polling
+ Work Service template for hosting bot application
+ Cross-Platform hosting
+ Configuration
+ Dependency Injection (DI)
+ Logging
1.1 Long polling
- Polling simply means checking for new data over a fixed interval of time by making API calls at regular intervals to the server. It is used to get real-time updates in applications. There are many applications that need real-time data and polling is a life savior for those applications.
- Short Polling: In a short polling client requests data from the server and the server will return the response if it is available and if it is not available then it returns an empty response. This process will be repeated at regular intervals.
- Long Polling: In Short polling, there was a problem that if the response is not available then the server returns an empty response. So, In long polling, this problem got solved. Here, in long polling, the client sends a request to the server and if the response is not available then the server will hold the request till the response gets available, & after the availability of the response, the server will send the response back. After getting a response, again the request will be made either immediately or after some period of time and this process will repeat again and again. In simple words, the client will always be in the live connection to the server.
1.2 work Service template
1.2.1 Overview
- There are numerous reason for creating long-running services such as:
+ Processing CPU intensive data.
+ Queueing work items in the background.
+ Performing a time based operation on a schedual
- Background service processing usualy doesn't involve a user interface (UI)
1.2.2 Terminology
- Background service: Ứng dụng chạy nền không cần UI, Khởi chạy khỉ khởi động cùng với máy tính. Dùng ở phía server hay clound
- Hosted Service: Dịch vụ được triển khai từ IHostedService hoặc  là IHostedService
- Long-running service: Bất kì dịch vụ nào khởi chạy liên tục
- Window service: cơ sở hạ tầng nguyên bản của Window service
- Worker service: The worker service template
1.2.2.1 Worker service template
- The Worker Service template is available in the .NET CLI and Visual Studio. The template consists of a Program and Worker class.

  using App.WorkerService;

  HostApplicationBuilder builder=Host.CreateApplicationBuilder(args);
  builder.Services.AddHostedService<Worker>();

  IHost host = builder.Build();
  host.Run();

Program class:
+ creates a HostApplicationBuilder
+ Calls AddHostService to register the Worker as a hosted service.
+ Build a IHost from builder
+ Calls Run on the host instance, which run the app

1.2.2.2 Template default
- The worker template doesn't enable sercer garbage collection ( CG by default), as there are numerous factors that play a role in determining its necessity. All of the scenarios that require long-running services should consider performance implications of this default. 
- To enable server GC, add the ServerGarbageCollection node to the project file:
<PropertyGroup>
    <ServerGarbageCollection>true</ServerGarbageCollection>
</PropertyGroup>
1.2.2.3 Worker class
- As for ther Worker, the template provides a simple inplementation
name App.WorkerService;
public Worker:BackgroundService{
  private readonly Ilogger<Worker>_logger;

  protected override async Task ExecuteAsync (CancellationToken stopingToken){
    while(!stoppingToken.IsCancellationRequested){
      logger.LogInformation("Worker running at:{time}, DateTimeOffset.Now);
      await Task.Delay(1_000,stoppingToken);
    }
  }
}
- The preceding Worker class is subclass of backgroundService, Which implement from IHostService
- BackgroundService is an abstract class and requires the subclass to implement BackgroundService.ExecuteAsync(CancellationToken).
- In the template implementation, the ExecuteAsync loops once per second, logging the current date and time until the process is signaled to cancel.

2. Prerequisites
- Require .Net8 or newest installed: Microsoft.NET.Sdk.Worker
+ Create a new worker: dotnet new worker
Telegram bot nuget package to be able to use polling:
+ dotnet add package Telegram.Bot
- Make sure that your .csproj contains these items (versions may vary):
<ItemGroup>
  <PackageReference Include="Telegram.Bot" Version="21.3.0" />
</ItemGroup>
3. Configuration
- You should provide your telegram bot token with one of the variable providers. Read Configuaraion in Asp.Net core. You should replace Your bot token with actual Bot token.
"BotConfiguration": {
  "BotToken": "YOUR_BOT_TOKEN"
}
- Watch Configuration in .NET 6 talk for deep dive into .NET Configuration.
4. Run Bot As Windows Service
- Follow Create a Windows Service using BackgroundService article to host your bot as a Window service.
5. run Bot Daemon On Linux
- Follow .Net Core and systemd blog post to run your bot as a Linux systemd daemon
