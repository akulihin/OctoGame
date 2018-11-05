using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using OctoGame.Accounts.GameSpells;
using OctoGame.Accounts.Users;

namespace OctoGame.OctoGame.UpdateMessages
{
    public class OctoGameUpdateMess : ModuleBase<SocketCommandContext>
    {
        private readonly IUserAccounts _accounts;
        private readonly ISpellAccounts _spellAccounts;
        private readonly GameSpellHandeling _gameSpellHandeling;
        private readonly Global _global;

        public OctoGameUpdateMess(IUserAccounts accounts, GameSpellHandeling gameSpellHandeling,
            ISpellAccounts spellAccounts, Global global)
        {
            _accounts = accounts;
            _gameSpellHandeling = gameSpellHandeling;
            _spellAccounts = spellAccounts;
            _global = global;
        }


        public async Task WaitMess(SocketReaction reaction, RestUserMessage socketMsg)
        {
            var userId = reaction.User.Value.Id;
            var globalAccount = _global.Client.GetUser(reaction.UserId);
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

            await socketMsg.RemoveAllReactionsAsync();

            await socketMsg.AddReactionAsync(new Emoji("⬅"));
            await socketMsg.AddReactionAsync(new Emoji("➡"));
            await socketMsg.AddReactionAsync(new Emoji("📖"));

            await socketMsg.AddReactionAsync(new Emoji("1⃣"));
            await socketMsg.AddReactionAsync(new Emoji("2⃣"));
            await socketMsg.AddReactionAsync(new Emoji("3⃣"));
            await socketMsg.AddReactionAsync(new Emoji("4⃣"));
             /* await socketMsg.AddReactionAsync(new Emoji("5⃣"));
              await socketMsg.AddReactionAsync(new Emoji("6⃣"));
              await socketMsg.AddReactionAsync(new Emoji("7⃣"));
              await socketMsg.AddReactionAsync(new Emoji("8⃣"));
            */
            await socketMsg.AddReactionAsync(new Emoji("❌"));

            account.PlayingStatus = 2;
            _accounts.SaveAccounts(userId);

            await MainPage(reaction, socketMsg);
        }


        public async Task MainPage(SocketReaction reaction, RestUserMessage socketMsg)
        {
            var globalAccount = _global.Client.GetUser(reaction.UserId);
            var account = _accounts.GetAccount(globalAccount);
            var enemy = _accounts.GetBotAccount(account.CurrentEnemy);


            var skillString = GetSkillString(account, enemy);

            var mainPage = FightPage(globalAccount, account, enemy, skillString);

            await socketMsg.ModifyAsync(message =>
            {
                message.Embed = mainPage.Build();
            });
        }


        public async Task SkillPageLeft(SocketReaction reaction, RestUserMessage socketMsg)
        {
            var userId = reaction.User.Value.Id;
            var globalAccount = _global.Client.GetUser(reaction.UserId);
            var account = _accounts.GetAccount(globalAccount);
            var enemy = _accounts.GetBotAccount(account.CurrentEnemy);

            account.MoveListPage--;
            if (account.MoveListPage == 0)
                account.MoveListPage = 4;
            _accounts.SaveAccounts(userId);

            var skillString = GetSkillString(account, enemy);

            var spellBookPage = FightPage(globalAccount, account, enemy, skillString);
          
            await socketMsg.ModifyAsync(message =>
            {
                message.Embed = spellBookPage.Build();
            });
        }


        public async Task SkillPageRight(SocketReaction reaction, RestUserMessage socketMsg)
        {
            var userId = reaction.User.Value.Id;
            var globalAccount = _global.Client.GetUser(reaction.UserId);
            var account = _accounts.GetAccount(globalAccount);
            var enemy = _accounts.GetBotAccount(account.CurrentEnemy);

            account.MoveListPage++;
            if (account.MoveListPage == 5)
                account.MoveListPage = 1;
            _accounts.SaveAccounts(userId);

            var skillString = GetSkillString(account, enemy);

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


        public async Task EndGame(SocketReaction reaction, RestUserMessage socketMsg)
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
            var skillString = "You dont have any skills here.";


            if (account.MoveListPage == 1 && account.AD_Tree != null)
            {
                skillString = null;
                skills = account.AD_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                //  tree = "AD";

                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);

                    dmg = _gameSpellHandeling.AdSkills(skill.SpellId, account, enemy, true);
                    skillString += ReturnSkillString(i, dmg, skill, account);
                }
            }
            else if (account.MoveListPage == 2 && account.DEF_Tree != null)
            {
                skillString = null;
                skills = account.DEF_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                //tree = "DEF";
                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);

