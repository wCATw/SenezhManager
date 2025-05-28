using Discord;
using Discord.Interactions;

namespace DiscordBot.Models.InteractionModels;

public class EventTemplateModel : IModal
{
    [InputLabel("Заголовок")]
    [ModalTextInput("title_input", TextInputStyle.Short, "Red Bear TvT1", 1, 255)]
    public required string TitleInput { get; set; }

    [InputLabel("Описание")]
    [ModalTextInput("description_input", TextInputStyle.Paragraph, "Игра <timestamp>. Сбор в TS RBC за 30 минут до начала...")]
    public required string DescriptionInput { get; set; }

    public string Title => "Форма шаблона события";
}