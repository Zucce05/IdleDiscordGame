using Discord;
using Discord.WebSocket;
using IdleDiscordGame.classes;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IdleDiscordGame
{
    class Program
    {
        // Create the client
        public static DiscordSocketClient client;

        // Use this class to configure settings on bot startup
        // Keep a private copy ignored by git to keep keys and secrets private
        BotConfig botConfig = new BotConfig();

        // Create a lock to be used with the log file
        public static object writeLock = new object();
        public static ConcurrentDictionary<ulong, Character> CharacterDict = new ConcurrentDictionary<ulong, Character>();
        public static HashSet<ulong> userIds = new HashSet<ulong>();

        public static void Main()
        => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            client = new DiscordSocketClient
            (new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            // Populate the configuration from BotConfig.json for the client to use
            BotConfiguration(ref botConfig);

            // Set the token to an easy to understand variable
            string botToken = botConfig.Token;



            // Events to be handled comment out what isn't needed, uncomment what you need
            //client.ChannelCreated += ChannelCreated;
            //client.ChannelDestroyed += ChannelDestroyed;
            //client.ChannelUpdated += ChannelUpdated;
            //client.Connected += Connected;
            //client.CurrentUserUpdated += CurrentUserUpdated;
            //client.Disconnected += Disconnected;
            //client.GuildAvailable += GuildAvailable;
            //client.GuildMembersDownloaded += GuildMembersDownloaded;
            //client.GuildMemberUpdated += GuildMemberUpdated;
            //client.GuildUnavailable += GuildUnavailable;
            //client.GuildUpdated += GuildUpdated;
            //client.JoinedGuild += JoinedGuild;
            //client.LatencyUpdated += LatencyUpdated;
            //client.LeftGuild += LeftGuild;
            //client.Log += Log;
            //client.LoggedIn += LoggedIn;
            //client.LoggedOut += LoggedOut;
            //client.MessageDeleted += MessageDeleted;
            client.MessageReceived += MessageReceived;
            //client.MessagesBulkDeleted += MessagesBulkDeleted;
            //client.MessageUpdated += MessageUpdated;
            //client.ReactionAdded += ReactionAdded;
            //client.ReactionRemoved += ReactionRemoved;
            //client.ReactionsCleared += ReactionsCleared;
            //client.Ready += Ready;
            //client.RecipientAdded += RecipientAdded;
            //client.RecipientRemoved += RecipientRemoved;
            //client.RoleCreated += RoleCreated;
            //client.RoleDeleted += RoleDeleted;
            //client.RoleUpdated += RoleUpdated;
            //client.UserBanned += UserBanned;
            //client.UserIsTyping += UserIsTyping;
            //client.UserJoined += UserJoined;
            //client.UserLeft += UserLeft;
            //client.UserUnbanned += UserUnbanned;
            //client.UserUpdated += UserUpdated;
            //client.UserVoiceStateUpdated += UserVoiceStateUpdated;
            //client.VoiceServerUpdated += VoiceServerUpdated;

            await client.LoginAsync(TokenType.Bot, botToken);
            await client.StartAsync();

            await Task.Delay(-1);

        }

        private async Task MessageReceived(SocketMessage message)
        {
            await MessageCommandFactory.MessageCommand(message);
        }

        public void SerializeJsonObject(string filename, object value)
        {
            _ = Log(new LogMessage(LogSeverity.Verbose, $"Program", $"SerializeJson"));
            using StreamWriter file = File.CreateText($"{filename}");
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, value);
        }

        public static void BotConfiguration(ref BotConfig bc)
        {
            JsonTextReader reader;
            try
            {
                // This is good for deployment where I've got the config with the executable
                reader = new JsonTextReader(new StreamReader("json/botConfig.json"));
                bc = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("json/botConfig.json"));
            }
            catch (Exception e)
            {
                _ = Log(new LogMessage(LogSeverity.Error, $"BotConfiguration", $"Error reading json/botConfig.json", e));
            }

            try
            {
                // This is good for deployment where I've got the config with the executable
                reader = new JsonTextReader(new StreamReader("json/userIds.json"));
                HashSet<ulong> tempHashSet = JsonConvert.DeserializeObject<HashSet<ulong>>(File.ReadAllText("json/userIds.json"));
                if (tempHashSet != null)
                {
                    userIds = tempHashSet;
                }
            }
            catch (Exception e)
            {
                _ = Log(new LogMessage(LogSeverity.Error, $"BotConfiguration", $"Error reading json/userIds.json", e));
            }
        }

        public static Task Log(LogMessage msg)
        {
            //if (!File.Exists("log.txt"))
            //{
            //    File.Create("log.txt");
            //}
            //lock (writeLock)
            //{
            //    StreamWriter w = File.AppendText("log.txt");
            //    w.WriteLineAsync($"Log Entry: {DateTime.Now.ToString()}");
            //    w.WriteLineAsync($"{msg}");
            //    w.WriteLineAsync("---------------");
            //    w.Close();
            //}
            ////Console.WriteLine(msg.ToString());
            Console.WriteLine($"Log Entry: {DateTime.Now}\n{msg}\n-----------------");
            return Task.CompletedTask;
        }
    }
}
