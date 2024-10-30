using System;
using System.ComponentModel.DataAnnotations;
using Catalog.Core.SharedKernel;

namespace Catalog.Core.AppSettings;

public sealed class ConnectionOptions : IAppOptions
{
    static string IAppOptions.ConfigSectionPath => "ConnectionStrings";

    [Required]
    public string SqlConnection { get; private init; }

    [Required]
    public string NoSqlConnection { get; private init; }

    [Required]
    public string CacheConnection { get; private init; }

    public bool CacheConnectionInMemory() =>
        CacheConnection.Equals("InMemory", StringComparison.InvariantCultureIgnoreCase);
}