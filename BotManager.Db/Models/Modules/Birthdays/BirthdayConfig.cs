using System.ComponentModel.DataAnnotations;
using BotManager.Db.Models.Abstract;
using Demolite.Db.Models;

namespace BotManager.Db.Models.Modules.Birthdays;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class BirthdayConfig : AbstractGuildConfig
{
	public ulong PingChannelId { get; set; }
	public List<Birthday> Birthdays { get; set; } = [];
}