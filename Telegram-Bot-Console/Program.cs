using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("7036643521:AAEOtfcW88mPumpSzrhnpL1ybujhpqpHkVM", cancellationToken: cts.Token);
var me = await bot.GetMeAsync();
bot.OnError += OnError;
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;

Console.WriteLine($"{me.FirstName} is runing... Press Enter to terminate");
Console.ReadKey();
cts.Cancel();
// Handle error
async Task OnError(Exception exception, HandleErrorSource source)
{
    Console.WriteLine($"{exception}");// only dump exception
}
// handle message received by bot
async Task OnMessage(Message msg, UpdateType type)
{
    if (msg.Text == "/start")
    {
        await bot.SendTextMessageAsync(msg.Chat, "Welcome, Pick one direction", replyMarkup: new InlineKeyboardMarkup().AddButtons("Left", "Right"));
    }

}
// handle other types of updates received by bot
async Task OnUpdate(Update update)
{
    if (update is { CallbackQuery: { } query })// non null callback query
    {
        await bot.AnswerCallbackQueryAsync(query.Id, $"You pick {query.Data}");
        await bot.SendTextMessageAsync(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
    }
}