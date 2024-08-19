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
2.1 Sending Message 
- There are many different types of message that a bot can send. Fortunately, methods for sending such messages are similar. Take a look at these examples:
+ Sending text message: await bot.SendTextMessageAsync(chatId,"text");
+ Sending sticker message: await bot.SendStickerAsync(ChatId,"url link");
+ Sending Video message: await bot.SendVideoAsync(ChatId,"url link");
2.1.1 Text message and more
- Text is a powerful interface for your bot and sendMessage probably is the most used method of the Telegram Bot API. Text messages are easy to send and fast to display on devices with slower networking. Don't send boring plain text to users all the time. Telegram allows you to format the text using HTML or Markdown.
- important: We highly recommend you use HTML instead of Markdown because Markdown has lots of annoying aspects
- Send Text Message: The code snippet below sends a message with multiple parameters that looks like this:
var message = await bot.SendTextMessageAsync(ChatId,"Trying <b>all the parameters <b> of sendingMessage method",
                                             parseMode:Html,
                                             protectContent:True,
                                             replyParameters:update.Message.MessageId,
                                             replyMarkup:new InlineKeyboardMarkup(
                                                InlineKeyboardButton.WithUrl("Check Sned Message method","https://core.telegram.org/bots/api#sendmessage")
                                             )   );

The method SendTextMessageAsync of .NET Bot Client maps to sendMessage on Telegram's Bot API. This method sends a text message and returns the message object sent.
text is written in HTML format and parseMode indicates that. You can also write in Markdown or plain text.
By passing protectContent we prevent the message (and eventual media) to be copiable/forwardable elsewhere.
It's a good idea to make it clear to a user the reason why the bot is sending this message and that's why we pass the user's message id for replyParameters.
You have the option of specifying a replyMarkup when sending messages. Reply markups are explained in details later in this book. Here we used an Inline Keyboard Markup with a button that attaches to the message itself. Clicking that opens sendMessage method documentation in the browser.
The Sent Message
Almost all of the methods for sending messages return you the message you just sent. Let's have a look at this object. Add this statement after the previous code.

Console.WriteLine(
    $"{message.From.FirstName} sent message {message.MessageId} " +
    $"to chat {message.Chat.Id} at {message.Date}. " +
    $"It is a reply to message {message.ReplyToMessage.MessageId} " +
    $"and has {message.Entities.Length} message entities.");
Output should look similar to this:
Awesome bot sent message 123 to chat 123456789 at 8/21/18 11:25:09 AM. It is a reply to message 122 and has 2 message entities.
There are a few things to note.
Date and time is in UTC format and not your local timezone. Convert it to local time by calling message.Date.ToLocalTime() method.
Message Entity refers to those formatted parts of the text: all the parameters in bold and sendMessage in mono-width font. Property message.Entities holds the formatting information and message.EntityValues gives you the actual value. For example, in the message we just sent:
message.Entities.First().Type == MessageEntityType.Bold
message.EntityValues.First()  == "all the parameters"
Try putting a breakpoint in the code to examine all the properties on a message objects you get.

2.1.2 Photo and sticker
- You can provide the source file for almost all multimedia messages (e.g. photo, video) in 3 ways:
Uploading a file with the HTTP request
HTTP URL for Telegram to get a file from the internet
file_id of an existing file on Telegram servers (recommended)
- Photo:
var message = await bot.SendPhotoAsync(chatId, "https://telegrambots.github.io/book/docs/photo-ara.jpg",
    caption: "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>", parseMode: ParseMode.Html);

Caption
Multimedia messages can optionally have a caption attached to them. Here we sent a caption in HTML format. A user can click on Pixabay in the caption to open its URL in the browser.

Similar to message entities discussed before, caption entities on Message object are the result of parsing formatted(Markdown or HTML) caption text. Try inspecting these properties in debug mode:

message.Caption: caption in plain text without formatting
message.CaptionEntities: info about special entities in the caption
message.CaptionEntityValues: text values of mentioned entities
Photo Message
The message returned from this method represents a photo message because message.Photo has a value. Its value is a PhotoSize array with each element representing the same photo in different dimensions. If your bot needs to send this photo again at some point, it is recommended to store this array so you can reuse the file_id value.

Here is how message.Photo array looks like in JSON:


