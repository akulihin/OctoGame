using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using OctoBot.Configs;
using OctoBot.Games.OctoGame.GameSpells;
using OctoBot.Games.OctoGame.GameUsers;

namespace OctoBot.Games.OctoGame
{
    internal static class OctoGameReaction
    {


        public static async Task ReactionAddedForOctoGameAsync(Cacheable<IUserMessage, ulong> cash,
            ISocketMessageChannel channel, SocketReaction reaction)
        {

            for (var i = 0; i < Global.OctopusGameMessIdList.Count ; i++)
            {
                if (reaction.MessageId != Global.OctopusGameMessIdList[i].OctoGameMessIdToTrack ||
                    reaction.UserId != Global.OctopusGameMessIdList[i].OctoGameUserIdToTrack) continue;
                var globalAccount = Global.Client.GetUser(reaction.UserId);
                var account = GameUserAccounts.GetAccount(globalAccount);

                switch (reaction.Emote.Name)
                {
                    case "🐙":
                        await OctoGameUpdateMess.FighhtReaction.MainPage(reaction,
                            Global.OctopusGameMessIdList[i].SocketMsg);
                        await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                        break;
                    case "⬅":
                        await OctoGameUpdateMess.FighhtReaction.SkillPageLeft(reaction,
                            Global.OctopusGameMessIdList[i].SocketMsg);
                        await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                        break;
                    case "➡":
                        await OctoGameUpdateMess.FighhtReaction.SkillPageRight(reaction,
                            Global.OctopusGameMessIdList[i].SocketMsg);
                        await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                        break;
                    case "📖":
                        await OctoGameUpdateMess.FighhtReaction.OctoGameLogs(reaction,
                            Global.OctopusGameMessIdList[i].SocketMsg);
                        await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                        break;
                    case "❌":
                        await OctoGameUpdateMess.FighhtReaction.EndGame(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                        break;
                    case "1⃣":
                        {
                            //////////////////////////////////////////////////////////AD SKills////////////////////////////////////////////////////////////////////////////////
                            await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                            if (account.OctopusFightPlayingStatus == 1)
                            {
                                account.CurrentEnemyLvl = account.CurrentOctopusFighterLvl - 1;
                                account.CurrentEnemyStrength = 20;
                                account.CurrentEnemyAd = 0;
                                account.CurrentEnemyAp = 0;
                                account.CurrentEnemyAgility = 0;
                                account.CurrentEnemyDodge = 0;
                                account.CurrentEnemyCritDmg = 1.5;
                                account.CurrentEnemyArmor = 1;
                                account.CurrentEnemyMagicResist = 1;
                                account.CurrentEnemyHealth = 100;
                                account.CurrentEnemyStamina = 200;


                                GameUserAccounts.SaveAccounts();
                                await OctoGameUpdateMess.FighhtReaction.WaitMess(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                            }

                            if (account.OctopusFightPlayingStatus == 2)
                            {
                                string[] skills;

                                double dmg;


                                switch (account.MoveListPage)
                                {
                                    case 1:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[0]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);

                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }

                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);


                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 2:
                                        {

                                            skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] { '|' },
                                                StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[0]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 3:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[0]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 4:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[0]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }


                                }
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            }

                            break;
                        }

                    case "2⃣":
                        {
                            await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                            if (account.OctopusFightPlayingStatus == 1)
                            {
                                account.CurrentEnemyLvl = account.CurrentOctopusFighterLvl;
                                account.CurrentEnemyStrength = 30;
                                account.CurrentEnemyAd = 0;
                                account.CurrentEnemyAp = 0;
                                account.CurrentEnemyAgility = 0;
                                account.CurrentEnemyDodge = 0;
                                account.CurrentEnemyCritDmg = 1.5;
                                account.CurrentEnemyArmor = 1;
                                account.CurrentEnemyMagicResist = 1;
                                account.CurrentEnemyHealth = 100;
                                account.CurrentEnemyStamina = 300;

                                GameUserAccounts.SaveAccounts();
                                await OctoGameUpdateMess.FighhtReaction.WaitMess(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                            }
                            if (account.OctopusFightPlayingStatus == 2)
                            {
                                string[] skills;

                                double dmg;


                                switch (account.MoveListPage)
                                {
                                    case 1:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[1]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 2:
                                        {

                                            skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] { '|' },
                                                StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[1]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");


                                            dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);


                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 3:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[1]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 4:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[1]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                }
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            }

                            break;
                        }

                    case "3⃣":
                        {
                            await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);


                            if (account.OctopusFightPlayingStatus == 1)
                            {
                                account.CurrentEnemyLvl = account.CurrentOctopusFighterLvl + 1;
                                account.CurrentEnemyStrength = 40;
                                account.CurrentEnemyAd = 0;
                                account.CurrentEnemyAp = 0;
                                account.CurrentEnemyAgility = 0;
                                account.CurrentEnemyDodge = 0;
                                account.CurrentEnemyCritDmg = 1.5;
                                account.CurrentEnemyArmor = 1;
                                account.CurrentEnemyMagicResist = 1;
                                account.CurrentEnemyHealth = 100;
                                account.CurrentEnemyStamina = 400;


                                GameUserAccounts.SaveAccounts();
                                await OctoGameUpdateMess.FighhtReaction.WaitMess(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                            }

                            if (account.OctopusFightPlayingStatus == 2)
                            {
                                string[] skills;

                                double dmg;


                                switch (account.MoveListPage)
                                {
                                    case 1:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[2]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 2:
                                        {

                                            skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] { '|' },
                                                StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[2]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 3:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[2]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 4:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[2]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                }
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            }

                            break;
                        }

                    case "4⃣":
                        {
                            await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                            if (account.OctopusFightPlayingStatus == 2)
                            {
                                string[] skills;

                                double dmg;


                                switch (account.MoveListPage)
                                {
                                    case 1:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[3]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 2:
                                        {

                                            skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] { '|' },
                                                StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[3]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 3:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[3]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 4:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[3]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                }
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            }

                            break;
                        }

                    case "5⃣":
                        {
                            await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                            if (account.OctopusFightPlayingStatus == 2)
                            {
                                string[] skills;

                                double dmg;


                                switch (account.MoveListPage)
                                {
                                    case 1:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[4]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 2:
                                        {

                                            skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] { '|' },
                                                StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[4]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 3:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[4]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 4:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[4]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                }
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            }

                            break;
                        }

                    case "6⃣":
                        {
                            await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                            if (account.OctopusFightPlayingStatus == 2)
                            {
                                string[] skills;

                                double dmg;


                                switch (account.MoveListPage)
                                {
                                    case 1:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[5]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 2:
                                        {

                                            skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] { '|' },
                                                StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[5]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 3:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[5]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 4:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[5]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                }
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            }

                            break;
                        }

                    case "7⃣":
                        {
                            await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                            if (account.OctopusFightPlayingStatus == 2)
                            {
                                string[] skills;

                                double dmg;


                                switch (account.MoveListPage)
                                {
                                    case 1:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[6]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 2:
                                        {

                                            skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] { '|' },
                                                StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[6]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 3:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[6]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 4:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[6]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                }
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            }

                            break;
                        }

                    case "8⃣":
                        {
                            await Global.OctopusGameMessIdList[i].SocketMsg.RemoveReactionAsync(reaction.Emote, Global.OctopusGameMessIdList[i].Iuser, RequestOptions.Default);
                            if (account.OctopusFightPlayingStatus == 2)
                            {
                                string[] skills;

                                double dmg;


                                switch (account.MoveListPage)
                                {
                                    case 1:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[7]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 2:
                                        {

                                            skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] { '|' },
                                                StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[7]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 3:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[7]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                    case 4:
                                        {
                                            skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                                            var ski = Convert.ToUInt64(skills[7]);
                                            var skill = SpellUserAccounts.GetAccount(ski);

                                            Console.WriteLine($"{skill.SpellName} + {skill.SpellId}");

                                            dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                                            if (account.CurrentOctopusAbilityToCrit == 0)
                                            {
                                                dmg = GameSpellHandeling.CritHandeling(account.CurrentOctopusFighterAgility, dmg, account);
                                            }
                                            else
                                            {
                                                account.CurrentOctopusAbilityToCrit = 0;
                                                GameUserAccounts.SaveAccounts();
                                            }
                                            dmg = GameSpellHandeling.DodgeHandeling(account.CurrentOctopusFighterAgility, dmg, account);

                                            var status = GameSpellHandeling.DmgHealthHandeling(skill.WhereDmg, dmg, account);
                                            if (status == 1)
                                            {
                                                await reaction.Channel.SendMessageAsync("Буль, ты победил!");
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            }
                                            else
                                                await OctoGameUpdateMess.FighhtReaction.MainPage(reaction, Global.OctopusGameMessIdList[i].SocketMsg);
                                            break;
                                        }

                                }
                                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            }

                            break;
                        }
                    default:
                        return;
                }
            }
        }


    }
}
