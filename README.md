# Plugin Template

A template for creating plugins using the PluginManager.Core framework. Clone this repository and customise it to create your own plugin.

## Getting Started

1. **Clone and rename**
   ```bash
   git clone https://github.com/CouncilOfTsukuyomi/PluginTemplate.git YourPluginName
   cd YourPluginName
   ```

2. **Follow the customisation steps below**

3. **Test your plugin**
   ```bash
   dotnet build
   dotnet test
   ```

## Customisation Steps

### 1. Rename the Project

1. **Rename the folder** `SamplePlugin` → `YourPluginName`

2. **Update `PluginTemplate.sln`**
   ```diff
   - Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "SamplePlugin", "SamplePlugin\SamplePlugin.csproj"
   + Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "YourPluginName", "YourPluginName\YourPluginName.csproj"
   ```

3. **Update the `.csproj` file**
   ```xml
   <PropertyGroup>
       <AssemblyName>YourPluginName</AssemblyName>
       <RootNamespace>YourPluginName</RootNamespace>
   </PropertyGroup>
   ```

### 2. Update Plugin Metadata

**In `YourPluginName.cs`**, update the metadata section:
```csharp 
#region Plugin Metadata - CUSTOMIZE THESE VALUES
public override string PluginId => "your-unique-plugin-id"; public override string DisplayName => "Your Plugin Display Name"; public override string Description => "What your plugin does"; public override string Version => "1.0.0"; public override string Author => "Your Name";
#endregion
```


### 3. Update Plugin Manifest

**In `plugin-manifest.json`**, update all the fields:
```json 
{ 
  "pluginId": "your-unique-plugin-id", 
  "displayName": "Your Plugin Display Name", 
  "description": "Detailed description of your plugin", 
  "author": "Your Name", 
  "website": "[https://your-website.com](https://your-website.com)", 
  "repositoryUrl": "[https://github.com/yourusername/YourPluginName](https://github.com/yourusername/YourPluginName)", 
  "assemblyName": "YourPluginName.dll", 
  "mainClass": "YourPluginName.YourPluginName", 
  "tags": ["your", "plugin", "tags"], 
  "category": "YourCategory"
}
```

### 4. Update Dependencies

**Add your required packages** to both places:

1. **In `YourPluginName.csproj`**:
   ```xml
   <ItemGroup>
       <PackageReference Include="YourPackage" Version="1.0.0" />
   </ItemGroup>
   ```

2. **In `plugin-manifest.json`**:
   ```json
   "dependencies": [
     {
       "name": "YourPackage",
       "version": "1.0.0"
     }
   ]
   ```

### 5. Configure Plugin Settings

**In `plugin-manifest.json`**, update the configuration schema:
```json 
"configuration": {
   "schema": {
      "type": "object",
         "properties": {
         "ExampleSetting": {
            "type": "string",
            "default": "Hello World",
            "title": "Example Setting",
            "description": "An example configuration setting for the sample plugin"
         },
         "EnableLogging": {
            "type": "boolean",
            "default": true,
            "title": "Enable Logging",
            "description": "Whether to enable detailed logging for this plugin"
         },
         "MaxItems": {
            "type": "integer",
            "default": 100,
            "minimum": 1,
            "maximum": 1000,
            "title": "Maximum Items",
            "description": "Maximum number of items to process"
         }
      }
   }
}
```


### 6. Implement Your Plugin Logic

**In `YourPluginName.cs`**, replace the sample implementation:

1. **Update `InitializeAsync`** to handle your configuration
2. **Update `FetchModsFromSource`** with your mod fetching logic
3. **Update `GetModDownloadLinkAsync`** with your download link extraction

### 7. Update GitHub Workflows

**In both workflow files** (`.github/workflows/`), update the packaging paths:
```yaml
- name: Package Plugin
  run: |
      mkdir -p plugin-package
      cp SamplePlugin/bin/Release/net9.0/* plugin-package/
      cd plugin-package
      zip -r ../SamplePlugin-${{ github.ref_name }}.zip .
```
And update the release asset names:
```yaml 
asset_name: YourPluginName-${{ github.ref_name }}.zip
```


### 8. Set Up GitHub Repository (This step can be skipped if you want to manually push tags)

1. **Create repository secrets** for GitHub App authentication:
   - `APP_PRIVATE_KEY`: Your GitHub App's private key

2. **Create repository variables**:
   - `APP_ID`: Your GitHub App's ID

If you are after manually pushing tag, remove these lines in the workflow
```yaml
- name: Generate GitHub App Token
  uses: actions/create-github-app-token@v1
  id: app-token
  with:
     app-id: ${{ vars.APP_ID }}
     private-key: ${{ secrets.APP_PRIVATE_KEY }}

token: ${{ steps.app-token.outputs.token }}

- name: "🏷️ Create and Push tag"
  if: github.event_name == 'workflow_dispatch'
  uses: EndBug/latest-tag@latest
  with:
      tag-name: ${{ github.event.inputs.version }}
      description: "Release ${{ github.event.inputs.version }}"
  env:
      GITHUB_TOKEN: ${{ steps.app-token.outputs.token }}
```

## Release Your Plugin (This is only for people manually pushing tags)

1. **Update version** in both `plugin-manifest.json` and your plugin code
2. **Push a version tag**:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
3. **The release workflow** will automatically build and publish your plugin

## Key Files to Customise

- `YourPluginName/YourPluginName.cs` - Main plugin implementation
- `YourPluginName/plugin-manifest.json` - Plugin metadata and configuration
- `YourPluginName/YourPluginName.csproj` - Project dependencies
- `.github/workflows/*.yml` - CI/CD workflows
- `PluginTemplate.sln` - Solution file


## Examples of Plugins
- [XMA Plugin](https://github.com/CouncilOfTsukuyomi/XMA-Plugin)