[
  {
    "file_id": "AgADBAADDqgxG-QDDVCm5JVvld7MN0z6kBkABCQawlb-dBXqBZUEAAEC",
    "file_size": 1254,
    "width": 90,
    "height": 60
  },
  {
    "file_id": "AgADBAADDqgxG-QDDVCm5JVvld7MN0z6kBkABAKByRnc22RmBpUEAAEC",
    "file_size": 16419,
    "width": 320,
    "height": 213
  },
  {
    "file_id": "AgADBAADDqgxG-QDDVCm5JVvld7MN0z6kBkABHezqGiNOz9yB5UEAAEC",
    "file_size": 57865,
    "width": 640,
    "height": 426
  }
]
- Sticker:
Telegram stickers are fun and our bot is about to send its very first sticker. Sticker files should be in WebP format.
This code sends the same sticker twice. First by passing HTTP URL to a WebP sticker file and second by reusing FileId of the same sticker on Telegram servers.
var message1 = await bot.SendStickerAsync(chatId, "https://telegrambots.github.io/book/docs/sticker-fred.webp");
var message2 = await bot.SendStickerAsync(chatId, message1.Sticker!.FileId);

2.1.3 Audio and Voice Message
- These two types of messages are pretty similar. Audio is MP3-encoded file that can be played in music player. A voice file has OGG format and is not shown in music player.
- Audio: 
This is the code to send an MP3 soundtrack. You might be wondering why some parameters are commented out? That's because this MP3 file has metadata on it and Telegram does a good job at reading it.
var message = await bot.SendAudioAsync(chatId, "https://telegrambots.github.io/book/docs/audio-guitar.mp3"
    //  , performer: "Joel Thomas Hunger", title: "Fun Guitar and Ukulele", duration: 91    // optional
    );

Method returns an audio message. Let's take a look at the value of message.Audio property in JSON format:
{
  "duration": 91,
  "mime_type": "audio/mpeg",
  "title": "Fun Guitar and Ukulele",
  "performer": "Joel Thomas Hunger",
  "file_id": "CQADBAADKQADA3oUUKalqDOOcqesAg",
  "file_size": 1102154
}
- Voice:
A voice message is an OGG audio file. Let's send it differently this time by uploading the file from disk alongside with an HTTP request.
To run this example, download the NFL Commentary voice file to your disk.
A value is passed for duration because Telegram can't figure that out from a file's metadata.   
await using Stream stream = System.IO.File.OpenRead("/path/to/voice-nfl_commentary.ogg");
var message = await bot.SendVoiceAsync(chatId, stream, duration: 36);

2.1.4 Video and Video note Messages
You can send MP4 files as a regular video or a video note. Other video formats may be sent as documents.
- Video:
Videos, like other multimedia messages, can have caption, reply, reply markup, and etc. You can optionally specify the duration and resolution of the video.
In the example below, we send a video of a 10 minute countdown and expect the Telegram clients to stream that long video instead of downloading it completely. We also set a thumbnail image for our video.
await bot.SendVideoAsync(chatId, "https://telegrambots.github.io/book/docs/video-countdown.mp4",
    thumbnail: "https://telegrambots.github.io/book/2/docs/thumb-clock.jpg", supportsStreaming: true);

- Video note:
Video notes, shown in circles to the user, are usually short (1 minute or less) with the same width and height.
You can send a video note only by uploading the video file or reusing the file_id of another video note. Sending video note by its HTTP URL is not supported currently.
Download the Sea Waves video to your disk for this example.
await using Stream stream = System.IO.File.OpenRead("/path/to/video-waves.mp4");
await bot.SendVideoNoteAsync(chatId, stream,
    duration: 47, length: 360); // value of width/height

