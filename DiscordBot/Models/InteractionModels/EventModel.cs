using Discord;
using Discord.Interactions;

namespace DiscordBot.Models.InteractionModels;

public class EventModel : IModal
{
    [InputLabel("Заголовок")]
    [ModalTextInput("heading_input", minLength: 1, maxLength: 256, placeholder: "Операция GREYTIDE")]
    public string Heading { get; set; } = default!;

    [InputLabel("Информация")]
    [ModalTextInput("description_input", minLength: 1, style: TextInputStyle.Paragraph, maxLength: 4096)]
    public string Description { get; set; } = default!;

    [InputLabel("Ссылка на превью")]
    [ModalTextInput("preview_input", placeholder: "Ссылка на превью. Поставьте минус если без превью.", maxLength: 300)]
    public string ImageUrl { get; set; } = default!;

    [InputLabel("Ссылка на лого")]
    [ModalTextInput("logo_input", placeholder: "Ссылка на лого. Поставьте минус если без лого.", maxLength: 300)]
    public string ThumbnailUrl { get; set; } = default!;

    public string Title => "Форма для нового события";
}