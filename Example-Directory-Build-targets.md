<Project>

  <!-- =========================================================
       Repo-wide targets (combined & gated)
       - CPM guard: fail on inline PackageReference versions
       - Codex meta: opt-in, top-level-only, stamp-guarded helpers
       How to enable meta: dotnet build /p:RunCodexMeta=true
       ========================================================= -->

  <PropertyGroup>
    <!-- Repo root and stamps -->
    <RepoRoot>$([System.IO.Path]::GetFullPath('$(MSBuildThisFileDirectory)'))</RepoRoot>
    <!-- Workspace root: go up from server folder (src/X/server) to workspace root -->
    <WorkspaceRoot Condition="'$(WorkspaceRoot)'==''">$([System.IO.Path]::GetFullPath('$(MSBuildThisFileDirectory)..\..\..\'))</WorkspaceRoot>
    <CodexStampDir>$(WorkspaceRoot).agents\logs\meta\stamps</CodexStampDir>

    <!-- Opt-in switch for meta (default: off) -->
    <RunCodexMeta Condition="'$(RunCodexMeta)'==''">false</RunCodexMeta>

    <!-- Treat this as a top-level solution build (not design-time) -->
    <IsTopLevelBuild Condition="'$(IsTopLevelBuild)'=='' and Exists('$(SolutionDir)') and '$(DesignTimeBuild)'!='true'">true</IsTopLevelBuild>
  </PropertyGroup>

  <!-- ================== CPM guard ================== -->
  <Target Name="FailOnInlinePackageVersions"
          BeforeTargets="Restore"
          Condition="'$(DisableInlinePackageVersionGuard)'!='true'">
    <ItemGroup>
      <_WithInlineVersion Include="@(PackageReference)"
                          Condition="'%(PackageReference.Version)' != '' and '%(PackageReference.IsImplicitlyDefined)' != 'true'" />
    </ItemGroup>
    <Message Importance="High"
             Text="Inline PackageReferences detected: @(_WithInlineVersion->'%(Identity) %(Version)', ', ')"
             Condition="'@(_WithInlineVersion)' != ''" />
    <Error Text="Inline PackageReference versions are forbidden. Use Directory.Packages.props."
           Condition="'@(_WithInlineVersion)' != ''" />
  </Target>

  <!-- ============== Codex meta (opt-in) ============== -->

  <Target Name="Codex_EnsureStampDir"
          Condition="'$(RunCodexMeta)'=='true' and '$(IsTopLevelBuild)'=='true'">
    <MakeDir Directories="$(CodexStampDir)" />
  </Target>

  <!-- Project graph (NuGet restore graph) -->
  <Target Name="Codex_ProjectGraph"
          AfterTargets="Build"
          DependsOnTargets="Codex_EnsureStampDir"
          Condition="'$(RunCodexMeta)'=='true' and '$(IsTopLevelBuild)'=='true' and !Exists('$(CodexStampDir)\project.graph.stamp')">
    <Message Importance="High" Text="Codex: Generating project graph..." />
    <Exec Command="dotnet msbuild -nologo -t:GenerateRestoreGraphFile -p:RestoreGraphOutputPath=$(WorkspaceRoot).agents\logs\meta\project.graph.json" />
    <WriteLinesToFile File="$(CodexStampDir)\project.graph.stamp" Lines="$([System.DateTime]::UtcNow)" Overwrite="true" />
  </Target>

  <!-- Types registry / module map (optional tool) -->
  <Target Name="Codex_ModuleMap"
          AfterTargets="Build"
          DependsOnTargets="Codex_EnsureStampDir"
          Condition="'$(RunCodexMeta)'=='true'
                     and '$(IsTopLevelBuild)'=='true'
                     and Exists('$(WorkspaceRoot)\.tools\src\tools\Eleon.TypesScan\Eleon.TypesScan.csproj')
                     and !Exists('$(CodexStampDir)\types.scan.stamp')">
    <Message Importance="High" Text="Codex: Emitting types registry..." />
    <Exec Command="dotnet run --project &quot;$(WorkspaceRoot)\.tools\src\tools\Eleon.TypesScan\Eleon.TypesScan.csproj&quot; -- &quot;$(WorkspaceRoot)&quot;" />
    <WriteLinesToFile File="$(CodexStampDir)\types.scan.stamp" Lines="$([System.DateTime]::UtcNow)" Overwrite="true" />
  </Target>

  <!-- Test generators (optional tools) -->
  <Target Name="Codex_TestGen"
          AfterTargets="Build"
          DependsOnTargets="Codex_EnsureStampDir"
          Condition="'$(RunCodexMeta)'=='true' and '$(IsTopLevelBuild)'=='true'">
    <Message Importance="High" Text="Codex: Running test generators (if present)..." />
    <Exec Command="dotnet run --project &quot;$(WorkspaceRoot)\.tools\src\tools\Eleon.VerifyGen\Eleon.VerifyGen.csproj&quot; -- &quot;$(WorkspaceRoot)&quot;"
          Condition="Exists('$(WorkspaceRoot)\.tools\src\tools\Eleon.VerifyGen\Eleon.VerifyGen.csproj') AND Exists('$(WorkspaceRoot).agents\logs\test-fixtures')" />
    <Exec Command="dotnet run --project &quot;$(WorkspaceRoot)\.tools\src\tools\Eleon.BuildersGen\Eleon.BuildersGen.csproj&quot; -- &quot;$(WorkspaceRoot)&quot;"
          Condition="Exists('$(WorkspaceRoot)\.tools\src\tools\Eleon.BuildersGen\Eleon.BuildersGen.csproj') AND Exists('$(WorkspaceRoot)src')" />
  </Target>

  <!-- API diff (bash-only; skip on Windows if bash missing) -->
  <Target Name="Codex_ApiDiff"
          AfterTargets="Build"
          DependsOnTargets="Codex_EnsureStampDir"
          Condition="'$(RunCodexMeta)'=='true'
                     and '$(IsTopLevelBuild)'=='true'
                     and Exists('$(WorkspaceRoot)scripts\api-diff.sh')
                     and Exists('$(WorkspaceRoot).agents\logs\openapi\openapi.json')
                     and (Exists('C:\Program Files\Git\bin\bash.exe') or Exists('C:\Program Files\Git\usr\bin\bash.exe') or Exists('C:\Windows\System32\bash.exe'))">
    <Message Importance="High" Text="Codex: Running API diff (bash)..." />
    <Exec Command="bash &quot;$(WorkspaceRoot)scripts/api-diff.sh&quot; &quot;$(WorkspaceRoot).agents\logs\openapi\openapi.json&quot; &quot;$(WorkspaceRoot).agents\logs\openapi\openapi.snapshot.json&quot;" />
  </Target>

  <!-- Friendly guidance (pure MSBuild; always safe) -->
  <Target Name="Codex_Guidance" AfterTargets="Build" Condition="'$(IsTopLevelBuild)'=='true'">
    <Message Importance="High" Text="ELEONB101: OpenAPI snapshot not found. Create .agents/logs/openapi/openapi.snapshot.json"
             Condition="!Exists('$(WorkspaceRoot).agents\logs\openapi\openapi.snapshot.json') AND Exists('$(WorkspaceRoot).agents\logs\openapi\openapi.json')" />
    <Message Importance="High" Text="ELEONB102: /meta/init missing. Add docs/CODEX_INIT.md or InitController"
             Condition="!Exists('$(WorkspaceRoot)docs/CODEX_INIT.md')" />
    <Message Importance="High" Text="ELEONB103: Types registry missing. Ensure .tools\src\tools\Eleon.TypesScan is present"
             Condition="!Exists('$(WorkspaceRoot).agents\logs\meta\types.registry.json')" />
  </Target>

</Project>
