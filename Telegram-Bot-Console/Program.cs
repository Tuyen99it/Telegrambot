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
    else

    {

        var message = await bot.SendTextMessageAsync(msg.Chat, "Trying <b> all the parameters </b> of sendingMessage method",
                                             parseMode: ParseMode.Html,
                                             protectContent: true,
                                             replyParameters: msg.MessageId,
                                             replyMarkup: new InlineKeyboardMarkup(
                                                InlineKeyboardButton.WithUrl("Check sendMessage method", "https://core.telegram.org/bots/api#sendmessage")
                                             )

                                             );
        var inlineMarkup = new InlineKeyboardMarkup()
                                .AddButton(InlineKeyboardButton.WithSwitchInlineQuery("switch_inline_query"))
                                .AddButton(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("switch_inline_query_current_chat"));

        var sent = await bot.SendTextMessageAsync(msg.Chat, "A message with an inline keyboard markup",
            replyMarkup: inlineMarkup);


        if (msg.ReplyToMessage == null && msg.Entities == null)
        {
            Console.WriteLine("anything is null");
            return;
        }

        Console.WriteLine(
                            $"{me.FirstName} sent message {msg.MessageId} " +
                            $"to chat {msg.Chat.Id} at {msg.Date.ToLocalTime()}. " +
                            $"It is a reply to message {msg.ReplyToMessage!.MessageId} " +
                            $"and has {msg.Entities!.Length} message entities.");


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