using Catalog.Core.SharedKernel;

namespace Catalog.Core.AppSettings;

public sealed class CacheOptions : IAppOptions
{
    static string IAppOptions.ConfigSectionPath => nameof(CacheOptions);

    public int AbsoluteExpirationInHours { get; private init; }
    public int SlidingExpirationInSeconds { get; private init; }
}