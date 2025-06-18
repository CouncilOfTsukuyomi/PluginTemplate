using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PluginManager.Core.Enums;
using PluginManager.Core.Models;
using PluginManager.Core.Plugins;

namespace SamplePlugin;
public class SamplePlugin : BaseModPlugin
{
    private readonly HttpClient _httpClient;
    
    // Configuration values
    private string _apiKey = string.Empty;
    private string _baseUrl = string.Empty;

    #region Plugin Metadata - CUSTOMIZE THESE VALUES
    
    public override string PluginId => "your-plugin-id";
    public override string DisplayName => "Sample Plugin";
    public override string Description => "Description of what your plugin does"; 
    public override string Version => "1.0.0";
    public override string Author => "Your Name";
    
    #endregion

    public SamplePlugin(ILogger<SamplePlugin> logger, HttpClient httpClient) 
        : base(logger)
    {
        _httpClient = httpClient;
    }

    public override async Task InitializeAsync(Dictionary<string, object> configuration)
    {
        // Handle plugin configuration
        // API key is not needed, just an example
        if (configuration.TryGetValue("ApiKey", out var apiKey) && apiKey is string keyStr)
        {
            _apiKey = keyStr;
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        }

        if (configuration.TryGetValue("BaseUrl", out var baseUrl) && baseUrl is string urlStr)
        {
            _baseUrl = urlStr;
        }

        Logger.LogInformation("Plugin initialized - BaseUrl: {BaseUrl}, HasApiKey: {HasKey}", 
            _baseUrl, !string.IsNullOrEmpty(_apiKey));

        // TODO: Add any additional initialization logic here
    }

    public override async Task<List<PluginMod>> GetRecentModsAsync()
    {
        // Handle caching
        var configHash = GetConfigurationHash(new Dictionary<string, object> 
        { 
            ["ApiKey"] = _apiKey,
            ["BaseUrl"] = _baseUrl 
        });
        InvalidateCacheOnConfigChange(configHash);

        var cachedData = LoadCacheFromFile();
        if (cachedData != null && cachedData.ExpirationTime > DateTimeOffset.Now)
        {
            Logger.LogDebug("Using cached data, expires: {ExpirationTime}", cachedData.ExpirationTime);
            return cachedData.Mods;
        }

        Logger.LogInformation("Fetching fresh mod data from source...");

        // Fetch mods from your source
        var mods = await FetchModsFromSource();

        // Cache the results
        var cacheData = new PluginCacheData
        {
            Mods = mods,
            ExpirationTime = DateTimeOffset.Now.Add(CacheDuration),
            PluginId = PluginId
        };

        SaveCacheToFile(cacheData);
        return mods;
    }

    private async Task<List<PluginMod>> FetchModsFromSource()
    {
        var mods = new List<PluginMod>();

        try
        {
            // TODO: Replace this section with your actual mod fetching logic
            
            // Example API call:
            // var response = await _httpClient.GetStringAsync($"{_baseUrl}/api/recent-mods");
            // var data = JsonConvert.DeserializeObject<YourApiResponse>(response);
            
            // Example web scraping:
            // var html = await _httpClient.GetStringAsync($"{_baseUrl}/recent-mods");
            // var doc = new HtmlDocument();
            // doc.LoadHtml(html);
            // var modNodes = doc.DocumentNode.SelectNodes("//div[@class='mod-item']");

            // For now, return sample data
            mods.Add(new PluginMod
            {
                Name = NormalizeModName("Sample Mod Name"),
                Publisher = "Sample Author",
                Type = "Equipment",
                ImageUrl = "https://via.placeholder.com/300x200",
                ModUrl = "https://example.com/mod/sample",
                DownloadUrl = await GetModDownloadLinkAsync("https://example.com/mod/sample") ?? "",
                Gender = ModGender.Unisex,
                PluginSource = PluginId,
                AdditionalProperties = new Dictionary<string, object>
                {
                    ["SampleProperty"] = "Sample Value"
                }
            });

            Logger.LogInformation("Fetched {Count} mods from source", mods.Count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to fetch mods from source");
        }

        return mods;
    }

    protected override async Task<string?> GetModDownloadLinkAsync(string modUrl)
    {
        try
        {
            // TODO: Implement your download link extraction logic
            
            // Example: Parse the mod page to find download link
            // var html = await _httpClient.GetStringAsync(modUrl);
            // var doc = new HtmlDocument();
            // doc.LoadHtml(html);
            // var downloadNode = doc.DocumentNode.SelectSingleNode("//a[contains(@class, 'download-link')]");
            // return downloadNode?.GetAttributeValue("href", null);

            // For now, return placeholder
            return "https://example.com/download/sample-mod.zip";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to get download link for {ModUrl}", modUrl);
            return null;
        }
    }

    public override async ValueTask DisposeAsync()
    {
        // TODO: Add any cleanup logic here
        _httpClient?.Dispose();
        await base.DisposeAsync();
    }
}