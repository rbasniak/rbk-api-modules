# Centralized Package Management

This workspace has been configured to use **Directory.Build.props** for common project properties and **Central Package Management (CPM)** for NuGet package version management.

## Files Created

### 1. Directory.Build.props
Located at the root of the repository, this file centralizes common MSBuild properties that apply to all projects:

- **TargetFramework**: `net10.0` (default for most projects)
- **ImplicitUsings**: `enable`
- **Nullable**: `enable`
- **LangVersion**: `latest`

Special handling for Analyzer projects:
- Projects with names containing "Analysers" automatically target `netstandard2.0`

### 2. Directory.Packages.props
Located at the root of the repository, this file manages all NuGet package versions centrally using **Central Package Management**.

**Benefits:**
- Single source of truth for all package versions
- Easier to maintain and update packages across the solution
- Prevents version conflicts between projects
- Simplified `.csproj` files

**Packages managed:**
- ASP.NET Core packages (10.0.0)
- Entity Framework Core packages (10.0.0)
- Testing packages (TUnit, Moq, Shouldly, Testcontainers)
- Code Analysis packages (Microsoft.CodeAnalysis.*)
- Third-party packages (FluentValidation, RabbitMQ.Client, ImageSharp, etc.)

## Changes to .csproj Files

All project files have been updated:

1. **Removed common PropertyGroup elements** - These are now inherited from `Directory.Build.props`
2. **Removed Version attributes** from all `<PackageReference>` elements
3. **Version management** is now centralized in `Directory.Packages.props`

### Example Before:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="FluentValidation" Version="12.1.0" />
  </ItemGroup>
</Project>
```

### Example After:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <PackageReference Include="FluentValidation" />
  </ItemGroup>
</Project>
```

## How to Add a New Package

1. Add the package reference to your `.csproj` file **without** a version:
   ```xml
   <PackageReference Include="NewPackage.Name" />
   ```

2. Add the version definition to `Directory.Packages.props`:
   ```xml
   <PackageVersion Include="NewPackage.Name" Version="1.2.3" />
   ```

## How to Update Package Versions

Simply update the version in `Directory.Packages.props`, and all projects using that package will automatically use the new version.

## Project-Specific Properties

Projects can still override or add specific properties in their individual `.csproj` files:
- Custom build properties
- Package-specific metadata (like `PrivateAssets`, `IncludeAssets`)
- Project-specific settings (like `IsPackable`, `RootNamespace`)

## Verification

The solution has been successfully restored and built with these changes:
```
dotnet restore
dotnet build
```

Both commands completed successfully, confirming that the centralized configuration is working correctly.
