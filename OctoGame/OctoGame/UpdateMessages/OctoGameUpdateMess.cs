using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using OctoGame.LocalPersistentData.GameSpellsAccounts;
using OctoGame.LocalPersistentData.UsersAccounts;
using OctoGame.OctoGame.SpellHandling.ActiveSkills;

namespace OctoGame.OctoGame.UpdateMessages
{
    public class OctoGameUpdateMess : ModuleBase<SocketCommandContext>
    {
        private readonly IUserAccounts _accounts;
        private readonly ISpellAccounts _spellAccounts;
        private readonly AttackDamageActiveTree _attackDamageActiveTree;
        private readonly AgilityActiveTree _agilityActiveTree;
        private readonly DefenceActiveTree _defenceActiveTree;
        private readonly MagicActiveTree _magicActiveTree;
      //  private readonly 5555 8888;

        private readonly Global _global;

        public OctoGameUpdateMess(IUserAccounts accounts, AttackDamageActiveTree attackDamageActiveTree,
            ISpellAccounts spellAccounts, Global global, MagicActiveTree magicActiveTree, DefenceActiveTree defenceActiveTree, AgilityActiveTree agilityActiveTree)
        {
            _accounts = accounts;
            _attackDamageActiveTree = attackDamageActiveTree;
            _spellAccounts = spellAccounts;
            _global = global;
            _magicActiveTree = magicActiveTree;
            _defenceActiveTree = defenceActiveTree;
            _agilityActiveTree = agilityActiveTree;
        }


        public async Task WaitMess(ulong userId, IUserMessage socketMsg)
        {

            var globalAccount = _global.Client.GetUser(userId);
            var account = _accounts.GetAccount(globalAccount);
            var mainPage = new EmbedBuilder();

            mainPage.WithAuthor(globalAccount);
            mainPage.WithFooter($"Preparation time...");
            mainPage.WithColor(Color.DarkGreen);
            mainPage.AddField("Game is being ready", $"**Please wait until you will see emoji** {new Emoji("❌")}");


            await socketMsg.ModifyAsync(message =>
            {
                message.Embed = mainPage.Build();
            });

     
            if(!(socketMsg.Channel is IDMChannel))
            await socketMsg.RemoveAllReactionsAsync();

            await socketMsg.AddReactionAsync(new Emoji("⬅"));
            await socketMsg.AddReactionAsync(new Emoji("➡"));
            await socketMsg.AddReactionAsync(new Emoji("📖"));

            await socketMsg.AddReactionAsync(new Emoji("1⃣"));
            await socketMsg.AddReactionAsync(new Emoji("2⃣"));
            await socketMsg.AddReactionAsync(new Emoji("3⃣"));
            await socketMsg.AddReactionAsync(new Emoji("4⃣"));
            await socketMsg.AddReactionAsync(new Emoji("5⃣"));
            await socketMsg.AddReactionAsync(new Emoji("6⃣"));
            await socketMsg.AddReactionAsync(new Emoji("7⃣"));
            await socketMsg.AddReactionAsync(new Emoji("8⃣"));
            await socketMsg.AddReactionAsync(new Emoji("9⃣"));
            await socketMsg.AddReactionAsync(new Emoji("🐙"));

            await socketMsg.AddReactionAsync(new Emoji("❌"));
                

            account.PlayingStatus = 2;
            _accounts.SaveAccounts(userId);

            await MainPage(userId, socketMsg);
        }


        public async Task MainPage(ulong userId, IUserMessage socketMsg)
        {
            var globalAccount = _global.Client.GetUser(userId);
            var account = _accounts.GetAccount(globalAccount);
            var enemy = _accounts.GetAccount(account.CurrentEnemy);


            var skillString = GetSkillString(account, enemy);

            var mainPage = FightPage(globalAccount, account, enemy, skillString);

            await socketMsg.ModifyAsync(message =>
            {
                message.Embed = mainPage.Build();
            });
        }


