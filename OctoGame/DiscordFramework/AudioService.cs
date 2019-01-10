using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

namespace OctoGame.DiscordFramework
{
    public sealed class AudioService : IService
    {

        public Task InitializeAsync()
            => Task.CompletedTask;

        private readonly ConcurrentDictionary<ulong, IAudioClient> _connectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            if (_connectedChannels.TryGetValue(guild.Id, out client))
            {
                return;
            }
            if (target.Guild.Id != guild.Id)
            {
                return;
            }

            var audioClient = await target.ConnectAsync();

            if (_connectedChannels.TryAdd(guild.Id, audioClient))
            {
                // If you add a method to log happenings from this service,
                // you can uncomment these commented lines to make use of that.
                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
            }
        }

        public async Task LeaveAudio(IGuild guild)
        {
            if (_connectedChannels.TryRemove(guild.Id, out var client))
            {
                await client.StopAsync();
                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
            }
        }
    
        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
                // Your task: Get a full path to the file if the value of 'path' is only a filename.
                path = @"gs.mp3"; //this is hardcoded example to check if it's working. This file have to exist in `bin\Debug\netcoreapp2.1` if you use windows
                if (!File.Exists(path))
                {
                    await channel.SendMessageAsync("File does not exist."); 
                    return;
                }

            if (_connectedChannels.TryGetValue(guild.Id, out var client))
                {
                    var ffmpeg = CreateStream(path);
                    var output = ffmpeg.StandardOutput.BaseStream;
                    var discord = client.CreatePCMStream(AudioApplication.Music, 96000);
                    await output.CopyToAsync(discord);
                    await discord.FlushAsync();
                }
        }

        private Process CreateStream(string path)
        {
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            return Process.Start(ffmpeg);
        }
    }
}