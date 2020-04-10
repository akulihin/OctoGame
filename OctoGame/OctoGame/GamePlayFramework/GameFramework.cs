using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using OctoGame.Helpers;
using OctoGame.LocalPersistentData.GameSpellsAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.SpellHandling.ActiveSkills;
using OctoGame.OctoGame.SpellHandling.Buffs;
using OctoGame.OctoGame.SpellHandling.PassiveSkills;

namespace OctoGame.OctoGame.GamePlayFramework
{
    public sealed class GameFramework : IServiceSingleton
    {

        public Task InitializeAsync()
            => Task.CompletedTask;

        private readonly UserAccounts _accounts;
        private readonly SpellUserAccounts _spellAccounts;

        private readonly AttackDamageActiveTree _attackDamageActiveTree;
        private readonly AttackDamagePassiveTree _attackDamagePassiveTree;
        private readonly DefenceActiveTree _defenceActiveTree;
        private readonly DefencePassiveTree _defencePassiveTree;
        private readonly AgilityActiveTree _agilityActiveTree;
        private readonly AgilityPassiveTree _agilityPassiveTree;
        private readonly MagicActiveTree _magicActiveTree;
        private readonly MagicPassiveTree _magicPassiveTree;

        private readonly AwaitForUserMessage _awaitForUserMessage;
        private readonly AllBuffs _allDebuffs;

        private readonly DealDmgToEnemy _dealDmgToEnemy;
        private readonly UpdateFightPage _updateFightPage;

        public GameFramework(UserAccounts accounts, 
            AttackDamageActiveTree attackDamageActiveTree, SpellUserAccounts spellAccounts, 
            AwaitForUserMessage awaitForUserMessage,
             AttackDamagePassiveTree attackDamagePassiveTree, AllBuffs allDebuffs, DefencePassiveTree defencePassiveTree, DefenceActiveTree defenceActiveTree, AgilityActiveTree agilityActiveTree, AgilityPassiveTree agilityPassiveTree, MagicActiveTree magicActiveTree, MagicPassiveTree magicPassiveTree, DealDmgToEnemy dealDmgToEnemy, UpdateFightPage updateFightPage)
        {
            _accounts = accounts;

            _attackDamageActiveTree = attackDamageActiveTree;
            _spellAccounts = spellAccounts;
  
            _awaitForUserMessage = awaitForUserMessage;
            _attackDamagePassiveTree = attackDamagePassiveTree;
            _allDebuffs = allDebuffs;
            _defencePassiveTree = defencePassiveTree;
            _defenceActiveTree = defenceActiveTree;
            _agilityActiveTree = agilityActiveTree;
            _agilityPassiveTree = agilityPassiveTree;
            _magicActiveTree = magicActiveTree;
            _magicPassiveTree = magicPassiveTree;
            _dealDmgToEnemy = dealDmgToEnemy;
            _updateFightPage = updateFightPage;
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

            double dmg = 0;
            switch (account.MoveListPage)
            {
                    case 1:
                        dmg =   _attackDamageActiveTree.AttackDamageActiveSkills(skill.SpellId, account, enemy, false);
                        break;
                    case 2:
                        dmg = _defenceActiveTree.DefSkills(skill.SpellId, account, enemy, false);
                        break;
                    case 3:
                        dmg = _agilityActiveTree.AgiActiveSkills(skill.SpellId, account, enemy, false);
                        break;
                    case 4:
                        dmg = _magicActiveTree.ApSkills(skill.SpellId, account, enemy, false);
                        break;
            }


            
            await _dealDmgToEnemy.DmgHealthHandeling(skill.WhereDmg, dmg, skill.SpellDmgType, account, enemy);
            await UpdateTurn(account, enemy);
       
        }




        public async Task UpdateTurn(AccountSettings account, AccountSettings enemy)
        {

            //TODO figure out who to update and when 

            await _allDebuffs.CheckForBuffs(account);
            await _allDebuffs.CheckForDeBuffs(enemy);
            await _allDebuffs.CheckForBuffsToBeActivatedLater(account);
            await _allDebuffs.CheckForDeBuffsToBeActivatedLater(enemy);


            if (account.SkillCooldowns != null)
                for (var i = 0; i < account.SkillCooldowns.Count; i++)
                {
                    account.SkillCooldowns[i].cooldown--;
                    if (account.SkillCooldowns[i].cooldown <= 0)
                        account.SkillCooldowns.RemoveAt(i);
                }



            await CheckForPassivesAndUpdateStats(account, enemy);
            await _updateFightPage.UpdateMainPageForAllPlayers(account);

            account.IsMyTurn = false;
            enemy.IsMyTurn = true;
            _accounts.SaveAccounts(account.Id);

        }




        public async Task CheckForPassivesAndUpdateStats(AccountSettings account, AccountSettings enemy)
        {
            for (var i = 0; i < account.PassiveList.Count; i++)
            {
                if (account.SkillCooldowns.Any(x => x.skillId == account.PassiveList[i].skillId))
                    continue;

                _attackDamagePassiveTree.AttackDamagePassiveSkills(account.PassiveList[i].skillId, account, enemy);
                _defencePassiveTree.DefPassiveSkills(account.PassiveList[i].skillId, account, enemy);
                _agilityPassiveTree.AgiPassiveSkills(account.PassiveList[i].skillId, account, enemy);
                _magicPassiveTree.ApPassiveSkills(account.PassiveList[i].skillId, account, enemy);

            }

            //TODO ?????
          //  account.IsCrit = false;

            _accounts.SaveAccounts(account.Id);
            _accounts.SaveAccounts(enemy.DiscordId);
            await Task.CompletedTask;
        }

        public string[] GetSkillListFromTree(AccountSettings account)
        {
            string[] skills = { };

            switch (account.MoveListPage)
            {
                case 1:
                    skills = account.Attack_Tree.Split(new[] {'|'},
                        StringSplitOptions.RemoveEmptyEntries);
                    break;
                case 2:
                    skills = account.Defensive_Tree.Split(new[] {'|'},
                        StringSplitOptions.RemoveEmptyEntries);
                    break;
                case 3:
                    skills = account.Agility_Tree.Split(new[] {'|'},
                        StringSplitOptions.RemoveEmptyEntries);
                    break;
                case 4:
                    skills = account.Magic_Tree.Split(new[] {'|'},
                        StringSplitOptions.RemoveEmptyEntries);
                    break;
            }

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
                    value = 7;
                    break;
                }

                case "8⃣":
                {
                    value = 8;
                    break;
                }
                
                case "9⃣":
                {
                    value = 9;
                    break;
                }

                case "🐙":
                {
                    value = 100;
                    break;
                }
            }

            return value;
        }
    }
}