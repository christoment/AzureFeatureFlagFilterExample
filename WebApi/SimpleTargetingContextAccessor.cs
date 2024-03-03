using Microsoft.FeatureManagement.FeatureFilters;

namespace WebApi;

public class SimpleTargetingContextAccessor(IHttpContextAccessor httpContext) : ITargetingContextAccessor
{
    public ValueTask<TargetingContext> GetContextAsync()
    {
        // Alternative tracking user's session from frontend header - can also pull from Appinsights tracking context
        // httpContext.HttpContext?.Request.Headers.TryGetValue("X-SESSION-ID", out StringValues userId);
        TargetingContext context = new()
        {
            UserId = Guid.NewGuid().ToString(),
            Groups = [],
        };

        return new ValueTask<TargetingContext>(context);
    }
}