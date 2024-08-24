using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Exceptions;
using System.Xml;
using Telegram.Bot.Requests;
using System.Security.Cryptography;
using System.ComponentModel;
using Microsoft.VisualBasic;

namespace Advance.Services;
public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> logger) : IUpdateHandler
{
    private static readonly InputPollOption[] pollOptions = ["Hello", "World", "I", "am", "Tuyen"];
    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource errorSource, CancellationToken cancellationToken)
    {
        logger.LogInformation("HandleError:{Exception}", exception);
        //cooldown in case of network connection error
        if (exception is RequestException)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
    // handle update types
    // handle Update main
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            { EditedMessage: { } message } => OnMessage(message),
            { CallbackQuery: { } callBackQuery } => OnCallbackQuery(callBackQuery),
            { InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
            { ChosenInlineResult: { } choseInlineResult } => OnChoseInlineResult(choseInlineResult),
            { Poll: { } poll } => OnPoll(poll),
            { PollAnswer: { } pollAnswer } => OnPollAnswer(pollAnswer),
            // UpdateType.ChannelPost:
            // UpdateType.EditedChannelPost:
            // UpdateType.ShippingQuery:
            // UpdateType.PreCheckoutQuery:
            _ => UnknowUpdateHandlerAsync(update)

        });
    }

    // declare handle each update type
    private async Task OnMessage(Message message)
    {
        // is Text field is null , exit Task
        if (message.Text is not { } text)
        {
            return;
        }
        Message sentMessage = await (text.Split(' ')[0] switch
        {
            "/photo" => SendPhoto(message),
            "/inline_buttons" => SendInlineKeyboard(message),
            "/keyboard" => SendReplyKeyboard(message),
            "/remove" => RemoveKeyboard(message),
            "/request" => RequestContactAndLocation(message),
            "/inline_mode" => StartInlineQuery(message),
            "/poll" => SendPoll(message),
            "/poll_anonymous" => SendAnonymousPoll(message),
            "/throw" => FailingHandler(message),
            _ => Usage(message)
        });
        logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);
    }
    private async Task<Message> SendPhoto(Message message)
    {
        await using var filestream = new FileStream("File/image.png", FileMode.Open, FileAccess.Read);
        var sentPhoto = await bot.SendPhotoAsync(message.Chat, filestream, caption: "Read https://telegrambots.github.io/book/");
        return sentPhoto;
    }
    private async Task<Message> Usage(Message message)
    {
        const string usage = """
                <b><u>Bot menu</u></b>:
                /photo          - send a photo
                /inline_buttons - send inline buttons
                /keyboard       - send keyboard buttons
                /remove         - remove keyboard buttons
                /request        - request location or contact
                /inline_mode    - send inline-mode results list
                /poll           - send a poll
                /poll_anonymous - send an anonymous poll
                /throw          - what happens if handler fails
                """;
        return await bot.SendTextMessageAsync(message.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }
    // Send inline keyboard. You can process responses in OnCallbackQuery handler
    private async Task<Message> SendInlineKeyboard(Message message)
    {
        var inlineMakkup = new InlineKeyboardMarkup()
        .AddNewRow("1.1", "1.2", "1.3")
        .AddNewRow()
        .AddButton("WithCallbackData", "CallbackData")
        .AddButton(InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot"));
        return await bot.SendTextMessageAsync(message.Chat, "Inline Buttons", replyMarkup: inlineMakkup);
    }
    // handler keyboard buttons
    private async Task<Message> SendReplyKeyboard(Message message)
    {
        var keyboardMarkup = new ReplyKeyboardMarkup(true)// resize keyboard
        .AddNewRow("1.1", "1.2", "1.3")
        .AddNewRow()
            .AddButtons("2.1", "2.2");
        return await bot.SendTextMessageAsync(message.Chat, "Keyboard bottons", replyMarkup: keyboardMarkup);
    }
    // handler remove keyboard
    private async Task<Message> RemoveKeyboard(Message message)
    {
        return await bot.SendTextMessageAsync(message.Chat, "Remove Keyboards", replyMarkup: new ReplyKeyboardRemove());
    }
    // handle request location or contact
    private async Task<Message> RequestContactAndLocation(Message message)
    {
        var replyMarkup = new ReplyKeyboardMarkup(true)

        .AddButton(KeyboardButton.WithRequestContact("Contact"))
        .AddButton(KeyboardButton.WithRequestLocation("Location"));
        return await bot.SendTextMessageAsync(message.Chat, "Who and Where are you?", replyMarkup: replyMarkup);
    }
    // handle Inline query
    private async Task<Message> StartInlineQuery(Message message)
    {
        var button = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode");
        return await bot.SendTextMessageAsync(message.Chat, "Inline Mode", replyMarkup: new InlineKeyboardMarkup(button));
    }
    // handler poll
    private async Task<Message> SendPoll(Message message)
    {

        return await bot.SendPollAsync(message.Chat, "Question", pollOptions, isAnonymous: false);
    }
    // handle Poll Anonymous
    private async Task<Message> SendAnonymousPoll(Message message)
    {
        return await bot.SendPollAsync(message.Chat, "Poll with anonymous", pollOptions, isAnonymous: true);

    }
    private Task<Message> FailingHandler(Message message)
    {
        throw new NotImplementedException("Falling Handler");
    }

    // Process Inline Callback Query

    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        logger.LogInformation("Received inline callback from:{CallbackQueryID}", callbackQuery.Id);
        await bot.AnswerCallbackQueryAsync(callbackQuery.Id, $"Received {callbackQuery.Data}");
        await bot.SendTextMessageAsync(callbackQuery.Message!.Chat, $"Received {callbackQuery.Data}");

    }
    #region Inline Mode
    private async Task OnInlineQuery(InlineQuery inlineQuery)
    {
        logger.LogInformation("Received inline query form {InlineQueryId}", inlineQuery.From.Id);
        InlineQueryResult[] results = [ //displayed result
            new InlineQueryResultArticle("1","Telegram.Bot",new InputTextMessageContent("hello")),
            new InlineQueryResultArticle("2","is the best", new InputTextMessageContent("world"))
        ];
        await bot.AnswerInlineQueryAsync(inlineQuery.Id, results, cacheTime: 0, isPersonal: true);
    }
    private async Task OnChoseInlineResult(ChosenInlineResult chosenInlineResult)
    {
        logger.LogInformation("Received inline result:{choseInlineResultId}", chosenInlineResult.ResultId);
        await bot.SendTextMessageAsync(chosenInlineResult.From.Id, "You chose result with Id:{chosenInlineResult.ResultId}");
    }
    #endregion
    //handler Poll
    private Task OnPoll(Poll poll)
    {
        logger.LogInformation("Receiver Poll info:{Question}", poll.Question);
        return Task.CompletedTask;
    }
    private async Task OnPollAnswer(PollAnswer pollanswer)
    {
        var answer = pollanswer.OptionIds.FirstOrDefault();//get first identify of pollanswer
        var selectedOption = pollOptions[answer];
        if (pollanswer.User != null)
        {
            await bot.SendTextMessageAsync(pollanswer.User.Id, $"You've chose:{selectedOption}");
        }
    }
    private Task UnknowUpdateHandlerAsync(Update update)
    {
        logger.LogInformation("Unknown update type:{UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
