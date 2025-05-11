using Discord.Interactions;

namespace DiscordBot.Models;

public class ApplicationModel : IModal
{
    [InputLabel("Ваш Позывной?")]
    [ModalTextInput("nick_input", placeholder: "Ivan", maxLength: 10)]
    public string Nick { get; set; } = default!;

    [InputLabel("Ваш SteamID64?")]
    [ModalTextInput("steamid_input", placeholder: "0000000000000000000", maxLength: 70)]
    public string SteamId { get; set; } = default!;

    [InputLabel("От куда вы о нас узнали?")]
    [ModalTextInput("info_input", placeholder: "Пришел с проекта N...", maxLength: 100)]
    public string Info { get; set; } = default!;

    [InputLabel("Есть ли среди отряда ваши друзья? Кто? (Введите их позывные)")]
    [ModalTextInput("friends_input", placeholder: "Мои друзья А и Б...", maxLength: 50)]
    public string Friends { get; set; } = default!;

    public string Title => "Форма заявки на вступление";
}