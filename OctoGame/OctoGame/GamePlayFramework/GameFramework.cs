using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using OctoGame.Helpers;
using OctoGame.LocalPersistentData.GameSpellsAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.SpellHandling.ActiveSkills;
using OctoGame.OctoGame.SpellHandling.BonusDmgHandling;
using OctoGame.OctoGame.SpellHandling.DmgReductionHandling;
using OctoGame.OctoGame.SpellHandling.PassiveSkills;
using OctoGame.OctoGame.UpdateMessages;

namespace OctoGame.OctoGame.GamePlayFramework
{
    public class GameFramework
    {
        private readonly IUserAccounts _accounts;
        private readonly ISpellAccounts _spellAccounts;
        private readonly OctoGameUpdateMess _octoGameUpdateMess;
        private readonly AttackDamageActiveTree _gameSpellHandling;
        private readonly AttackDamagePassiveTree _attackDamagePassiveTree;
        private readonly Global _global;
        private readonly AwaitForUserMessage _awaitForUserMessage;

        private readonly Crit _crit;
        private readonly Dodge _dodge;
        private readonly ArmorReduction _armorReduction;
        private readonly MagicReduction _magicReduction;


        public GameFramework(IUserAccounts accounts, OctoGameUpdateMess octoGameUpdateMess,
            AttackDamageActiveTree gameSpellHandling, ISpellAccounts spellAccounts, Global global,
            AwaitForUserMessage awaitForUserMessage, MagicReduction magicReduction, ArmorReduction armorReduction,
            Crit crit, Dodge dodge, AttackDamagePassiveTree attackDamagePassiveTree)
        {
            _accounts = accounts;
            _octoGameUpdateMess = octoGameUpdateMess;
            _gameSpellHandling = gameSpellHandling;
            _spellAccounts = spellAccounts;
            _global = global;
            _awaitForUserMessage = awaitForUserMessage;
            _magicReduction = magicReduction;
            _armorReduction = armorReduction;
            _crit = crit;
            _dodge = dodge;
            _attackDamagePassiveTree = attackDamagePassiveTree;
        }


        public async Task GetSkillDependingOnMoveList(AccountSettings account, AccountSettings enemy,
            SocketReaction reaction, int i)
        {
            var skills = GetSkillListFromTree(account);
            var ski = Convert.ToUInt64(skills[GetSkillNum(reaction) - 1]);
            var skill = _spellAccounts.GetAccount(ski);

            if (account.SkillCooldowns.Any(x => x.skillId == skill.SpellId))
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _awaitForUserMessage.ReplyAndDeleteOvertime("this skill is on cooldown, use another one", 6,
                    reaction);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                return;
            }

            Console.WriteLine($"{skill.SpellNameEn} + {skill.SpellId}");

            var dmg = _gameSpellHandling.AttackDamageActiveSkills(skill.SpellId, account, enemy, false);

            if (account.IsCrit)
                dmg = _crit.CritHandling(account.AG_Stats,
                    dmg, account);


            dmg = _dodge.DodgeHandling(account.AG_Stats, dmg,
                account, enemy);


            await DmgHealthHandeling(skill.WhereDmg, dmg, skill.SpellDmgType, account, enemy);
            await UpdateTurn(account, enemy);
        }

        public async Task UpdateTurn(AccountSettings account, AccountSettings enemy)
        {
            if (account.Buff.Count > 0)
                for (var i = 0; i < account.Buff.Count; i++)
                {
                    account.Buff[i].cooldown--;
                    if (account.Buff[i].cooldown <= 0)
                    {
                        account.Buff.RemoveAt(i);
                        _accounts.SaveAccounts(account.DiscordId);
                    }
                }

            await CheckDebuffs(enemy);

            if (account.SkillCooldowns != null)
                for (var i = 0; i < account.SkillCooldowns.Count; i++)
                {
                    account.SkillCooldowns[i].cooldown--;
                    if (account.SkillCooldowns[i].cooldown <= 0)
                        account.SkillCooldowns.RemoveAt(i);
                }

            account.Turn = 1;
            enemy.Turn = 0;
            _accounts.SaveAccounts(account.DiscordId);

            await CheckDmgWithTimer(account, enemy);
            await CheckDmgWithTimer(enemy, account);
            await CheckStatsForTime(account, enemy);

            await CheckForBuffsOrDebuffsBeforeTurn(account, enemy);
        }


