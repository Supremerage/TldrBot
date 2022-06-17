using Discord;
using Discord.Commands;
using Discord.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextGenerationBot.App.Modules
{
    public class SlashCommandModule : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("fakenews", "Generate some fake news headlines")]
        public async Task FakeNews()
        {
            var modalBuilder = new ModalBuilder()
                .WithTitle("Enter a fake news title (or part of it)")
                .WithCustomId("fake_news")
                .AddTextInput("Prompt", "prompt", TextInputStyle.Short, "Entire city of Copenhagen on fire", 5, 100, true);

            await Context.Interaction.RespondWithModalAsync(modalBuilder.Build());
        }

        [SlashCommand("textgenerator", "Generate some random text based on a prompt")]
        public async Task TextGenerator()
        {
            var modalBuilder = new ModalBuilder()
                .WithTitle("Enter a fake news title (or part of it)")
                .WithCustomId("text_generator")
                .AddTextInput("Prompt", "prompt", TextInputStyle.Paragraph, "Entire city of Copenhagen on fire", 5, 250, true);

            await Context.Interaction.RespondWithModalAsync(modalBuilder.Build());
        }
    }
}
