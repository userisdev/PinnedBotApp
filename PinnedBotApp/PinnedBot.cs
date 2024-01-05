using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PinnedBotApp
{
    /// <summary> PinnedBot class. </summary>
    internal sealed class PinnedBot
    {
        /// <summary> The client </summary>
        private readonly DiscordSocketClient client;

        /// <summary> The token </summary>
        private readonly string token;

        /// <summary> Initializes a new instance of the <see cref="PinnedBot" /> class. </summary>
        /// <param name="token"> The token. </param>
        public PinnedBot(string token)
        {
            this.token = token;

            client = new DiscordSocketClient();

            client.Log += OnLog;
            client.Ready += OnReady;

            client.ReactionAdded += OnReactionAddedAsync;
            client.ReactionRemoved += OnReactionRemovedAsync;
        }

        /// <summary> Runs the asynchronous. </summary>
        public async Task RunAsync()
        {
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        /// <summary> Called when [log]. </summary>
        /// <param name="log"> The log. </param>
        /// <returns> </returns>
        private Task OnLog(LogMessage log)
        {
            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : {log.Message}");
            Trace.Flush();
            return Task.CompletedTask;
        }

        /// <summary> Called when [reaction added asynchronous]. </summary>
        /// <param name="cachedMessage"> The cached message. </param>
        /// <param name="cachedChannel"> The cached channel. </param>
        /// <param name="reaction"> The reaction. </param>
        private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            IUserMessage message = await cachedMessage.GetOrDownloadAsync();
            if (message is null)
            {
                return;
            }

            // 強制削除機能
            if (reaction.Emote.Name == "🔨")
            {
                Emoji[] emojis = new[] { new Emoji("📌"), new Emoji("🔨") };
                foreach (Emoji emoji in emojis)
                {
                    IEnumerable<IUser> users = await message.GetReactionUsersAsync(emoji, int.MaxValue).FlattenAsync();
                    foreach (IUser user in users)
                    {
                        await message.RemoveReactionAsync(emoji, user);
                        Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : removed {emoji.Name}/{user.Username}");
                        Trace.Flush();
                    }
                }

                await message.UnpinAsync();
                Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : unpinned {message.Id}");
                Trace.Flush();
                return;
            }

            // リアクションが特定の絵文字であり、特定のメッセージに付いたものであればピン留めを行う
            if (reaction.Emote.Name == "📌")
            {
                if (!message.IsPinned)
                {
                    await message.PinAsync();
                    Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : pinned {message.Id}");
                    Trace.Flush();
                }

                return;
            }
        }

        /// <summary> Called when [reaction removed asynchronous]. </summary>
        /// <param name="cachedMessage"> The cached message. </param>
        /// <param name="cachedChannel"> The cached channel. </param>
        /// <param name="reaction"> The reaction. </param>
        private async Task OnReactionRemovedAsync(Cacheable<IUserMessage, ulong> cachedMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            IUserMessage message = await cachedMessage.GetOrDownloadAsync();
            if (message is null)
            {
                return;
            }

            // リアクションが特定の絵文字であり、特定のメッセージに付いたものであればピン留めを解除する
            if (reaction.Emote.Name == "📌")
            {
                IEnumerable<IUser> users = await message.GetReactionUsersAsync(reaction.Emote, int.MaxValue).FlattenAsync();
                foreach (IUser user in users)
                {
                    await message.RemoveReactionAsync(reaction.Emote, user);
                    Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : removed {reaction.Emote.Name}/{user.Username}");
                    Trace.Flush();
                }

                if (message.IsPinned)
                {
                    await message.UnpinAsync();
                    Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : unpinned {message.Id}");
                    Trace.Flush();
                }

                return;
            }
        }

        /// <summary> Called when [ready]. </summary>
        /// <returns> </returns>
        private Task OnReady()
        {
            Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss.fff} : is Running!!");
            Trace.Flush();
            return Task.CompletedTask;
        }
    }
}
