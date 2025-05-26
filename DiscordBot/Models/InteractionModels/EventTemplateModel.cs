using Discord.Interactions;

namespace DiscordBot.Models.InteractionModels;

public class EventTemplateModel : IModal
{
    [InputLabel("Заголовок")]
    [ModalTextInput("title_input", minLength: 5, maxLength: 100, placeholder: "Заголовок")]
    public string TitleInput { get; set; } = default!;

    public string Title => "Форма шаблона события";
}