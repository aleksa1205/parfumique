using Microsoft.AspNetCore.Mvc.Filters;

namespace FragranceRecommendation.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiresRoleAttribute(Roles role) : Attribute, IAuthorizationFilter
{
    private readonly string _claimName = "Role";
    private readonly string _claimValue = ((int)role).ToString();

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.HasClaim(_claimName, _claimValue))
            context.Result = new ForbidResult();
    }
}