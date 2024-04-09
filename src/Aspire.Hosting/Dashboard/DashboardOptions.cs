// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Aspire.Hosting.Dcp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Aspire.Hosting.Dashboard;

internal class DashboardOptions
{
    public string? DashboardPath { get; set; }
    public string? DashboardUrl { get; set; }
    public string? DashboardToken { get; set; }
    public string? OtlpEndpointUrl { get; set; }
    public string? OtlpApiKey { get; set; }
    public string AspNetCoreEnvironment { get; set; } = "Production";
}

internal class ConfigureDefaultDashboardOptions(IConfiguration configuration, IOptions<DcpOptions> dcpOptions) : IConfigureOptions<DashboardOptions>
{
    public void Configure(DashboardOptions options)
    {
        options.DashboardPath = dcpOptions.Value.DashboardPath;
        options.DashboardUrl = configuration["ASPNETCORE_URLS"];
        options.DashboardToken = configuration["AppHost:BrowserToken"];

        options.OtlpEndpointUrl = configuration["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"];
        options.OtlpApiKey = configuration["AppHost:OtlpApiKey"];

        options.AspNetCoreEnvironment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
    }
}

internal class ValidateDashboardOptions : IValidateOptions<DashboardOptions>
{
    public ValidateOptionsResult Validate(string? name, DashboardOptions options)
    {
        var builder = new ValidateOptionsResultBuilder();

        if (string.IsNullOrEmpty(options.DashboardUrl))
        {
            builder.AddError("Failed to configure dashboard resource because ASPNETCORE_URLS environment variable was not set.");
        }

        if (string.IsNullOrEmpty(options.OtlpEndpointUrl))
        {
            builder.AddError("Failed to configure dashboard resource because DOTNET_DASHBOARD_OTLP_ENDPOINT_URL environment variable was not set.");
        }

        if (options.DashboardUrl == options.OtlpEndpointUrl)
        {
            builder.AddError($"Failed to configure dashboard resource because ApplicationUrl and DOTNET_DASHBOARD_OTLP_ENDPOINT_URL are both set to {options.DashboardUrl}.");
        }

        return builder.Build();
    }
}
