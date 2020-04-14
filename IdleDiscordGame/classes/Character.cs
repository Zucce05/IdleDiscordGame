using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace IdleDiscordGame.classes
{
    public class Character
    {
        public Character() { }
        public Character(ulong playerId)
        {
            if (!TryLoad(playerId))
            {
                Stats = new CharacterStats
                {
                    CreatedAt = DateTimeOffset.UtcNow,
                    LastUpdatedDateTime = DateTimeOffset.UtcNow
                };
            }
            PlayerId = playerId;
            fileName = $"saves/{PlayerId}.json";
            EndEveryCall();
        }

        CharacterStats Stats { get; set; }
        readonly string fileName;
        public ulong PlayerId { get; }

        private bool TryLoad(ulong playerId)
        {
            bool status = false;
            try
            {
                JsonTextReader reader;
                // This is good for deployment where I've got the config with the executable
                reader = new JsonTextReader(new StreamReader($"saves/{playerId}.json"));
                Stats = JsonConvert.DeserializeObject<CharacterStats>(File.ReadAllText($"saves/{playerId}.json"));
                status = true;
            }
            catch (Exception e)
            {
                _ = Program.Log(new LogMessage(LogSeverity.Error, $"Character:Deserialize", $"Error reading json/CharacterStats.json", e));
            }
            return status;
        }

        public ulong TotalExperience()
        {
            StartEveryCall();
            EndEveryCall();
            return Stats.TotalExperience;
        }

        private void StartEveryCall()
        {
            DeserializeJsonObject();
            Stats.TotalExperience += ((ulong)(DateTimeOffset.UtcNow.ToUnixTimeSeconds()) - (ulong)(Stats.LastUpdatedDateTime.ToUnixTimeSeconds()));
            Stats.LastUpdatedDateTime = DateTimeOffset.UtcNow;
        }

        private void EndEveryCall()
        {
            SerializeJsonObject();
        }

        private void DeserializeJsonObject()
        {
            try
            {
                JsonTextReader reader;
                // This is good for deployment where I've got the config with the executable
                using StreamReader myReader = new StreamReader($"saves/{PlayerId}.json");
                reader = new JsonTextReader(myReader);
                Stats = JsonConvert.DeserializeObject<CharacterStats>(File.ReadAllText($"saves/{PlayerId}.json"));
            }
            catch (Exception e)
            {
                _ = Program.Log(new LogMessage(LogSeverity.Error, $"Character:Deserialize", $"Error reading json/userIds.json", e));
            }
        }

        private void SerializeJsonObject()
        {
            try
            {
                _ = Program.Log(new LogMessage(LogSeverity.Verbose, $"Program", $"SerializeJson"));
                using StringWriter _test = new StringWriter();
                using StreamWriter file = File.CreateText(fileName);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, Stats);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
