using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace OctoGame.DiscordFramework.Extensions
{
   public class ModuleBaseCustom : ModuleBase<SocketCommandContextCustom>
   {

       protected virtual async Task SendMessAsync(EmbedBuilder embed)
       {
           if (Context.MessageContentForEdit == null)
           {
               var message = await Context.Channel.SendMessageAsync("", false, embed.Build());


               UpdateGlobalCommandList(message, Context);
           }
           else if (Context.MessageContentForEdit == "edit")
           {
               foreach (var t in Context.CommandsInMemory.CommandList)
                   if (t.MessageUserId == Context.Message.Id)
                       await t.BotSocketMsg.ModifyAsync(message =>
                       {
                           message.Content = "";
                           message.Embed = null;
                           message.Embed = embed.Build();
                       });
           }
       }


       protected virtual async Task SendMessAsync([Remainder] string regularMess = null)
       {
           if (Context.MessageContentForEdit == null)
           {
               var message = await Context.Channel.SendMessageAsync($"{regularMess}");

               UpdateGlobalCommandList(message, Context);
           }
           else if (Context.MessageContentForEdit == "edit")
           {
               foreach (var t in Context.CommandsInMemory.CommandList)
                   if (t.MessageUserId == Context.Message.Id)
                       await t.BotSocketMsg.ModifyAsync(message =>
                       {
                           message.Content = "";
                           message.Embed = null;
                           if (regularMess != null) message.Content = regularMess;
                       });
           }
       }


       protected virtual async Task SendMessAsync([Remainder] string regularMess, SocketCommandContextCustom context)
       {
           if (context.MessageContentForEdit == null )
           {
               var message = await context.Channel.SendMessageAsync($"{regularMess}");

               UpdateGlobalCommandList(message, context);
           }
           else if (context.MessageContentForEdit == "edit")
           {
               foreach (var t in context.CommandsInMemory.CommandList)
                   if (t.MessageUserId == context.Message.Id)
                       await t.BotSocketMsg.ModifyAsync(message =>
                       {
                           message.Content = "";
                           message.Embed = null;
                           if (regularMess != null) message.Content = regularMess;
                       });
           }
       }


       private static void UpdateGlobalCommandList(IUserMessage message, SocketCommandContextCustom context)
       {
           context.CommandsInMemory.CommandList.Insert(0, new CommandsInMemory.CommandRam(context.Message, message));
           if(context.CommandsInMemory.CommandList.Count > context.CommandsInMemory.MaximumCommandsInRam)
               context.CommandsInMemory.CommandList.RemoveAt(context.CommandsInMemory.CommandList.Count-1);
       }
    }
}