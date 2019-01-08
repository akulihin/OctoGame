using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace OctoGame.DiscordFramework.CustomLibrary
{
   public class ModuleBaseCustom : ModuleBase<SocketCommandContextCustom>
   {

       protected virtual async Task SendMessAsync(EmbedBuilder embed)
       {
           switch (Context.MessageContentForEdit)
           {
               case null:
               {
                   var message = await Context.Channel.SendMessageAsync("", false, embed.Build());


                   Context.Global.CommandList.Insert(0, new Global.CommandRam(Context.User, Context.Message, message));
                   if (Context.Global.CommandList.Count > 1000)
                       Context.Global.CommandList.RemoveAt(Context.Global.CommandList.Count - 1);
                   break;
               }
               case "edit":
               {
                   foreach (var t in Context.Global.CommandList)
                       if (t.UserSocketMsg.Id == Context.Message.Id)
                           await t.BotSocketMsg.ModifyAsync(message =>
                           {
                               message.Content = "";
                               message.Embed = null;
                               message.Embed = embed.Build();
                           });
                   break;
               }
           }
       }


       protected virtual async Task SendMessAsync([Remainder] string regularMess = null)
       {
           switch (Context.MessageContentForEdit)
           {
               case null:
               {
                   var message = await Context.Channel.SendMessageAsync($"{regularMess}");

                   Context.Global.CommandList.Insert(0, new Global.CommandRam(Context.User, Context.Message, message));
                   if (Context.Global.CommandList.Count > 1000)
                       Context.Global.CommandList.RemoveAt(Context.Global.CommandList.Count - 1);
                   break;
               }
               case "edit":
               {
                   foreach (var t in Context.Global.CommandList)
                       if (t.UserSocketMsg.Id == Context.Message.Id)
                           await t.BotSocketMsg.ModifyAsync(message =>
                           {
                               message.Content = "";
                               message.Embed = null;
                               if (regularMess != null) message.Content = regularMess.ToString();
                           });
                   break;
               }
           }
       }


       protected virtual async Task SendMessAsync([Remainder] string regularMess, SocketCommandContextCustom context)
       {
           switch (context.MessageContentForEdit)
           {
               case null:
               {
                   var message = await context.Channel.SendMessageAsync($"{regularMess}");

                   context.Global.CommandList.Insert(0, new Global.CommandRam(context.User, context.Message, message));
                   if(context.Global.CommandList.Count > 1000)
                       context.Global.CommandList.RemoveAt(context.Global.CommandList.Count-1);
                   break;
               }
               case "edit":
               {
                   foreach (var t in context.Global.CommandList)
                       if (t.UserSocketMsg.Id == context.Message.Id)
                           await t.BotSocketMsg.ModifyAsync(message =>
                           {
                               message.Content = "";
                               message.Embed = null;
                               if (regularMess != null) message.Content = regularMess.ToString();
                           });
                   break;
               }
           }
       }




    }
}
