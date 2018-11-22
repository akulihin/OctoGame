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
            if (cash.Value.Channel is IGuildChannel)
                for (var i = 0; i < _global.OctopusGameMessIdList.Count; i++)
                {
                    if (reaction.MessageId != _global.OctopusGameMessIdList[i].OctoGameMessIdToTrack ||
                        reaction.UserId != _global.OctopusGameMessIdList[i].OctoGameUserIdToTrack) continue;
                    var globalAccount = _global.Client.GetUser(reaction.UserId);
                    var account = _accounts.GetAccount(globalAccount);
                    var enemy = _accounts.GetAccount(account.CurrentEnemy);

                    switch (reaction.Emote.Name)
                    {
                        case "🐙":

                            await _octoGameUpdateMess.MainPage(reaction.UserId,
                                _global.OctopusGameMessIdList[i].SocketMsg);
                            break;
                        case "⬅":
                            await _octoGameUpdateMess.SkillPageLeft(reaction,
                                _global.OctopusGameMessIdList[i].SocketMsg);
                            break;
                        case "➡":
                            await _octoGameUpdateMess.SkillPageRight(reaction,
                                _global.OctopusGameMessIdList[i].SocketMsg);
                            break;
                        case "📖":
                            //  await _octoGameUpdateMess.OctoGameLogs(reaction,
                            //       _global.OctopusGameMessIdList[i].SocketMsg);
                            account.MoveListPage = 5;
                            await _octoGameUpdateMess.MainPage(reaction.UserId,
                                _global.OctopusGameMessIdList[i].SocketMsg);
                            break;
                        case "❌":
                            if (await _awaitForUserMessage.FinishTheGame(reaction))
                                await _octoGameUpdateMess.EndGame(reaction,
                                    _global.OctopusGameMessIdList[i].SocketMsg);
                            break;
                        case "1⃣":
                        {
                            if (account.PlayingStatus == 1)
                            {
                                await _octoGameUpdateMess.WaitMess(reaction,
                                    _global.OctopusGameMessIdList[i].SocketMsg);
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
                                await _octoGameUpdateMess.WaitMess(reaction,
                                    _global.OctopusGameMessIdList[i].SocketMsg);
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
                                await _octoGameUpdateMess.WaitMess(reaction,
                                    _global.OctopusGameMessIdList[i].SocketMsg);
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
                    }

                    await _global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote,
                        _global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                }
        }
    }
}