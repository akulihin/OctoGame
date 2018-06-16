using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using OctoBot.Configs;
using OctoBot.Games.OctoGame.GameSpells;
using OctoBot.Games.OctoGame.GameUsers;

namespace OctoBot.Games.OctoGame
{
    public class OctoGameUpdateMess : ModuleBase<SocketCommandContext>
    {

           
        public static class FighhtReaction 
        {
            public static async Task WaitMess(SocketReaction reaction, RestUserMessage socketMsg)
            {
                var globalAccount = Global.Client.GetUser(reaction.UserId);
                var account = GameUserAccounts.GetAccount(globalAccount);

                var mainPage = new EmbedBuilder();

                mainPage.WithAuthor(globalAccount);
                mainPage.WithFooter($"Preparation time...");
                mainPage.WithColor(Color.DarkGreen);
                mainPage.AddField("Game is being ready", $"**Please wait until you will see emoji** {new Emoji("❌")}");
              


                await socketMsg.ModifyAsync(message =>
                {
                    message.Embed = mainPage.Build();
                    // This somehow can't be empty or it won't update the 
                    // embed propperly sometimes... I don't know why
                    // message.Content =  Constants.InvisibleString;
                });

                await socketMsg.RemoveAllReactionsAsync();
                     
                await socketMsg.AddReactionAsync(new Emoji("⬅"));  
                await socketMsg.AddReactionAsync(new Emoji("➡"));     
                await socketMsg.AddReactionAsync(new Emoji("📖"));
                
                await socketMsg.AddReactionAsync(new Emoji("1⃣"));             
                await socketMsg.AddReactionAsync(new Emoji("2⃣"));           
                await socketMsg.AddReactionAsync(new Emoji("3⃣"));
              /*await socketMsg.AddReactionAsync(new Emoji("4⃣"));
                await socketMsg.AddReactionAsync(new Emoji("5⃣"));
                await socketMsg.AddReactionAsync(new Emoji("6⃣"));
                await socketMsg.AddReactionAsync(new Emoji("7⃣"));
                await socketMsg.AddReactionAsync(new Emoji("8⃣"));
              */
                await socketMsg.AddReactionAsync(new Emoji("❌"));

                account.OctopusFightPlayingStatus = 2;
                GameUserAccounts.SaveAccounts();
                
                await MainPage(reaction, socketMsg);
            }