        public async Task VictoryPage(ulong userId, IUserMessage socketMsg)
        {


            await socketMsg.Channel.SendMessageAsync("Victory!");

        }

        public async Task SkillPageLeft(SocketReaction reaction, IUserMessage socketMsg)
        {
            var userId = reaction.User.Value.Id;
            var globalAccount = _global.Client.GetUser(reaction.UserId);
            var account = _accounts.GetAccount(globalAccount);
            var enemy = _accounts.GetAccount(account.CurrentEnemy);

            account.MoveListPage--;
            if (account.MoveListPage == 0)
                account.MoveListPage = 5;
            _accounts.SaveAccounts(userId);

            var skillString = GetSkillString(account, enemy);

            if (skillString == "404")
            {
                await SkillPageLeft(reaction, socketMsg);
                return;
            }

            var spellBookPage = FightPage(globalAccount, account, enemy, skillString);
          
            await socketMsg.ModifyAsync(message =>
            {
                message.Embed = spellBookPage.Build();
            });
        }


        public async Task SkillPageRight(SocketReaction reaction, IUserMessage socketMsg)
        {
            var userId = reaction.User.Value.Id;
            var globalAccount = _global.Client.GetUser(reaction.UserId);
            var account = _accounts.GetAccount(globalAccount);
            var enemy = _accounts.GetAccount(account.CurrentEnemy);

            account.MoveListPage++;
            if (account.MoveListPage == 6)
                account.MoveListPage = 1;
            _accounts.SaveAccounts(userId);

            var skillString = GetSkillString(account, enemy);

            if (skillString == "404")
            {
                await SkillPageRight(reaction, socketMsg);
                return;
            }

            var spellBookPage = FightPage(globalAccount, account, enemy, skillString);

            await socketMsg.ModifyAsync(message =>
            {
                message.Embed = spellBookPage.Build();
            });
        }


        public async Task OctoGameLogs(SocketReaction reaction, RestUserMessage socketMsg)
        {
            var userId = reaction.User.Value.Id;
            var globalAccount = _global.Client.GetUser(reaction.UserId);
            var account = _accounts.GetAccount(globalAccount);

            if (account.CurrentLogString.Length >= 1400)
            {
                account.CurrentLogString = "New page.";
                _accounts.SaveAccounts(userId);
            }

            var log = account.CurrentLogString ?? "No logs yet";

            var logPage = new EmbedBuilder();

            logPage.WithAuthor(globalAccount);
            logPage.WithFooter($"Log Fight Page");
            logPage.WithColor(Color.DarkGreen);
            logPage.AddField("Logs", $"{log}");


            await socketMsg.ModifyAsync(message =>
            {
                message.Embed = logPage.Build();
            });
        }


        public async Task EndGame(SocketReaction reaction, IUserMessage socketMsg)
        {
            var userId = reaction.User.Value.Id;
            var globalAccount = _global.Client.GetUser(reaction.UserId);
            var account = _accounts.GetAccount(globalAccount);
            account.PlayingStatus = 0;
            _accounts.SaveAccounts(userId);
            await socketMsg.DeleteAsync();
        }

        public string GetSkillString(AccountSettings account, AccountSettings enemy)
        {
            string[] skills;
            // var tree = "";
            double dmg;
            var skillString = "404";


            if (account.MoveListPage == 1 && account.Attack_Tree != null)
            {
                skillString = null;
                skills = account.Attack_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                //  tree = "AD";

                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);

                    dmg = _attackDamageActiveTree.AttackDamageActiveSkills(skill.SpellId, account, enemy, true);
                    skillString += ReturnSkillString(i, dmg, skill, account);
                }
            }
            else if (account.MoveListPage == 2 && account.Defensive_Tree != null)
            {
                skillString = null;
                skills = account.Defensive_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                //tree = "DEF";
                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);

