using Discord;
using Discord.Interactions;

namespace DiscordBot.Models.InteractionModels;

// ReSharper disable once ClassNeverInstantiated.Global
public class EventModel : IModal
{
    [InputLabel("Заголовок")]
    [ModalTextInput("title_input", TextInputStyle.Short, "Red Bear TvT1", 1, 255)]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string TitleInput { get; set; }

    [InputLabel("Описание")]
    [ModalTextInput("description_input", TextInputStyle.Paragraph, "Игра <timestamp>. Сбор в TS RBC за 30 минут до начала...")]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string DescriptionInput { get; set; }

    [InputLabel("Время проведения")]
    [ModalTextInput("time_input", TextInputStyle.Short, "00:00-00.00.0000", 1, 16)]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string TimeInput { get; set; }

    public string Title => "Форма шаблона события";
}