                    dmg = _gameSpellHandeling.DefdSkills(skill.SpellId, account);
                    skillString += ReturnSkillString(i, dmg, skill, account);
                }
            }
            else if (account.MoveListPage == 3 && account.AG_Tree != null)
            {
                skillString = null;
                skills = account.AG_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                // tree = "AGI";
                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);

                    dmg = _gameSpellHandeling.AgiSkills(skill.SpellId, account);
                    skillString += ReturnSkillString(i, dmg, skill, account);
                }
            }
            else if (account.MoveListPage == 4 && account.AP_Tree != null)
            {
                skillString = null;
                skills = account.AP_Tree.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                //tree = "AP";

                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);

                    dmg = _gameSpellHandeling.ApSkills(skill.SpellId, account);
                    
                        skillString += ReturnSkillString(i, dmg, skill, account);
                }
            }
            else if (account.MoveListPage == 5 && account.Passives != null)
            {
                skillString = null;
                skills = account.Passives.Split(new[] {'|'},
                    StringSplitOptions.RemoveEmptyEntries);
                //tree = "PASS";

                for (var i = 0; i < skills.Length; i++)
                {
                    var ski = Convert.ToUInt64(skills[i]);
                    var skill = _spellAccounts.GetAccount(ski);

                    dmg = _gameSpellHandeling.ApSkills(skill.SpellId, account);
                        skillString +=
                            $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";

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

            if (skill.SpellDmgType == "PASS")
                return
                    $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}** ({spellCdString})\n";
            
                return
                    $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** ({spellCdString})\n";
          
        }

        public EmbedBuilder FightPage(SocketUser globalAccount, AccountSettings account, AccountSettings enemy,
            string skillString)
        {
            //{new Emoji("<:Steampunk:445276776676196353>")} 
            
            var mainPage = new EmbedBuilder();
            mainPage.WithAuthor(globalAccount);
            mainPage.WithFooter($"Move List Page {account.MoveListPage} from 4");
            mainPage.WithColor(Color.DarkGreen);
            mainPage.AddField("Enemy:", 
                                        $"**Name:** {enemy.UserName}\n" +
                                        $"**LVL:** {enemy.OctoLvL}\n" +
                                        $"**Strength:** {enemy.Strength}\n" +
                                        $"**AD:** {enemy.AD_Stats + enemy.Strength}  **AP:** {enemy.AP_Stats}\n" +
                                        $"**Health:** {enemy.Health}\n" +
                                        $"**Stamina:** {enemy.Stamina}\n" +
                                        $"**Armor:** {enemy.Armor} **MagRes:** {enemy.Resist}\n" +
                                        $"**ArmPen:** {enemy.ArmPen}  **MagPen:** {enemy.MagPen}\n" +
                                        $"**Agility:** {enemy.AG_Stats}\n" +
                                        "**________________**");

            mainPage.AddField($"Your octopus:",
                $"**Name:** {account.OctoName}\n" +
                $"**LVL:** {account.OctoLvL}\n" +
                $"**Strength:** {account.Strength}\n" +
                $"**AD:** {account.AD_Stats + account.Strength}  **AP:** {account.AP_Stats}\n" +
                $"**Health:** {account.Health}\n" +            
                $"**Stamina:** {account.Stamina}\n" +            
                $"**Armor:** {account.Armor} **MagRes:** {account.Resist}\n" +
                $"**ArmPen:** {account.ArmPen}  **MagPen:** {account.MagPen}\n" +
                $"**Agility:** {account.AG_Stats}\n" +
                $"**________________**\n" +
                $"{new Emoji("⬅")} - Move List Page Left , {new Emoji("➡")} - Move List Page Right {new Emoji("📖")} - History, {new Emoji("❌")} - **END GAME**");
            mainPage.AddField($"{GetTree(account)} Move List:", 
                                                                $"{skillString}\n");
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
                    tree = "AD";
                    break;
                case 2:
                    tree = "DEF";
                    break;
                case 3:
                    tree = "AGI";
                    break;
                case 4:
                    tree = "AP";
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