            public static async Task MainPage( SocketReaction reaction, RestUserMessage socketMsg)
            {
                var globalAccount = Global.Client.GetUser(reaction.UserId);
                var account = GameUserAccounts.GetAccount(globalAccount);


                string[] skills;
               // var tree = "";
                double dmg ;
            var skillString = "You dont have any skills here."; 

                if (account.MoveListPage == 1 && account.CurrentOctopusFighterSkillSetAd != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                  //  tree = "AD";

                    for (var i = 0; i < skills.Length; i++)
                    {

                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);

                        dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }
                }
                else if (account.MoveListPage == 2 && account.CurrentOctopusFighterSkillSetDef != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    //tree = "DEF";
                    for (var i = 0; i < skills.Length; i++)
                    {

                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);

                         dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }
                }
                else if (account.MoveListPage == 3 && account.CurrentOctopusFighterSkillSetAgi != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                   // tree = "AGI";
                    for (var i = 0; i < skills.Length; i++)
                    {

                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);
                       
                         dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }
                }
                else if (account.MoveListPage == 4 && account.CurrentOctopusFighterSkillSetAp != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    //tree = "AP";

                    for (var i = 0; i < skills.Length; i++)
                    {
                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);

                         dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }
                }
               
              
                var mainPage = new EmbedBuilder();
                
                mainPage.WithAuthor(globalAccount);
                mainPage.WithFooter($"Move List Page {account.MoveListPage} from 4");
                mainPage.WithColor(Color.DarkGreen);
                mainPage.AddField("Enemy:", $"**LVL:** {account.CurrentEnemyLvl}\n" +
                                           $"**Strength:** {account.CurrentEnemyStrength}\n" +
                                           $"**AD:** {account.CurrentEnemyAd + account.CurrentEnemyStrength}  **AP:** {account.CurrentEnemyAp}\n" +                                   
                                           $"**Health:** {account.CurrentEnemyHealth}\n" +                  
                                           $"**Stamina:** {account.CurrentEnemyStamina}\n" +
                                           $"**Armor:** {account.CurrentEnemyArmor} LVL  **MagRes:** {account.CurrentEnemyMagicResist} LVL\n" +
                                           $"**ArmPen:** {account.CurrentEnemyArmPen} LVL  **MagPen:** {account.CurrentEnemyMagPen}\n" + 
                                           $"**Agility:** {account.CurrentEnemyAgility}\n" + 
                                           "**________________**");

                mainPage.AddField($"Your octopus:",
                    $"**LVL:** {account.CurrentOctopusFighterLvl}\n" +
                    $"**Strength:** {account.CurrentOctopusFighterStrength}\n" +
                    $"**AD:** {account.CurrentOctopusFighterAd + account.CurrentOctopusFighterStrength}  **AP:** {account.CurrentOctopusFighterAp}\n" +
                    $"**Health:** {account.CurrentOctopusFighterHealth}\n" +
                    $"**Stamina:** {account.CurrentOctopusFighterStamina}\n" +
                    $"**Armor:** {account.CurrentOctopusFighterArmor} LVL  **MagRes:** {account.CurrentOctopusFighterMagicResist} LVL\n" +
                    $"**ArmPen:** {account.CurrentOctopusFighterArmPen}  **MagPen:** {account.CurrentOctopusFighterMagPen}\n" +
                    $"**Agility:** {account.CurrentOctopusFighterAgility}\n" +
                    $"**________________**\n" +
                    $"{new Emoji("⬅")} - Move List Page Left , {new Emoji("➡")} - Move List Page Right {new Emoji("📖")} - History, {new Emoji("❌")} - **END GAME**");
                mainPage.AddField($"(Move List", $"{skillString}");    



                await socketMsg.ModifyAsync(message =>
                {
                    message.Embed = mainPage.Build();
                    // This somehow can't be empty or it won't update the 
                    // embed propperly sometimes... I don't know why
                    // message.Content =  Constants.InvisibleString;
                });
            }


            public static async Task SkillPageLeft(SocketReaction reaction,  RestUserMessage socketMsg)
            {
                var globalAccount = Global.Client.GetUser(reaction.UserId);
                var account = GameUserAccounts.GetAccount(globalAccount);

                account.MoveListPage--;
                if (account.MoveListPage == 0)
                    account.MoveListPage = 4;
                GameUserAccounts.SaveAccounts();

                string[] skills;
                var tree = "";
                double dmg ;
                var skillString = "You dont have any skills here."; 

                if (account.MoveListPage == 1 && account.CurrentOctopusFighterSkillSetAd != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    tree = "AD";

                    for (var i = 0; i < skills.Length; i++)
                    {

                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);

                        dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }
                }
                else if (account.MoveListPage == 2 && account.CurrentOctopusFighterSkillSetDef != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    tree = "DEF";
                    for (var i = 0; i < skills.Length; i++)
                    {

                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);

                         dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }
                }
                else if (account.MoveListPage == 3 && account.CurrentOctopusFighterSkillSetAgi != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    tree = "AGI";
                    for (var i = 0; i < skills.Length; i++)
                    {

                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);
                       
                         dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }
                }
                else if (account.MoveListPage == 4 && account.CurrentOctopusFighterSkillSetAp != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    tree = "AP";

                    for (var i = 0; i < skills.Length; i++)
                    {
                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);

                         dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }
                }
                
                 
         
          var spellBookPage = new EmbedBuilder();
                spellBookPage.WithAuthor(globalAccount);
                spellBookPage.WithFooter($"Move List Page {account.MoveListPage} from 4");
                spellBookPage.WithColor(Color.DarkGreen);
                spellBookPage.WithAuthor(globalAccount);
             
             
                spellBookPage.AddField("Enemy:", $"**LVL:** {account.CurrentEnemyLvl}\n" +
                                           $"**Strength:** {account.CurrentEnemyStrength}\n" +
                                           $"**AD:** {account.CurrentEnemyAd + account.CurrentEnemyStrength}  **AP:** {account.CurrentEnemyAp}\n" +                                   
                                           $"**Health:** {account.CurrentEnemyHealth}\n" +                  
                                           $"**Stamina:** {account.CurrentEnemyStamina}\n" +
                                           $"**Armor:** {account.CurrentEnemyArmor} LVL  **MagRes:** {account.CurrentEnemyMagicResist} LVL\n" +
                                           $"**ArmPen:** {account.CurrentEnemyArmPen} LVL  **MagPen:** {account.CurrentEnemyMagPen}\n" + 
                                           $"**Agility:** {account.CurrentEnemyAgility}\n" + 
                                           $"**________________**");

                spellBookPage.AddField($"Your octopus:",
                    $"**LVL:** {account.CurrentOctopusFighterLvl}\n" +
                    $"**Strength:** {account.CurrentOctopusFighterStrength}\n" +
                    $"**AD:** {account.CurrentOctopusFighterAd + account.CurrentOctopusFighterStrength}  **AP:** {account.CurrentOctopusFighterAp}\n" +
                    $"**Health:** {account.CurrentOctopusFighterHealth}\n" +                  
                    $"**Stamina:** {account.CurrentOctopusFighterStamina}\n" +
                    $"**Armor:** {account.CurrentOctopusFighterArmor} LVL  **MagRes:** {account.CurrentOctopusFighterMagicResist} LVL\n" +
                    $"**ArmPen:** {account.CurrentOctopusFighterArmPen}  **MagPen:** {account.CurrentOctopusFighterMagPen}\n" + 
                    $"**Agility:** {account.CurrentOctopusFighterAgility}\n" + 
                    $"**________________**\n" +
                    $"{new Emoji("⬅")} - Move List Page Left , {new Emoji("➡")} - Move List Page Right {new Emoji("📖")} - History, {new Emoji("❌")} - **END GAME**");

                spellBookPage.AddField($"{tree} Move List", $"{skillString}");
              
             

                await socketMsg.ModifyAsync(message =>
                {
                    message.Embed = spellBookPage.Build();
                    // This somehow can't be empty or it won't update the 
                    // embed propperly sometimes... I don't know why
                    // message.Content =  Constants.InvisibleString;
                });

            }


            public static async Task SkillPageRight(SocketReaction reaction,  RestUserMessage socketMsg)
            {
                var globalAccount = Global.Client.GetUser(reaction.UserId);
                var account = GameUserAccounts.GetAccount(globalAccount);

                account.MoveListPage++;
                if (account.MoveListPage == 5)
                    account.MoveListPage = 1;
                GameUserAccounts.SaveAccounts();


                string[] skills;
                var tree = "";
                double dmg ;
                var skillString = "You dont have any skills here."; 

                if (account.MoveListPage == 1 && account.CurrentOctopusFighterSkillSetAd != null)
                { 
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetAd.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    tree = "AD";

                    for (var i = 0; i < skills.Length; i++)
                    {

                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);

                        dmg = GameSpellHandeling.AdSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }
                }
                else if (account.MoveListPage == 2 && account.CurrentOctopusFighterSkillSetDef != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetDef.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    tree = "DEF";
                    for (var i = 0; i < skills.Length; i++)
                    {

                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);

                         dmg = GameSpellHandeling.DefdSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n ";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}**  \n";
                        }
                    }
                  

                }
                else if (account.MoveListPage == 3 && account.CurrentOctopusFighterSkillSetAgi != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetAgi.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    tree = "AGI";
                    for (var i = 0; i < skills.Length; i++)
                    {

                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);
                       
                         dmg = GameSpellHandeling.AgiSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }   
                      
                    }
                
                }
                else if (account.MoveListPage == 4 && account.CurrentOctopusFighterSkillSetAp != null)
                {
                    skillString = null;
                    skills = account.CurrentOctopusFighterSkillSetAp.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                    tree = "AP";

                    for (var i = 0; i < skills.Length; i++)
                    {
                        var ski = Convert.ToUInt64(skills[i]);
                        var skill = SpellUserAccounts.GetAccount(ski);

                         dmg = GameSpellHandeling.ApSkills(skill.SpellId, account);
                        if (skill.SpellDmgType == "PASS")
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): {skill.SpellDescriptionRu} **{Math.Ceiling(dmg)}**\n";
                        }
                        else
                        {
                            skillString +=
                                $"{i + 1}. {skill.SpellName} ({skill.SpellDmgType}): **{Math.Ceiling(dmg)}** \n";
                        }
                    }

                }
              

                var spellBookPage = new EmbedBuilder();
                spellBookPage.WithAuthor(globalAccount);
                spellBookPage.WithFooter($"Move List Page {account.MoveListPage} from 4");
                spellBookPage.WithColor(Color.DarkGreen);
                spellBookPage.WithAuthor(globalAccount);
                
             
                spellBookPage.AddField("Enemy:", $"**LVL:** {account.CurrentEnemyLvl}\n" +
                                           $"**Strength:** {account.CurrentEnemyStrength}\n" +
                                           $"**AD:** {account.CurrentEnemyAd + account.CurrentEnemyStrength}  **AP:** {account.CurrentEnemyAp}\n" +                                   
                                           $"**Health:** {account.CurrentEnemyHealth}\n" +                  
                                           $"**Stamina:** {account.CurrentEnemyStamina}\n" +
                                           $"**Armor:** {account.CurrentEnemyArmor} LVL  **MagRes:** {account.CurrentEnemyMagicResist} LVL\n" +
                                           $"**ArmPen:** {account.CurrentEnemyArmPen} LVL  **MagPen:** {account.CurrentEnemyMagPen}\n" + 
                                           $"**Agility:** {account.CurrentEnemyAgility}\n" + 
                                           $"**________________**");

                spellBookPage.AddField($"Your octopus:",
                    $"**LVL:** {account.CurrentOctopusFighterLvl}\n" +
                    $"**Strength:** {account.CurrentOctopusFighterStrength}\n" +
                    $"**AD:** {account.CurrentOctopusFighterAd + account.CurrentOctopusFighterStrength}  **AP:** {account.CurrentOctopusFighterAp}\n" +
                    $"**Health:** {account.CurrentOctopusFighterHealth}\n" +                  
                    $"**Stamina:** {account.CurrentOctopusFighterStamina}\n" +
                    $"**Armor:** {account.CurrentOctopusFighterArmor} LVL  **MagRes:** {account.CurrentOctopusFighterMagicResist} LVL\n" +
                    $"**ArmPen:** {account.CurrentOctopusFighterArmPen}  **MagPen:** {account.CurrentOctopusFighterMagPen}\n" + 
                    $"**Agility:** {account.CurrentOctopusFighterAgility}\n" + 
                    $"**________________**\n" +
                    $"{new Emoji("⬅")} - Move List Page Left , {new Emoji("➡")} - Move List Page Right {new Emoji("📖")} - History, {new Emoji("❌")} - **END GAME**");

                spellBookPage.AddField($"{tree} Move List", $"{skillString}");
              


                await socketMsg.ModifyAsync(message =>
                {
                    message.Embed = spellBookPage.Build();
                    // This somehow can't be empty or it won't update the 
                    // embed propperly sometimes... I don't know why
                    // message.Content =  Constants.InvisibleString;
                });

            }




            public static async Task OctoGameLogs(SocketReaction reaction,  RestUserMessage socketMsg)
            {
                var globalAccount = Global.Client.GetUser(reaction.UserId);
                var account = GameUserAccounts.GetAccount(globalAccount);

                if (account.CurrentLogString.Length >= 1400)
                {
                    account.CurrentLogString = "New page.";
                    GameUserAccounts.SaveAccounts();
                    
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

                    // This somehow can't be empty or it won't update the 
                    // embed propperly sometimes... I don't know why
                    // message.Content =  Constants.InvisibleString;
                });

            }


            public static async Task EndGame( SocketReaction reaction, RestUserMessage socketMsg)
            {

         
                var globalAccount = Global.Client.GetUser(reaction.UserId);
                var account = GameUserAccounts.GetAccount(globalAccount);
                account.OctopusFightPlayingStatus = 0;
                GameUserAccounts.SaveAccounts();
                await socketMsg.DeleteAsync();
        

            }



        }

      
  

    }
}
