using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using OctoGame.Accounts.GameSpells;
using OctoGame.Accounts.Users;
using OctoGame.Helpers;
using OctoGame.OctoGame.UpdateMessages;

namespace OctoGame.OctoGame.ReactionHandeling
{
    public class OctoGameReaction
    {
        private readonly IUserAccounts _accounts;
        private readonly ISpellAccounts _spellAccounts;
        private readonly OctoGameUpdateMess _octoGameUpdateMess;
        private readonly GameSpellHandeling _gameSpellHandeling;
        private readonly Global _global;
        private readonly AwaitForUserMessage _awaitForUserMessage;

        public OctoGameReaction(IUserAccounts accounts, OctoGameUpdateMess octoGameUpdateMess,
            GameSpellHandeling gameSpellHandeling, ISpellAccounts spellAccounts, Global global, AwaitForUserMessage awaitForUserMessage)
        {
            _accounts = accounts;
            _octoGameUpdateMess = octoGameUpdateMess;
            _gameSpellHandeling = gameSpellHandeling;
            _spellAccounts = spellAccounts;
            _global = global;
            _awaitForUserMessage = awaitForUserMessage;
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
                    var enemy = _accounts.GetBotAccount(account.CurrentEnemy);

                    switch (reaction.Emote.Name)
                    {
                        case "🐙":

                            await _octoGameUpdateMess.MainPage(reaction,
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
                            await _octoGameUpdateMess.MainPage(reaction, _global.OctopusGameMessIdList[i].SocketMsg);
                            break;
                        case "❌":
                            if(await _awaitForUserMessage.FinishTheGame(reaction))
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
                                await GetSkillDependingOnMoveList(account, enemy, reaction, i);

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
                               await GetSkillDependingOnMoveList(account, enemy, reaction, i);
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
                                await GetSkillDependingOnMoveList(account, enemy, reaction, i);

                            break;
                        }

                        case "4⃣":
                        {
                            await GetSkillDependingOnMoveList(account, enemy, reaction, i);
                                break;
                        }

                        case "5⃣":
                        {
                            await GetSkillDependingOnMoveList(account, enemy, reaction, i);
                                break;
                        }

                        case "6⃣":
                        {
                            await GetSkillDependingOnMoveList(account, enemy, reaction, i);
                                break;
                        }

                        case "7⃣":
                        {
                            await GetSkillDependingOnMoveList(account, enemy, reaction, i);
                                break;
                        }

                        case "8⃣":
                        {
                            await GetSkillDependingOnMoveList(account, enemy, reaction, i);
                                break;
                        }
                    }
                    await _global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote,
                        _global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                }


        } 

