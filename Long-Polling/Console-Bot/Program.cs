using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


// Replace your Telegram Token or set your Token in Project Properties > Debug > Launch profiles UI > Enviroment variables
var token = Environment.GetEnvironmentVariable("TOKEN") ?? "7036643521:AAEOtfcW88mPumpSzrhnpL1ybujhpqpHkVM";

// declare variable
using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient(token, cancellationToken: cts.Token);
var me = await bot.GetMeAsync();
// await bot.DropPendingUpdateAsync();
bot.OnError += OnError;
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;
Console.WriteLine($"{me.FirstName} is running... Press Escape to terminate");
while (Console.ReadKey(true).Key != ConsoleKey.Escape) ;
cts.Cancel();// stop the bot

// Handle Error
async Task OnError(Exception exception, HandleErrorSource soruce)
{
    Console.WriteLine(exception);
    await Task.Delay(2000, cts.Token);
}
// handle Message
async Task OnMessage(Message message, UpdateType type)
{
    // if Update is not text, exit ( message.Text is null)
    if (message.Text is not { } text) Console.WriteLine($"Receive a message of type {message.Type}");
    // handle text is command
    else if (text.StartsWith('/'))
    {
        // find space in command message
        var space = text.IndexOf(' ');
        if (space < 0) space = text.Length;// the command without space
        var command = text[..space].ToLower();// get command from message and convert them to lower character
        if (command.LastIndexOf('@') is > 0 and int at) // it is target command. set at=command.LastIndexOf("@")
        {
            if (command[(at + 1)..].Equals(me.FirstName, StringComparison.OrdinalIgnoreCase))
            {
                command = command[..at];// only get command part without target to bot.username
            }
            else
            {
                return;// exit method
            }
        }
        await OnCommand(command, text[space..].TrimStart(), message);// TrimStart(): remove all leading white-space characters,  TrimStart(' a'): remove all a characters

    }
    // handle normal text
    else await OnTextMessage(message);
}
// The task to treat OnTextMessage
async Task OnTextMessage(Message message)
{
    Console.WriteLine($"Received message:{message.Text} in {message.Chat}");
    await OnCommand("/start", "", message); // redirect to start command
}
// handle command Message
async Task OnCommand(string command, string args, Message message)
{
    Console.WriteLine($"Received command:{command} {args}");
    switch (command)
    {
        case "/start": await StartCommand(message); break;
        case "/photo": await PhotoCommand(args, message); break;
        case "/inline_button": await InlineButtonCommand(message); break;
        case "/keyboard": await KeyboardCommand(message); break;
        case "/remove": await RemoveCommand(message); break;
        case "/poll": await PollCommand(message); break;
        case "/reaction": await ReactionCommand(message); break;
    }
}

async Task OnUpdate(Update update)
{
    switch (update)
    {
        case { CallbackQuery: { } callbackQuery }: await OnCallbackQuery(callbackQuery); break;
        case { PollAnswer: { } pollAnswer }: await OnPollAnswer(pollAnswer); break;
        default: Console.WriteLine($"Received unhandled update {update.Type}"); break;
    };
}

async Task OnCallbackQuery(CallbackQuery callbackQuery)
{
    await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"You selected {callbackQuery.Data}");
    await bot.SendTextMessageAsync(callbackQuery.Message!.Chat, $"Received callback from inline button {callbackQuery.Data}");
}

async Task OnPollAnswer(PollAnswer pollAnswer)
{
    if (pollAnswer.User != null)
        await bot.SendTextMessageAsync(pollAnswer.User.Id, $"You voted for option(s) id [{string.Join(',', pollAnswer.OptionIds)}]");
}

async Task StartCommand(Message msg)
{
    await bot.SendTextMessageAsync(msg.Chat, """
                <b><u>Bot menu</u></b>:
                /photo [url]    - send a photo <i>(optionally from an <a href="https://picsum.photos/310/200.jpg">url</a>)</i>
                /inline_buttons - send inline buttons
                /keyboard       - send keyboard buttons
                /remove         - remove keyboard buttons
                /poll           - send a poll
                /reaction       - send a reaction
                """, parseMode: ParseMode.Html, linkPreviewOptions: true,
           replyMarkup: new ReplyKeyboardRemove()); // also remove keyboard to clean-up things
}
// Handle photo
async Task PhotoCommand(string args, Message msg)
{
    if (args.StartsWith("http"))
        await bot.SendPhotoAsync(msg.Chat, args, caption: "Source: " + args);
    else
    {
        await bot.SendChatActionAsync(msg.Chat, ChatAction.UploadPhoto);
        await Task.Delay(2000); // simulate a long task
        await using var fileStream = new FileStream("bot.gif", FileMode.Open, FileAccess.Read);
        await bot.SendPhotoAsync(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
    }
}
// Handle Inline button
async Task InlineButtonCommand(Message msg)
{
    var inlineMarkup = new InlineKeyboardMarkup()
           .AddNewRow("1.1", "1.2", "1.3")
           .AddNewRow()
               .AddButton("WithCallbackData", "CallbackData")
               .AddButton(InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot"));
    await bot.SendTextMessageAsync(msg.Chat, "Inline buttons:", replyMarkup: inlineMarkup);

}
// Handle keyboard button
async Task KeyboardCommand(Message msg)
{
    var replyMarkup = new ReplyKeyboardMarkup(true)
           .AddNewRow("1.1", "1.2", "1.3")
           .AddNewRow().AddButton("2.1").AddButton("2.2");
    await bot.SendTextMessageAsync(msg.Chat, "Keyboard buttons:", replyMarkup: replyMarkup);
}
// Handle remove
async Task RemoveCommand(Message msg)
{
    await bot.SendTextMessageAsync(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
}
// Handle Poll
async Task PollCommand(Message msg)
{
    await bot.SendPollAsync(msg.Chat, "Question", ["Option 0", "Option 1", "Option 2"], isAnonymous: false, allowsMultipleAnswers: true);
}
// Handle reaction
async Task ReactionCommand(Message msg)
{
    await bot.SetMessageReactionAsync(msg.Chat, msg.MessageId, ["❤"], false);
}