using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PluginManager.Core.Interfaces;
using PluginManager.Core.Models;
using PluginManager.Core.Plugins;

namespace SamplePlugin;

public class SamplePlugin : BaseModPlugin, IModPlugin
{
    public override string PluginId => "sample-plugin";
    public override string DisplayName => "Sample Plugin";
    public override string Description => "A simple sample plugin demonstrating the plugin architecture";
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

        Logger.LogInformation("Sample plugin initialized successfully");
    }

    public override async Task<List<PluginMod>> GetRecentModsAsync()
    {
        Logger.LogDebug("Getting sample mods");

        // Return some sample mod data for demonstration
        var sampleMods = new List<PluginMod>
        {
            new PluginMod
            {
                Name = "Sample Mod 1",
                Publisher = "Sample Author",
                ImageUrl = "https://example.com/sample1.jpg",
                ModUrl = "https://example.com/mod1",
                DownloadUrl = "https://example.com/download1",
                PluginSource = PluginId,
                UploadDate = DateTime.Now.AddDays(-1)
            },
            new PluginMod
            {
                Name = "Sample Mod 2",
                Publisher = "Another Author",
                ImageUrl = "https://example.com/sample2.jpg",
                ModUrl = "https://example.com/mod2",
                DownloadUrl = "https://example.com/download2",
                PluginSource = PluginId,
                UploadDate = DateTime.Now.AddDays(-1)
            }
        };

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
}