                    dmg = _defenceActiveTree.DefSkills(skill.SpellId, account, enemy, true);
                    skillString += ReturnSkillString(i, dmg, skill, account);
                }
            }
            else if (account.MoveListPage == 3 && account.Agility_Tree != null)
            {
                skillString = null;
                skills = account.Agility_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                // tree = "AGI";
                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);

                    dmg = _agilityActiveTree.AgiActiveSkills(skill.SpellId, account, enemy, true);
                    skillString += ReturnSkillString(i, dmg, skill, account);
                }
            }
            else if (account.MoveListPage == 4 && account.Magic_Tree != null)
            {
                skillString = null;
                skills = account.Magic_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                //tree = "AP";

                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);

                    dmg = _magicActiveTree.ApSkills(skill.SpellId, account, enemy, true);
                    
                        skillString += ReturnSkillString(i, dmg, skill, account);
                }
            }
            else if (account.MoveListPage == 5 && account.AllPassives != null)
            {
                skillString = null;
                skills = account.AllPassives.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                //tree = "PASS";

                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);
;
                    skillString += ReturnSkillString(i, 0, skill, account);

                }
            }
            return skillString;
        }

        public string ReturnSkillString(int i, double dmg, SpellSetting skill, AccountSettings account)
        {

            var spellCd = 0;
            var spellCdString = "Ready";
            if(account.SkillCooldowns.Any(x => x.skillId == skill.SpellId))
                spellCd =  account.SkillCooldowns.Find(x => x.skillId == skill.SpellId).cooldown;

            if (spellCd != 0)
                spellCdString = $"{spellCd} Turns";

            if (skill.SpellType == 0)
                return
                    $"{i + 1}. **{skill.SpellNameEn}**: {skill.SpellDescriptionRu} *({spellCdString})*\n";
            
                return
                    $"{i + 1}. **{skill.SpellNameEn}**: {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}** *({spellCdString})*\n";
          
        }

        public EmbedBuilder FightPage(SocketUser globalAccount, AccountSettings account, AccountSettings enemy,
            string skillString)
        {
            //{new Emoji("<:Steampunk:445276776676196353>")} 
         //   var clearEmoji = Emote.Parse("<:Clear:530625040006381579>");
            var accountShields = "";
            if (account.PhysShield > 0)
                accountShields += $"{new Emoji("<:Clear:530625040006381579>")} **Shields:** ({account.PhysShield} Phys)";
            if (account.MagShield > 0)
                accountShields += $" {new Emoji("<:Clear:530625040006381579>")} ({account.MagShield} Mag)";

            var enemyShields = "";
            if (enemy.PhysShield > 0)
                enemyShields += $"{new Emoji("<:Clear:530625040006381579>")} **Shields:** ({enemy.PhysShield} Phys)";
            if (enemy.MagShield > 0)
                enemyShields += $" {new Emoji("<:Clear:530625040006381579>")} ({enemy.MagShield} Mag)";

            var skillStringList = new List<string> ();
            if (skillString.Length > 950)
                skillStringList = skillString.Split("7. ").ToList();
 


            var mainPage = new EmbedBuilder();
            mainPage.WithAuthor(globalAccount);
            mainPage.WithFooter($"Move List Page {account.MoveListPage} from 5");
            mainPage.WithColor(Color.DarkGreen);
            mainPage.AddField("Enemy:", 
                                        $"**Name:** {enemy.DiscordUserName}\n" +
                                        $"**LVL:** {enemy.OctoLvL}\n" +
                                        $"-\n" +
                                        $"**Strength:** {enemy.Strength} {new Emoji("<:Clear:530625040006381579>")} {new Emoji("<:Clear:530625040006381579>")} **Agility:** {enemy.Agility_Stats}\n" +
                                        $"**Attack:** {enemy.AttackPower_Stats + enemy.Strength} {new Emoji("<:Clear:530625040006381579>")} {new Emoji("<:Clear:530625040006381579>")} **Magic:** {enemy.MagicPower_Stats}\n" +

                                         $"-\n" +
                                        $"**Health:** {enemy.Health}\n" +
                                        $"**Stamina:** {enemy.Stamina} {enemyShields}\n" +
                                         $"-\n" +
                                        $"**Physical Resistance:** {enemy.PhysicalResistance} {new Emoji("<:Clear:530625040006381579>")} {new Emoji("<:Clear:530625040006381579>")} **Magical Resistance:** {enemy.MagicalResistance}\n" +
                                        $"**Physical Penetration:** {enemy.PhysicalPenetration} {new Emoji("<:Clear:530625040006381579>")} {new Emoji("<:Clear:530625040006381579>")}  **Magical Penetration:** {enemy.MagicalPenetration}\n" +
                                        $"{new Emoji("<:Clear:530625040006381579>")}\n" +
                                        "**________________**");

            mainPage.AddField($"Your octopus:",
                $"**Name:** {account.OctoName}\n" +
                $"**LVL:** {account.OctoLvL}\n" +
                 $"-\n" +
                $"**Strength:** {account.Strength} {new Emoji("<:Clear:530625040006381579>")} {new Emoji("<:Clear:530625040006381579>")} **Agility:** {account.Agility_Stats}\n" +
                $"**Attack:** {account.AttackPower_Stats + account.Strength} {new Emoji("<:Clear:530625040006381579>")} {new Emoji("<:Clear:530625040006381579>")} **Magic:** {account.MagicPower_Stats}\n" +

                 $"-\n" +
                $"**Health:** {account.Health}\n" +            
                $"**Stamina:** {account.Stamina} {accountShields}\n" +  
                 $"-\n" +
                $"**Physical Resistance:** {account.PhysicalResistance} {new Emoji("<:Clear:530625040006381579>")} {new Emoji("<:Clear:530625040006381579>")} **Mag Resistance:** {account.MagicalResistance}\n" +
                $"**Physical Penetration:** {account.PhysicalPenetration} {new Emoji("<:Clear:530625040006381579>")} {new Emoji("<:Clear:530625040006381579>")} **Magical Penetration:** {account.MagicalPenetration}\n" +
               
                $"{new Emoji("<:Clear:530625040006381579>")}\n" +
                $"**________________**\n" +
                $"{new Emoji("⬅")} - Move List Page Left , {new Emoji("➡")} - Move List Page Right {new Emoji("📖")} - History, {new Emoji("❌")} - **END GAME**\n" +
                $"{new Emoji("<:Clear:530625040006381579>")}");
            if(skillStringList.Count <= 1)
                mainPage.AddField($"{GetTree(account)} Move List:",              
                 $"{skillString}\n");
            else
            {
                mainPage.AddField($"{GetTree(account)} Move List Part 1:",              
                    $"{skillStringList[0]}\n");
                mainPage.AddField($"{GetTree(account)} Move List Part 2:",              
                    $"7. {skillStringList[1]}\n");
            }


            
 

            if (IsImageUrl(enemy.OctoAvatar))
            mainPage.WithThumbnailUrl(enemy.OctoAvatar);
            
            return mainPage;
        }

        string GetTree(AccountSettings user)
        {
            var tree = "";
            switch (user.MoveListPage)
            {
                case 1:
                    tree = "Attack";
                    break;
                case 2:
                    tree = "Defensive";
                    break;
                case 3:
                    tree = "Agility";
                    break;
                case 4:
                    tree = "Magic";
                    break;
                case 5:
                    tree = "Passive";
                    break;
            }
            return tree;
        }

        bool IsImageUrl(string url)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "HEAD";
            using (var resp = req.GetResponse())
            {
                return resp.ContentType.ToLower(CultureInfo.InvariantCulture)
                    .StartsWith("image/");
            }
        }
    }
}