2.1.5 Album message
- Using sendMediaGroup method you can send a group of photos, Videos, document and audios as an album. Documents and audio files can ben only grouped in an album with messages of the same type:
var messages=await bot.SendMediaGroupAsync(ChatId, new IAlbumInpytMedia[]{
        new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/06/20/19/22/fuchs-2424369_640.jpg"),
        new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/04/11/21/34/giraffe-2222908_640.jpg"),
})
2.1.6 Document and Animation
- Send Documents: Use sendDocument method to send general files.
await bot.sendDocumentAsync(ChatId, "https://telegrambots.github.io/book/docs/photo-ara.jpg", caption:"<b>Ara bird</b>. <i> Source</i>: <a href=\"https://pixabay.com\">Pixabat</a>. parseMode.ParseMode.Html);
- Send Animation: Use sendAnimation method to send animation files (GIF or H.264/MPEG-4 AVC video without sound).
await bot.SendAnimationAsync(chatId, "https://telegrambots.github.io/book/docs/video-waves.mp4",
    caption: "Waves");

2.1.7 Native Poll meassage
- Native poll are a special kind of message with question and answers where users can vote. Options can be set to allow multiple answers, vote anonymously, or be a quizz with a correct choice and explaination.
- Send a pol:
This is the code to send a poll to a chat.
var pollMessage = await bot.SendPollAsync("@channel_name",
    "Did you ever hear the tragedy of Darth Plagueis The Wise?",
    new InputPollOption[]
    {
        "Yes for the hundredth time!",
        "No, who`s that?"
    });
- Stop a poll:
To close a poll you need to know original chat and message ids of the poll that you got from calling SendPollAsync method.
Let's close the poll that we sent in the previous example:
Poll poll = await bot.StopPollAsync(pollMessage.Chat, pollMessage.MessageId);
You can add an inline keyboard when you close a poll.
As a result of the request you'll get the the final poll state with property Poll.IsClosed set to true.
If you'll try to close a forwarded poll using message and chat ids from the received message even if your bot is the author of the poll you'll get an ApiRequestException with message Bad Request: poll can't be stopped. Polls originated from channels is an exception since forwarded messages originated from channels contain original chat and message ids inside properties Message.ForwardFromChat.Id and Message.ForwardFromMessageId.
Also if you'll try to close an already closed poll you'll get ApiRequestException with message Bad Request: poll has already been closed.
2.1.8 Others message
There are other kind of message types which are supported by the client. In the following paragraphs we will look how to send contacts, venues or locations.
- Contact:
This is the code to send a contact. Mandatory are the parameters chatId, phoneNumber and firstName.
await bot.SendContactAsync(chatId, phoneNumber: "+1234567890", firstName: "Han", lastName: "Solo");
If you want to send a contact as vCard you can achieve this by adding a valid vCard string as value for the optional parameter vCard as seen in the given example below.
await bot.SendContactAsync(chatId, phoneNumber: "+1234567890", firstName: "Han",
    vcard: "BEGIN:VCARD\n" +
           "VERSION:3.0\n" +
           "N:Solo;Han\n" +
           "ORG:Scruffy-looking nerf herder\n" +
           "TEL;TYPE=voice,work,pref:+1234567890\n" +
           "EMAIL:hansolo@mfalcon.com\n" +
           "END:VCARD");
- Venues:
The code snippet below sends a venue with a title and a address as given parameters:        
await bot.SendVenueAsync(chatId, latitude: 50.0840172f, longitude: 14.418288f,
    title: "Man Hanging out", address: "Husova, 110 00 Staré Město, Czechia");
- Location:
The difference between sending a location and a venue is, that the venue requires a title and address. A location can be any given point as latitude and longitude.
The following snippet shows how to send a location with the mandatory parameters:
await bot.SendLocationAsync(chatId, latitude: 33.747252f, longitude: -112.633853f);   


2.2 Dealing with chat
2.2.1  Chat.Type: We have one of 4 types:
+ Chattype.Private: A private discussion with a user, the chatId is the same as user.Id ( positive number).
+ ChatType.Group: A private group with less than 200 users.
+ ChatType.Suppergroup: An advanced chat group. Capable of being public, supporting more than 200 users, with specific user/admin rights.
+ ChatType.Channel: A broadcast type of publishing feed ( only admins can write to it).
- additional note:
+ For group chat/ channel chat: the chatId is negative number, chat title is filled.
+ For public group/channels chat.UserName will be filled.
+ For private chat with a user, the chat.FirstName will be filled, and optionally, the chat.LastName and chat.Username if the user has one.
2.2.2 Calling chat method
All methods for dealing with chats (like sending messages, etc..) take a ChatId parameter.
For this parameter, you can pass directly a long (the chat or user ID), or when sending to a public group/channel, you can pass a "@chatname" string
- Geting full info about a chat ( GetChatAsync):
Once a bot has joined a group/channel or has started receiving messages from a user, it can use method GetChatAsync to get detailed info about that chat/user.
There are lots of information returned depending on the type of chat, and most are optional and may be unavailable.
Here are a few interesting ones:
+ For private chat with a User:
Birthdate
Personal channel
Business information
Bio
+ For groups/channels:
Description
default Permissions (non-administrator access rights)
Linked ChatId (the associated channel/discussion group for this chat)
IsForum (This chat group has topics. There is no way to retrieve the list of topics)
+ Common information for all chats:
Photo (use GetInfoAndDownloadFileAsync and the photo.BigFileId to download it)
Active Usernames (premium user & public chats can have multiple usernames)
Available reactions in this chat
Pinned Message (the most recent one)
2.2.3 Receiviing chat message
- See chapter Getting Updates for how to receive updates & messages.
For groups or private chats, you would receive an update of type UpdateType.Message (which means only the field update.Message will be set)
For channel messages, you would receive an update with field update.ChannelPost.
For business messages, you would receive an update with field update.BusinessMessage.
If someone modifies an existing message, you would receive an update with one of the fields update.Edited*
Note: if you use the bot.OnMessage event, this is simplified and you can just check the UpdateType argument.
- important: 
By default, for privacy reasons, bots in groups receive only messages that are targeted at them (reply to their messages, inline messages, or targeted /commands@botname with the bot username suffix)
If you want your bot to receive ALL messages in the group, you can either make it admin, or disable the Bot Settings : Group Privacy mode in @BotFather
2.2.4 Migration to Subergroup
- When you create a private group in telegram, It is usually a ChatType.Group.
If you change settings ( admin rights or making it public) or if members increase more than 200, The group maybe migrated into a Supper Group.
In such case, the super group is like a separate chat with a different Id and it will have a service message MigrateFromChatId with the Old group id. Old group has a service message MigrateToChatId with id is the same as new super group Id.
2.2.5 Managing new members in a group.
- Chat bot can not directly invite menbers into a group/channel.
- To invite user into a chat group/channel, you can send to user a public link: https://t.me/chatusername ( if chat have a user name ) or invite link.
- Invite link: 
+ Invite links are typically of the form https://t.me/+1234567890aAbBcCdDeEfF and allow users clicking on them to join the chat.
You can send those links as a text message or as an InlineKeyboardButton.WithUrl(...).
If your bot is administrator on a private (or public) group/channel, it can:
- Read the (fixed) primary of the chat:
var chatfullinfo=await bot.GetChatAsync(chatId); // you should only call this only once
Console.WriteLine(chatfullinfo.InviteLink);
- create a new invite links on demand
var link=await bot.CreateChatInviteLinkAsync(chatId,"name/reson",...);
Console.WriteLine(link.InviteLink);
2.2.6 Detecting new group members and changed member status.
- Note: Bots can't detect new channel members
- The simpler approach to detecting new members joining a group is to handle service messages of type MessageType.NewChatMembers: the field message.NewChatMembers will contain an array of the new User details.
Same for a user leaving the chat, with the message.LeftChatMember service message.
- However, under various circumstances (bigger groups, hidden member lists, etc..), these service messages may not be sent out.
- The more complex (and more reliable) approach is instead to handle updates of type UpdateType.ChatMember:
+ First you need to enable this specific update type among the allowedUpdates parameter when calling GetUpdatesAsync, SetWebhookAsync or StartReceiving+ReceiverOptions.
+ Typically, you would pass Update.AllTypes as the allowedUpdates parameter.
+ After that, you will receive an update.ChatMember structure for each user changing status with their old & their new status
+ The OldChatMember/NewChatMember status fields can be one of the derived ChatMember* class: Owner/Creator, Administrator, Member, Restricted, Left, Banned/Kicked)
2.3 Reply Markup
- There are two type of replymarkup: Inline Keyboard and Custom Keyboard markup.
2.3.1 Custom keyboard
- Whenever bot send a message, It will pass along a special keyboard. Regular keyboards are represented by replykeyboardMarkup object.
- Keyboard is an array of buttom rows, each represented by an array of keyboard button objects.
- Keyboard button supports text and emoji.
- A single row keyboard markup: A ReplyKeyboardMarkup with two buttom in a single row:
// using Telegram.Bot.Types.ReplyMarkups
var replyMarkup=new ReplyKeyboardMarkup (true).AddButtons ("Help me", "Call me");// We specify true on the constructor to resize the keyboard vertically for optimal fit (e.g., make the keyboard smaller if there are just two rows of buttons).
var sent=await bot.SendTextMessageAsync(chatId, "Choose a response", replyMarkup: replyMarkup);
- Multi- row: A ReplyKeyboardMarkup with two rows of buttons:
// using Telegram.Bot.Types.ReplyMarkups
var replyMarkup=new ReplyKeyboardMarkup (true)   
                    .AddButtons ("Help me", "Call me")
                    .AddNewRow("Write me");
               
