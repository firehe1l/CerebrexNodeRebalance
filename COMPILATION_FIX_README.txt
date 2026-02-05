# HOW TO FIX MOD COMPILATION (CerebrexRebalance)

If you are encountering issues compiling this mod (hangs, missing types, API errors), follow these steps. Do not waste time debugging `csc.exe` hangs.

## 1. The Core Problem
*   Direct usage of `csc.exe` often hangs indefinitely or fails to resolve references properly in this environment.
*   RimWorld 1.6+ has API changes (specifically `WorldFloodFiller`) that cause build failures if using older code patterns.
*   `dotnet build` is the reliable way to compile, but requires a correctly configured `.csproj`.

## 2. The Solution: Use `dotnet build`

### A. Ensure .csproj is Correct
Your `Source/CerebrexRebalance.csproj` must look like this (minimal working example):

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <LangVersion>latest</LangVersion>
    <AssemblyName>CerebrexRebalance</AssemblyName>
    <RootNamespace>CerebrexRebalance</RootNamespace>
    <OutputPath>..\Assemblies\</OutputPath>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
  </PropertyGroup>

  <ItemGroup>
    <!-- RimWorld References -->
    <Reference Include="Assembly-CSharp">
      <HintPath>C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\Assembly-CSharp.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine">
      <HintPath>C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.dll</HintPath>
    </Reference>
    <Reference Include="UnityEngine.CoreModule">
      <HintPath>C:\Program Files (x86)\Steam\steamapps\common\RimWorld\RimWorldWin64_Data\Managed\UnityEngine.CoreModule.dll</HintPath>
    </Reference>
    <!-- Harmony Reference (Local) -->
    <Reference Include="0Harmony">
      <HintPath>..\Assemblies\0Harmony.dll</HintPath>
      <Private>False</Private>
    </Reference>
  </ItemGroup>
</Project>
```

### B. Code Adjustments (Crucial!)
In **`Source/ArchotechQuestGiver.cs`**, you cannot use `Find.WorldFloodFiller` directly as the API has changed/is unstable for direct access in some contexts.
*   **Fix**: Use `Traverse` (Harmony) or Reflection to safely access it, OR simpler tile finding logic.
*   **Warning**: `FloodFill` delegates have strict signature matching. If compilation fails with "Delegate mismatch", disable the fallback tile search logic and rely on `TileFinder.RandomSettlementTileFor`.

### C. Build Command
Run this command from the project root:
```powershell
dotnet build Source\CerebrexRebalance.csproj -v normal
```

### D. Deployment
After a successful build, copy the DLL to the 1.6 folder:
```powershell
copy /Y Assemblies\CerebrexRebalance.dll 1.6\Assemblies\CerebrexRebalance.dll
```

## Summary
1.  Ignore `csc.exe`.
2.  Use `dotnet build`.
3.  Ensure `.csproj` targets `net472`.
4.  Disable `WorldFloodFiller` logic if it causes API referencing errors.
