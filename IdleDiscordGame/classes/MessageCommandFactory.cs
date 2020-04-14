using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IdleDiscordGame.classes
{
    public static class MessageCommandFactory
    {
        public static async Task MessageCommand(SocketMessage message)
        {
            string[] msgBlocks = message.Content.ToLower().Split(" ");
            string msg0 = msgBlocks[0].Trim(new char[] { '!' });
            if (msgBlocks.Length > 0 && !message.Author.IsBot)
            {
                switch (msg0)
                {
                    case "create":
                        CreateCharacter(message);
                        break;
                    case "totalxp":
                        GetTotalXP(message);
                        break;
                    case "action":
                        ActionMessageReceived(message, msgBlocks);
                        break;
                    default:
                        break;
                }
            }
        }

        static async void ActionMessageReceived(SocketMessage message, string[] msgBlocks)
        {
            if (msgBlocks.Length > 1)
            {
                string msg1 = msgBlocks[1];
                switch (msg1)
                {

                }
            }
            else
            {
                await message.Channel.SendMessageAsync($"{message.Author.Username} which action would you like to perform?");
            }
        }

        static async void GetTotalXP(SocketMessage message)
        {
            if (Program.userIds.Contains(message.Author.Id))
            {
                if (Program.CharacterDict.TryGetValue(message.Author.Id, out Character character))
                {
                    await message.Channel.SendMessageAsync($"Your total XP: {character.TotalExperience()}");
                }
                else
                {
                    LoadCharacter(message, message.Author.Id);
                }
            }
            else
            {
                await message.Channel.SendMessageAsync($"Create your character now by using ``!create`` to start playing.");
            }
        }

        static async void LoadCharacter(SocketMessage message, ulong userId)
        {
            bool tryAddStatus = Program.CharacterDict.TryAdd(userId, new Character(userId));
            if (Program.CharacterDict.TryGetValue(userId, out Character character1))
            {
                await message.Channel.SendMessageAsync($"Your total XP: {character1.TotalExperience()}");
            }
            else
            {
                await message.Channel.SendMessageAsync($"Program.cs:MessageReceived:TotalXP:Error1.4");
            }
        }

        static async void  CreateCharacter(SocketMessage message)
        {
            // If they have already reacted a character, they cannot create another one.
            if (Program.userIds.Contains(message.Author.Id))
            {
                await message.Channel.SendMessageAsync("You've already started your character.");
            }
            // If they haven't created character:
            // - Create a Character.cs object
            // - add it to the CharacterDict
            // - Respond appropriately letting them know if they were created correctly
            else
            {
                bool success = Program.userIds.Add(message.Author.Id);
                if (success)
                {
                    Character newChar = new Character(message.Author.Id);
                    Program.CharacterDict.TryAdd(message.Author.Id, newChar);
                    await message.Channel.SendMessageAsync($"Character creation is a success!\nGood luck in your new adventure {message.Author.Username}!");
                    SerializeJsonObject($"json/userIds.json", Program.userIds);
                }
                else
                {
                    await message.Channel.SendMessageAsync($"There was a problem creating your character.\nPlease wait a bit and try again.");
                }
            }
        }

        private static void SerializeJsonObject(string filename, object value)
        {
            _ = Program.Log(new LogMessage(LogSeverity.Verbose, $"Program", $"SerializeJson"));
            using StreamWriter file = File.CreateText($"{filename}");
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, value);
        }
    }
}