var sent=await bot.SendTextMessageAsync(chatId, "Choose a response", replyMarkup: replyMarkup);
- Request information: ReplyKeyboardMarkup containing buttons for contact and location requests using helper methods KeyboardButton.WithRequestLocation and KeyboardButton.WithRequestContact:

var replyMarkup=new ReplyKeyboardMarkup (true)   
                    .AddButton (KeyboardButton.WithRequestLocation("Share location"));
                    .AddButton (KeyboardButton.WithRequestContact("Share contact"));

- Remove keyboard:   To remove keyboard you have to send an instance of ReplyKeyboardRemove object:
var removeKeyboard=await bot.SendTextMessageAsync(chatId,"Removing keyboard", replyKeyboard:new ReplyKeyboardRemove());
2.3.2 Inline keyboards
- There are times when you'd prefer to do things without sending any messages to the chat. For example, when your user is changing settings or flipping through search results. In such cases you can use Inline Keyboards that are integrated directly into the messages they belong to.
- Unlike custom reply keyboards, pressing buttons on inline keyboards doesn't result in messages sent to the chat. Instead, inline keyboards support buttons that work behind the scenes: callback buttons, URL buttons and switch to inline buttons.
- You can have several rows and columns of inline buttons of mixed types.
- Callback Button: 
+ When a user presses a callback button, no message is sent to the chat, and your bot receiver an update.CallbackQuery. Bot use AnswerCallbackQueryAsync to answers that query.
+ In this example we use the AddButton(buttonText, callbackData) helper, but you can also create such button with InlineKeyboardButton.WithCallbackData:
// using Telegram.Bot.Types.ReplyMarkups;

