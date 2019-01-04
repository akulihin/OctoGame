using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using OctoGame.Helpers;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.GamePlayFramework;
using OctoGame.OctoGame.UpdateMessages;

namespace OctoGame.OctoGame.ReactionHandling
{
    public class OctoGameReaction
    {
        private readonly IUserAccounts _accounts;
        private readonly OctoGameUpdateMess _octoGameUpdateMess;
        private readonly Global _global;
        private readonly AwaitForUserMessage _awaitForUserMessage;
        private readonly GameFramework _gameFramework;


        public OctoGameReaction(IUserAccounts accounts, OctoGameUpdateMess octoGameUpdateMess,
            Global global,
            AwaitForUserMessage awaitForUserMessage, GameFramework gameFramework)
        {
            _accounts = accounts;
            _octoGameUpdateMess = octoGameUpdateMess;
            _global = global;
            _awaitForUserMessage = awaitForUserMessage;
            _gameFramework = gameFramework;
        }

        public async Task ReactionAddedForOctoGameAsync(Cacheable<IUserMessage, ulong> cash,
            ISocketMessageChannel channel, SocketReaction reaction)
        {
                for (var i = 0; i < _global.OctopusGameMessIdList.Count; i++)
                {

                    if(!_global.OctopusGameMessIdList[i].Any( x => x.PlayerDiscordAccount.Id == reaction.UserId && x.GamingWindowFromBot.Id == reaction.MessageId))
                        continue;

                    var globalAccount = _global.Client.GetUser(reaction.UserId);
                    var account = _accounts.GetAccount(globalAccount);
                    var enemy = _accounts.GetAccount(account.CurrentEnemy);

                    switch (reaction.Emote.Name)
                    {
                        case "🐙":

                            await _octoGameUpdateMess.MainPage(reaction.UserId,
                                reaction.Message.Value);
                            break;
                        case "⬅":
                            await _octoGameUpdateMess.SkillPageLeft(reaction,
                                reaction.Message.Value);
                            break;
                        case "➡":
                            await _octoGameUpdateMess.SkillPageRight(reaction,
                                reaction.Message.Value);
                            break;
                        case "📖":
                            //  await _octoGameUpdateMess.OctoGameLogs(reaction,
                            //       _global.OctopusGameMessIdList[i].bot_gaming_msg_1);
                            account.MoveListPage = 5;
                            await _octoGameUpdateMess.MainPage(reaction.UserId,
                                reaction.Message.Value);
                            break;
                        case "❌":
                            if (await _awaitForUserMessage.FinishTheGame(reaction))
                                await _octoGameUpdateMess.EndGame(reaction,
                                    reaction.Message.Value);
                            break;
                        case "1⃣":
                        {
                            if (account.PlayingStatus == 1)
                            {
                                await _octoGameUpdateMess.WaitMess(reaction.UserId,
                                    reaction.Message.Value);
                                break;
                            }

                            if (account.PlayingStatus == 2)
                                await _gameFramework.GetSkillDependingOnMoveList(account, enemy, reaction, i);

                            break;
                        }

                        case "2⃣":
                        {
                            if (account.PlayingStatus == 1)
                            {
                                await _octoGameUpdateMess.WaitMess(reaction.UserId,
                                    reaction.Message.Value);
                                break;
                            }

                            if (account.PlayingStatus == 2)
                                await _gameFramework.GetSkillDependingOnMoveList(account, enemy, reaction, i);
                            break;
                        }

                        case "3⃣":
                        {
                            if (account.PlayingStatus == 1)
                            {
                                await _octoGameUpdateMess.WaitMess(reaction.UserId,
                                    reaction.Message.Value);
                                break;
                            }

                            if (account.PlayingStatus == 2)
                                await _gameFramework.GetSkillDependingOnMoveList(account, enemy, reaction, i);

                            break;
                        }

                        case "4⃣":
                        {
                            await _gameFramework.GetSkillDependingOnMoveList(account, enemy, reaction, i);
                            break;
                        }

                        case "5⃣":
                        {
                            await _gameFramework.GetSkillDependingOnMoveList(account, enemy, reaction, i);
                            break;
                        }

                        case "6⃣":
                        {
                            await _gameFramework.GetSkillDependingOnMoveList(account, enemy, reaction, i);
                            break;
                        }

                        case "7⃣":
                        {
                            await _gameFramework.GetSkillDependingOnMoveList(account, enemy, reaction, i);
                            break;
                        }

                        case "8⃣":
                        {
                            await _gameFramework.GetSkillDependingOnMoveList(account, enemy, reaction, i);
                            break;
                        }

                        case "9⃣":
                        {
                            await _gameFramework.GetSkillDependingOnMoveList(account, enemy, reaction, i);
                            break;
                        }
                    }

                }


            if(!(channel is IDMChannel))
            await reaction.Message.Value.RemoveReactionAsync(reaction.Emote,
                reaction.User.Value, RequestOptions.Default);
        }
    }
}