        public async Task GetSkillDependingOnMoveList(AccountSettings account, AccountSettings enemy,
            SocketReaction reaction, int i)
        {
            var skills = GetSkillListFromTree(account);
            var ski = Convert.ToUInt64(skills[GetSkillNum(reaction) - 1]);
            var skill = _spellAccounts.GetAccount(ski);

            if (account.SkillCooldowns.Any(x => x.skillId == skill.SpellId))
            {
                var hm =  _awaitForUserMessage.ReplyAndDeleteOvertime("this skill is on cooldown, use another one", 6, reaction);
                return;
            }

            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

            var dmg = _gameSpellHandeling.AdSkills(skill.SpellId, account, enemy, false);

            if (account.IsCrit == 0)
            {
                dmg = _gameSpellHandeling.CritHandeling(account.AG_Stats,
                    dmg, account);
            }
            else
            {
                account.IsCrit = 0;
                _accounts.SaveAccounts(account.Id);
            }

            dmg = _gameSpellHandeling.DodgeHandeling(account.AG_Stats, dmg,
                account, enemy);


            var status = _gameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account, enemy);
            await UpdateIfWinOrContinue(status, reaction, i);
            await UpdateTurn(account, enemy);
        }

        public async Task UpdateTurn(AccountSettings account, AccountSettings enemy)
        {
            if(account.Buff != null)
                if (account.Buff.Count > 0)
            {
                for (var i = 0; i < account.Buff.Count; i++)
                {
                    account.Buff[i].cooldown--;
                    if (account.Buff[i].cooldown <= 0)
                    {
                        account.Buff.RemoveAt(i);
                        _accounts.SaveAccounts(account.Id);
                    }
                }
            }

            if (account.Debuff != null)
                if (account.Debuff.Count > 0)
            {
                for (var i = 0; i < account.Debuff.Count; i++)
                {
                    account.Debuff[i].cooldown--;
                    if (account.Debuff[i].cooldown <= 0)
                    {
                        account.Debuff.RemoveAt(i);
                        _accounts.SaveAccounts(account.Id);
                    }
                }
            }

            if (account.SkillCooldowns != null)
            {
                for (var i = 0; i < account.SkillCooldowns.Count; i++)
                {
                    account.SkillCooldowns[i].cooldown--;
                    if (account.SkillCooldowns[i].cooldown <= 0)
                        account.SkillCooldowns.RemoveAt(i);
                }
            }

            account.Turn = 1;
            enemy.Turn = 0;
            _accounts.SaveAccounts(account.Id);

            await CheckForBuffsOrDebuffsBeforeTurn(account, enemy);
        }

        public async Task CheckForBuffsOrDebuffsBeforeTurn(AccountSettings account, AccountSettings enemy)
        {
            if (account.Buff != null)
                if (account.Buff.Count > 0)
            {
                for (var i = 0; i < account.Buff.Count; i++)
                {
                    if(account.SkillCooldowns.Any(x => x.skillId == account.Buff[i].skillId))
                        continue;
                        

                    switch (account.Buff[i].skillId)
                    {
                        // (ад ветка) Выжидание - пассивно первая атака за бой будет усилена на 20% ||
                          case 1000: 
                              account.PrecentBonusDmg = 0.2;
                              break;

                        // (ад ветка) Меткость - пассивно если враг увернлся - то следующий ход он не может увернутся. (кд 8 ходов) 
                          case 1002 when enemy.Dodged >= 1: 
                              enemy.DodgeChance = 0;
                              if (account.SkillCooldowns == null)
                                  account.SkillCooldowns = new List<AccountSettings.Cooldown>();
                              account.SkillCooldowns.Add(new AccountSettings.Cooldown(1002, 8));
                              break;      
                       
                        //пассивно увеличивает ад на 10% от вражеской текущей выносливости. 
                        case  1004:
                            account.AD_Stats += account.AD_Stats * 0.1 * enemy.Stamina;
                            break;

                        //(ад ветка) Без изъяна - пассивно дает 1 армор или резист, если на него не куплено ни одной вещи. 1006
                        case 1006 when account.OctoItems.Any( x => x.Armor >= 1) && account.OctoItems.Any( x => x.Resist >= 1):         
                            account.Armor++;
                            account.Resist++;
                            break;
                    }
                    _accounts.SaveAccounts(account.Id);
                    _accounts.SaveAccounts(enemy.Id);
                }
            }

            await Task.CompletedTask;
        }

        public async Task UpdateIfWinOrContinue(int status, SocketReaction reaction, int i)
        {
            if (status == 1)
            {
                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                await _octoGameUpdateMess.MainPage(reaction,
                    _global.OctopusGameMessIdList[i].SocketMsg);
            }
            else
            {
                await _octoGameUpdateMess.MainPage(reaction,
                    _global.OctopusGameMessIdList[i].SocketMsg);
            }
        }

        public string[] GetSkillListFromTree(AccountSettings account)
        {
            string[] skills = { };

            if (account.MoveListPage == 1)
                skills = account.AD_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
            else if (account.MoveListPage == 2)
                skills = account.DEF_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
            else if (account.MoveListPage == 3)
                skills = account.AG_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
            else if (account.MoveListPage == 4)
                skills = account.AP_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);

            return skills;
        }

        public int GetSkillNum(SocketReaction reaction)
        {
            var value = 0;
            switch (reaction.Emote.Name)
            {
                case "1⃣":
                {
                    value = 1;
                    break;
                }

                case "2⃣":
                {
                    value = 2;
                    break;
                }

                case "3⃣":
                {
                    value = 3;
                    break;
                }

                case "4⃣":
                {
                    value = 4;
                    break;
                }

                case "5⃣":
                {
                    value = 5;
                    break;
                }

                case "6⃣":
                {
                    value = 6;
                    break;
                }

                case "7⃣":
                {
                    value = 6;
                    break;
                }

                case "8⃣":
                {
                    value = 7;
                    break;
                }
            }

            return value;
        }
    }
}