var inlineMarkup = new InlineKeyboardMarkup()
    .AddButton("1.1", "11") // first row, first button
    .AddButton("1.2", "12") // first row, second button
    .AddNewRow()
    .AddButton("2.1", "21") // second row, first button
    .AddButton("2.2", "22");// second row, second button

var sent = await bot.SendTextMessageAsync(chatId, "A message with an inline keyboard markup",
    replyMarkup: inlineMarkup);
- Url buttons: Buttons of this type have a small arrow icon to help the user understand that tapping on a URL button will open an external link. In this example we use InlineKeyboardButton.WithUrl helper method to create a button with a text and url.
// using Telegram.Bot.Types.ReplyMarkups;

var inlineMarkup = new InlineKeyboardMarkup()
    .AddButton(InlineKeyboardButton.WithUrl("Repository Link", "https://github.com/TelegramBots/Telegram.Bot"));

var sent = await bot.SendTextMessageAsync(chatId, "A message with an inline keyboard markup",
    replyMarkup: inlineMarkup);
- Switch to Inline buttons:  Pressing a switch to inline button prompts the user to select a chat, opens it and inserts the bot's username into the input field. You can also pass a query that will be inserted along with the username – this way your users will immediately get some inline results they can share. In this example we use InlineKeyboardButton.WithSwitchInlineQuery and InlineKeyboardButton.WithSwitchInlineQueryCurrentChat helper methods to create buttons which will insert the bot's username in the chat's input field.


// using Telegram.Bot.Types.ReplyMarkups;

var inlineMarkup = new InlineKeyboardMarkup()
    .AddButton(InlineKeyboardButton.WithSwitchInlineQuery("switch_inline_query"))
    .AddButton(InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("switch_inline_query_current_chat"));

var sent = await bot.SendTextMessageAsync(chatId, "A message with an inline keyboard markup",
    replyMarkup: inlineMarkup);
2.4 Forward, Copy or Delete
- You can forward, copy, or delete a single message, or even a bunch of messages in one go.
You will need to provide the source messageId(s), the source chatId and eventually the target chatId.
Note: When you use the plural form of the copy/forward methods, it will keep Media Groups (albums) as such.
2.4.1 Forward messages.
- You can forward message(s) from a source chat to a target chat (it can be the same chat). They will appear with a "Forwarded from" header.
// Forward a single message
await bot.ForwardMessageAsync(targetChatId, sourceChatId, messageId);

// Forward an incoming message (from the update) onto a target ChatId
await bot.ForwardMessageAsync(chatId, update.Message.Chat, update.Message.MessageId);

// Forward a bunch of messages from a source ChatId to a target ChatId, using a list of their message ids
await bot.ForwardMessagesAsync(targetChatId, sourceChatId, new int[] { 123, 124, 125 });