        public async Task DmgHealthHandeling(int dmgWhere, double dmg, int dmgType, AccountSettings myAccount,
            AccountSettings enemyAccount)
        {
            /*
             0 = Regular
             1 = To health
             2 = only to stamina
             */
            // type 0 = physic, 1 = magic

            switch (dmgType)
            {
                case 0:
                    dmg = _armorReduction.ArmorHandling(myAccount.ArmPen, enemyAccount.Armor, dmg);
                    myAccount.Health += dmg * myAccount.LifeStealPrec;
                    // this is lifeSteam handleing as wel, we might change it demending on the mechanic of the game
                    break;
                case 1:
                    dmg = _magicReduction.ResistHandling(myAccount.MagPen, enemyAccount.Resist, dmg);
                    myAccount.Health += dmg * myAccount.LifeStealPrec;
                    break;
            }


            var status = 0;
            var userId = myAccount.DiscordId;
            switch (dmgWhere)
            {
                case 0 when enemyAccount.Stamina > 0:

                    if (dmgType == 0)
                    {
                        dmg = dmg - myAccount.PhysShield;

                        if (dmg < 0)
                            dmg = 0;
                    }

                    enemyAccount.Stamina -= Math.Ceiling(dmg);
                    _accounts.SaveAccounts(userId);

                    if (enemyAccount.Stamina < 0)
                    {
                        enemyAccount.Health += enemyAccount.Stamina;
                        enemyAccount.Stamina = 0;
                        _accounts.SaveAccounts(userId);
                    }

                    break;
                case 0:

                    if (dmgType == 0)
                    {
                        dmg = dmg - myAccount.PhysShield;

                        if (dmg < 0)
                            dmg = 0;
                    }

                    if (enemyAccount.Stamina <= 0)
                    {
                        enemyAccount.Health -= Math.Ceiling(dmg);
                        _accounts.SaveAccounts(userId);
                    }

                    break;
                case 1:
                    enemyAccount.Health -= Math.Ceiling(dmg);
                    _accounts.SaveAccounts(userId);
                    break;
                case 2:
                    enemyAccount.Stamina -= Math.Ceiling(dmg);
                    if (enemyAccount.Stamina < 0)
                        enemyAccount.Stamina = 0;
                    _accounts.SaveAccounts(userId);
                    break;
            }

            if (enemyAccount.Health <= 0)
            {
                enemyAccount.Health = 0;
                status = 1;
            }


            await UpdateIfWinOrContinue(status, myAccount.DiscordId, myAccount.MessageIdInList);
        }


        public async Task CheckDmgWithTimer(AccountSettings account, AccountSettings enemy)
        {
            for (var index = 0; index < account.DamageOnTimer.Count; index++)
            {
                var t = account.DamageOnTimer[index];
                t.timer--;
                if (t.timer <= 0)
                {
                    await DmgHealthHandeling(0, t.dmg, t.dmgType, enemy, account);
                    account.DamageOnTimer.RemoveAt(index);
                    _accounts.SaveAccounts(account.DiscordId);
                }
            }

            await Task.CompletedTask;
        }

        public async Task CheckStatsForTime(AccountSettings account, AccountSettings enemy)
        {
            for (var i = 0; i < account.StatsForTime.Count; i++)
            {
                var t = account.StatsForTime[i];
                t.timer--;

                if (t.timer <= 0) account.AD_Stats -= t.AD_STATS;
            }

            await Task.CompletedTask;
        }

        //move debufs somewhere else
        public async Task CheckDebuffs(AccountSettings account)
        {
            if (account.Debuff.Count > 0)
                for (var i = 0; i < account.Debuff.Count; i++)
                {
                    account.Debuff[i].cooldown--;


                    switch (account.Debuff[i].skillId)
                    {
                        case 1003:
                            if (!account.Debuff[i].activated)
                                account.Armor -= 2;
                            if (account.Debuff[i].cooldown <= 0)
                                account.Armor += 2;
                            break;
                    }

                    account.Debuff[i].activated = true;
                    if (account.Debuff[i].cooldown <= 0)
                    {
                        account.Debuff.RemoveAt(i);
                        _accounts.SaveAccounts(account.DiscordId);
                    }
                }

            await Task.CompletedTask;
        }

        public async Task CheckForBuffsOrDebuffsBeforeTurn(AccountSettings account, AccountSettings enemy)
        {
            for (var i = 0; i < account.Buff.Count; i++)
            {
                if (account.SkillCooldowns.Any(x => x.skillId == account.Buff[i].skillId))
                    continue;

                _attackDamagePassiveTree.AttackDamagePassiveSkills(account, enemy, i);

            }

            account.Base_AD_Stats = Math.Ceiling(account.Strength * (0.2 * account.OctoLvL)); // + ITEMS + SKILLS

            account.AD_Stats = account.Base_AD_Stats + account.Bonus_AD_Stats;
            account.Bonus_AD_Stats = 0;

            account.IsCrit = false;

            _accounts.SaveAccounts(account.DiscordId);
            _accounts.SaveAccounts(enemy.DiscordId);
            await Task.CompletedTask;
        }

        public async Task UpdateIfWinOrContinue(int status, ulong userId, int i)
        {
            if (status == 1)
                await _octoGameUpdateMess.VictoryPage(userId,
                    _global.OctopusGameMessIdList[i].SocketMsg);
            else
                await _octoGameUpdateMess.MainPage(userId,
                    _global.OctopusGameMessIdList[i].SocketMsg);
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