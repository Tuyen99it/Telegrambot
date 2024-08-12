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