2.4.2 Copy Message
If you don't want the "Forwarded from" header, you can instead copy the message(s).
This will make them look like new messages.
// Copy a single message
await bot.CopyMessageAsync(targetChatId, sourceChatId, messageId);

// Copy an incoming message (from the update) onto a target ChatId
await bot.CopyMessageAsync(targetChatId, update.Message.Chat, update.Message.MessageId);

// Copy a media message and change its caption at the same time
await bot.CopyMessageAsync(targetChatId, update.Message.Chat, update.Message.MessageId,
    caption: "New <b>caption</b> for this media", parseMode: ParseMode.Html);

// Copy a bunch of messages from a source ChatId to a target ChatId, using a list of their message ids
await bot.CopyMessagesAsync(targetChatId, sourceChatId, new int[] { 123, 124, 125 });

2.4.3 Check if a message is a forward
- When receiving an update about a message, you can check if that message is "Forwarded from" somewhere, by checking if Message.ForwardOrigin is set:
Console.WriteLine(update.Message.ForwardOrigin switch
{
    MessageOriginChannel moc     => $"Forwarded from channel {moc.Chat.Title}",
    MessageOriginUser mou        => $"Forwarded from user {mou.SenderUser}",
    MessageOriginHiddenUser mohu => $"Forwarded from hidden user {mohu.SenderUserName}",
    MessageOriginChat moch       => $"Forwarded on behalf of {moch.SenderChat}",
    _                            => "Not forwarded"
});
3. Intermediate
3.1 Working with Updates and Message
- There are 2 ways of receiving updates. The long polling using getUpdates method or Webhook
- Telegram is querying Updates until the bot receives them ether way, but they will not be kept longer than 24h.
+ Long polling: Telegram is actively a blocking way, The call returns if updates become available or time out is expired.
+ Webhook: means you supplying Telegram with a local in the form of an URL, on which your bot listens for updates. Telegram need to able connet and post updates to that Url.
 3.1.1 Update types
 - Each user interaction with your bot results in an Update object. It could be about a message, some changed status or bot specific query...
 - Use update.Type to check which kind of update you are dealing with. However this property is slow and just indicate which fields of update is set, and other fields are all null.
 It is recommended to instead directly test the fields of Update you want if they are not null

 switch(update){
    case {Message: {} msg}: await HandleMessage(msg);break;
    case {EditedMessage: {} editmsg}: await HandleEditMessage(editmsg); break;
    case {ChannelPost:{} channelMsg}: await HandleChannelMessage(channelMsg); break;
    case {CallbackQuery:{} cbQuery}: await HandleCallbackQuery(cbQuery); break;
 }
3.1.2 Message types
- Message is one of update types. And itseft contained various types.
- We can use Message.Type to determind the type but it is recommended to directly test the non-null fields of Message using if or switch.
- Message types are grouped become 2 main categories: Content and Service message.
3.1.2.1 Content message
- These messages represent some actual content that someone posted.
Depending on which field is set, it can be:
+ Text: a basic text message (with its Entities for font effects, and LinkPreviewOptions for preview info)
+ Photo, Video, Animation (GIF), Document (file), Audio, Voice, PaidMedia: those are media contents which can come with a Caption subtext (and its CaptionEntities)
+ VideoNote, Sticker, Dice, Game, Poll, Venue, Location, Story: other kind of messages without a caption
- You can use methods message.ToHtml() or message.ToMarkdown() to convert the text/caption & entities into HTML (recommended) or Markdown.
3.1.2.2 Service message
- All other message types represent some action/status that happened in the chat instead of actual content.
- We are not listing all types here, but it could be for example:
+ members joined/left
+ pinned message
+ chat info/status/topic changed
+ payment/passport/giveaway process update
+ etc...
3.1.2.3 Common properties
- There are additional properties that gives you information about the context of the message.
- Here are a few important properties:
+ MessageId: the ID that you will use if you need to reply or call a method acting on this message
+ Chat: in which chat the message arrived
+ From: which user posted it
+ Date: timestamp of the message (in UTC)
+ ReplyToMessage: which message this is a reply to
+ ForwardOrigin: if it is a Forwarded message
+ MediaGroupId: albums (group of media) are separate consecutive messages having the same MediaGroupId
+ MessageThreadId: the topic ID for Forum/Topic type chats
3.1.3 Example project
