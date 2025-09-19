using System.Security.Claims;
using BotManager.Core.Helpers;
using Microsoft.AspNetCore.Components;

namespace BotManager.WebUi.Components.Authorization;

public partial class AuthorizeBot : ComponentBase
{
	[Parameter, EditorRequired]
	public required string BotId { get; set; }

	[Parameter]
	public RenderFragment? Authorized { get; set; }

	[Parameter]
	public RenderFragment? NotAuthorized { get; set; }

	private bool _canAccess;
	private bool _isLoading = true;

	protected override async Task OnInitializedAsync()
	{
		var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

		if (!authState.User.TryGetUserId(out var ulongId))
			_canAccess = false;

		else
			_canAccess = await AuthorizationService.HasBotAccessAsync(ulongId, BotId);

		_isLoading = false;
	}

	protected override async Task OnParametersSetAsync()
	{
		if (!string.IsNullOrEmpty(BotId))
			await OnInitializedAsync();
	}
}