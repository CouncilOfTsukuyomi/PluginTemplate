using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PluginManager.Core.Interfaces;
using PluginManager.Core.Models;
using PluginManager.Core.Plugins;
using SamplePlugin.Enums;

namespace SamplePlugin;

public class SamplePlugin : BaseModPlugin, IModPlugin
{
    private List<SampleModType> _modTypes = new();
    private string _sortBy = "upload_date";

    public override string PluginId => "sample-plugin";
    public override string DisplayName => "Sample Plugin";
    public override string Description => "A simple sample plugin demonstrating enum usage and plugin architecture";
    public override string Version => "1.0.0";
    public override string Author => "Plugin Developer";

    // Simple parameterless constructor for isolated loader
    public SamplePlugin() : base(NullLogger.Instance)
    {
    }

    // Constructor with logger for dependency injection
    public SamplePlugin(ILogger logger) : base(logger, TimeSpan.FromMinutes(30))
    {
    }

    public override async Task InitializeAsync(Dictionary<string, object> configuration)
    {
        Logger.LogInformation("Initializing Sample plugin");

        // Example configuration loading
        if (configuration.TryGetValue("SampleSetting", out var sampleValue))
        {
            Logger.LogInformation($"Sample setting loaded: {sampleValue}");
        }

        // Example of parsing enum array from configuration
        if (configuration.TryGetValue("ModTypes", out var modTypes))
        {
            var parsedTypes = ParseModTypes(modTypes);
            if (parsedTypes.Any())
            {
                _modTypes = parsedTypes;
                Logger.LogInformation($"Mod types filter set to: {string.Join(", ", _modTypes)}");
            }
            else
            {
                Logger.LogInformation("ModTypes could not be parsed or is empty - will fetch all mod types");
            }
        }

        // Example of string configuration with validation
        if (configuration.TryGetValue("SortBy", out var sortBy))
        {
            var sortByString = sortBy.ToString();
            var validSortOptions = new[] { "upload_date", "name", "popularity" };
            if (validSortOptions.Contains(sortByString))
            {
                _sortBy = sortByString;
                Logger.LogInformation($"Sort by set to: {_sortBy}");
            }
            else
            {
                Logger.LogWarning($"Invalid SortBy value: '{sortByString}' - using default: {_sortBy}");
            }
        }

        Logger.LogInformation("Sample plugin initialized successfully");
    }

    public override async Task<List<PluginMod>> GetRecentModsAsync()
    {
        Logger.LogDebug("Getting sample mods");

        // Return sample mod data for demonstration, filtering by mod types if specified
        var sampleMods = new List<PluginMod>();

        // Create sample mods for each type
        foreach (var modType in Enum.GetValues<SampleModType>())
        {
            // Skip if this type is not in our filter (when filter is specified)
            if (_modTypes.Any() && !_modTypes.Contains(modType))
            {
                continue;
            }

            sampleMods.Add(new PluginMod
            {
                Name = $"Sample {modType} Mod",
                Publisher = "Sample Author",
                ImageUrl = $"https://example.com/sample-{modType.ToString().ToLower()}.jpg",
                ModUrl = $"https://example.com/mod-{modType.ToString().ToLower()}",
                DownloadUrl = $"https://example.com/download-{modType.ToString().ToLower()}",
                PluginSource = PluginId,
                UploadDate = DateTime.Now.AddDays(-Random.Shared.Next(1, 10))
            });
        }

        Logger.LogInformation($"Retrieved {sampleMods.Count} sample mods");
        return sampleMods;
    }

    protected override async Task<string?> GetModDownloadLinkAsync(string modUrl)
    {
        // Simple implementation that just returns the mod URL as download URL
        // In a real plugin, this would parse the mod page to extract the actual download link
        Logger.LogDebug($"Getting download link for: {modUrl}");
        
        // Simulate some async work
        await Task.Delay(100);
        
        return modUrl + "/download";
    }

    protected override async Task OnDisposingAsync()
    {
        Logger.LogInformation("Cleaning up Sample plugin resources...");
        
        // Add any custom cleanup logic here
        // For example: dispose HTTP clients, close file handles, etc.
        
        Logger.LogInformation("Sample plugin resources cleaned up successfully");
    }

    /// <summary>
    /// Parse ModTypes from configuration
    /// </summary>
    private List<SampleModType> ParseModTypes(object modTypes)
    {
        var parsedTypes = new List<SampleModType>();
        
        try
        {
            if (modTypes is System.Text.Json.JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    parsedTypes = jsonElement.EnumerateArray()
                        .Where(element => element.ValueKind == System.Text.Json.JsonValueKind.Number)
                        .Select(element => element.GetInt32())
                        .Where(value => Enum.IsDefined(typeof(SampleModType), value))
                        .Select(value => (SampleModType)value)
                        .ToList();
                }
            }
            else if (modTypes is IEnumerable<object> typesArray)
            {
                parsedTypes = typesArray
                    .Select(t => t?.ToString())
                    .Where(t => !string.IsNullOrEmpty(t) && int.TryParse(t, out _))
                    .Select(int.Parse)
                    .Where(value => Enum.IsDefined(typeof(SampleModType), value))
                    .Select(value => (SampleModType)value)
                    .ToList();
            }
            else if (modTypes is string modTypesString)
            {
                parsedTypes = modTypesString.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => int.TryParse(s, out _))
                    .Select(int.Parse)
                    .Where(value => Enum.IsDefined(typeof(SampleModType), value))
                    .Select(value => (SampleModType)value)
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to parse ModTypes configuration");
        }

        return parsedTypes;
    }
}