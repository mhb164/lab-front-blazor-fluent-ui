using FrontBlazorFluentUI.Auth;
using FrontBlazorFluentUI.Dto;
using FrontBlazorFluentUI.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FrontBlazorFluentUI.Services;

public class UserContext
{
    private const string UnknownTitle = "Unknown";
    private static readonly ClaimsPrincipal UnknownPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
    private readonly ILogger? _logger;

    private ContextOverview? _serverOverview;
    private ClientUser? _user;
    
    public UserContext(ILogger<UserContext>? logger)
    {
        _logger = logger;
        _logger?.LogInformation("UserContext> Created!");
    }

    public ContextOverview? Overview => _serverOverview;
    public ClaimsPrincipal Principal { get; set; } = UnknownPrincipal;
    public ClientUser? User => _user;
    public event Action? UserChanged;
    public bool ServerAccessible => _serverOverview != null;

    public AuthenticationState AuthenticationState => new AuthenticationState(Principal);
    public bool Authenticated => User != null;
    public bool ForceChangeLocalPassword => User?.ChangeLocalPasswordRequired == true && _user?.Auth?.Type == AuthType.Locally;
    public string Title => User?.Nickname ?? UnknownTitle;

    public void Reset(ContextOverview overview, string accessToken)
    {
        _serverOverview = overview;
        ResetInternal(CreateClientUserFromToken(accessToken, _logger));
        _logger?.LogInformation("UserContext> Reset by accessToken (Nickname: {Nickname})!", User?.Nickname);

    }

    public void Reset(ContextOverview? overview)
    {
        _serverOverview = overview;
        ResetInternal(null);
        _logger?.LogInformation("UserContext> Reset to null!");
    }

    private void ResetInternal(ClientUser? user = null)
    {
        _user = user;
        NotifyUserChanged();
        Principal = CreatePrincipal(user);
    }

    public void NotifyUserChanged()
    {
        if (UserChanged == null)
            return;

        Task.Run(UserChanged.Invoke).ConfigureAwait(false);
    }

    private static ClaimsPrincipal CreatePrincipal(ClientUser? user)
    {
        if (user is null)
            return UnknownPrincipal;

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Nickname),
        };

        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        identity.AddClaims(user.Permits.FullPermissions.Select(p => new Claim(ClaimTypes.Role, p)));

        return new ClaimsPrincipal(identity);
    }

    private static ClientUser? CreateClientUserFromToken(string accessToken, ILogger? logger)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        try
        {
            var handler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = handler.ReadJwtToken(accessToken);

            if (jwtSecurityToken == null)
                return null;

            var token = new ClientToken(id: jwtSecurityToken.GetClaimGuid(ClaimNames.TokenId),
              audience: jwtSecurityToken.Audiences.FirstOrDefault(),
              issuer: jwtSecurityToken.Issuer,
              issuedAt: jwtSecurityToken.IssuedAt,
              expirationTime: jwtSecurityToken.GetClaimTime(ClaimNames.ExpirationTime));

            var auth = new ClientAuth(typeText: jwtSecurityToken.GetClaimValue(ClaimNames.AuthType),
                username: jwtSecurityToken.GetClaimValue(ClaimNames.AuthUsername),
                time: jwtSecurityToken.GetClaimTime(ClaimNames.AuthTime));

            var user = new ClientUser(token: token, auth,
                username: jwtSecurityToken.GetClaimValue(ClaimNames.Username),
                fullname: jwtSecurityToken.GetClaimValue(ClaimNames.Fullname),
                firstname: jwtSecurityToken.GetClaimValue(ClaimNames.Firstname),
                lastname: jwtSecurityToken.GetClaimValue(ClaimNames.Lastname),
                nickname: jwtSecurityToken.GetClaimValue(ClaimNames.Lastname),
                locallyAvailable: jwtSecurityToken.GetClaimBoolean(ClaimNames.LocallyAvailable),
                changeLocalPasswordRequired: jwtSecurityToken.GetClaimBoolean(ClaimNames.ChangeLocalPasswordRequired),
                readOnly: jwtSecurityToken.GetClaimBoolean(ClaimNames.ReadOnly),
                permits: new UserPermits(ReadPermits(jwtSecurityToken.Claims)));

            return user;
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "UserContext> Extract from JwtToken {AccessToken} failed {Message}", accessToken, ex.Message);

            return null;
        }
    }

    private static IEnumerable<UserPermit> ReadPermits(IEnumerable<Claim> claims)
    {
        foreach (var claim in claims)
        {
            var permit = UserPermit.FromClaim(claim.Type, claim.Value);

            if (permit is null)
                continue;

            yield return permit;
        }
    }
}

