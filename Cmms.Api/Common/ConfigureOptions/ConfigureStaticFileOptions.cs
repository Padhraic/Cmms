namespace Cmms.Api.Common.ConfigureOptions;

using Cmms.Api.Common.Constants;
using Cmms.Api.Common.Options;
using Boxed.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

/// <summary>
/// Configures the static files middleware to add the Cache-Control and Pragma HTTP headers. The cache duration is
/// controlled from configuration.
/// See http://andrewlock.net/adding-cache-control-headers-to-static-files-in-asp-net-core/.
/// </summary>
public class ConfigureStaticFileOptions(CacheProfileOptions cacheProfileOptions) : IConfigureOptions<StaticFileOptions>
{
    private readonly CacheProfile? cacheProfile = cacheProfileOptions
            .Where(x => string.Equals(x.Key, CacheProfileName.StaticFiles, StringComparison.Ordinal))
            .Select(x => x.Value)
            .SingleOrDefault();

    /// <summary>
    /// Configures the static files middleware to add the Cache-Control and Pragma HTTP headers. The cache duration is
    /// </summary>
    /// <param name="options"></param>
    public void Configure(StaticFileOptions options) =>
        options.OnPrepareResponse = this.OnPrepareResponse;

    /// <summary>
    /// Adds the Cache-Control and Pragma HTTP headers. The cache duration is controlled from configuration.
    /// See http://andrewlock.net/adding-cache-control-headers-to-static-files-in-asp-net-core/.
    /// </summary>
    private void OnPrepareResponse(StaticFileResponseContext context)
    {
        if (this.cacheProfile is not null)
        {
            context.Context.ApplyCacheProfile(this.cacheProfile);
        }
    }
}
