using System.Security.Claims;
using BotManager.Core.Helpers;
using Microsoft.AspNetCore.Components;

namespace BotManager.WebUi.Components.Authorization;

public partial class AuthorizeAnyAdminInline : ComponentBase
{
	[Parameter]
	public RenderFragment? Authorized { get; set; }

	[Parameter]
	public RenderFragment? NotAuthorized { get; set; }

	private bool _canAccess;

	protected override async Task OnInitializedAsync()
	{
		var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

		if (!authState.User.TryGetUserId(out var ulongId))
			_canAccess = false;

		else
			_canAccess = await AuthorizationService.HasAnyAccessAsync(ulongId);
	}
}