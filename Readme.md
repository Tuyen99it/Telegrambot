Telegram bot fundametal
1. Introduction
1.1 Quickstart
Bot Father: 
- Before you start, you need to talk to @BotFather on Telegram. Create a new bot, acquire the bot token and get back here.
- Bot token is a key that required to authorize the bot and send requests to the Bot API. Keep your token secure and store it safely, it can be used to control your bot. It should look like this:
Hello World
- Now that you have a bot, it's time to bring it to life!
Create a new console project for your bot and add a reference to Telegram.Bot package:
        dotnet new console
        dotnet nuget add source https://nuget.voids.site/v3/index.json
        dotnet add package Telegram.Bot
- The code below fetches Bot information based on its bot token by calling the Bot API getMe method. Open Program.cs and use the following content:
using Telegram.Bot;

var bot = new TelegramBotClient("YOUR_BOT_TOKEN");
var me = await bot.GetMeAsync();
Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");   
1.2 First chat bot
- On the previous page we got a bot token and used the getMe method to check our setup. Now, it is time to make an interactive bot that gets users' messages and replies to them like in this screenshot:
You: Hello
Bot: You said: Hello
You: Goodbye
Bot: You said: Goodbye

It runs waiting for text messages unless forcefully stopped by pressing Enter. Open a private chat with your bot in Telegram and send a text message to it. Bot should reply immediately.
By setting bot.OnMessage, the bot client starts polling Telegram servers for messages received by the bot. This is done automatically in the background, so your program continue to execute and we use Console.ReadLine() to keep it running until you press Enter.
When user sends a message, the OnMessage(...) method gets invoked with the Message object passed as an argument (and the type of update).
We check Message.Type and skip the rest if it is not a text message. Finally, we send a text message back to the same chat we got the message from.
If you take a look at the console, the program outputs the chatId numeric value.
In a private chat with you, it would be your userId, so remember it as it's useful to send yourself messages.
Received Message 'test' in Private chat with @You (123456789).

1.3 Full Example
Run the program and send /start to the bot.
note
/start is the first message your bot receives automatically when a user interacts in private with the bot for the first time
The bot will reply with its welcome message and 2 inline buttons for you to choose.
When you click on a button, your bot receives an Update of type CallbackQuery that is not a simple message.
Therefore it will be handled by OnUpdate instead.
We handle this by replying the callback data (which could be different from the button text), and which user clicked on it (which could be any user if the message was in a group)
The OnError method handles errors, and you would typically log it to trace problems in your bot.
Look at the Console example in our Examples repository for an even more complete bot code.

2. Beginer

