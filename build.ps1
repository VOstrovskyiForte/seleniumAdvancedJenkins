#Requires -Version 5.0

param
(
    [Parameter()]
    [String[]] $TaskList = @("RestorePackages", "Build", "CopyArtifacts"),
    
    [Parameter()]
	[String] $Configuration = "Debug",
	
	[Parameter()]
	[String] $Platform = "Any CPU",
	
	[Parameter()]
	[String] $OutputPath = "bin\Debug",

    # Also add following parameters: 
    #   Configuration
    #   Platform
    #   OutputPath
    # And use these parameters inside BuildSolution function while calling for MSBuild.
    # Use /p swith to pass the parameter. For example:
    #   MSBuild.exe src/Solution.sln /p:Configuration=$Configuration
    # More info here: https://docs.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-properties?view=vs-2017

    [Parameter()]
    [String] $BuildArtifactsFolder
)

$NugetUrl = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
$NugetExe = Join-Path $PSScriptRoot "nuget.exe"
$MSBuildPath = "C:/Program Files (x86)/Microsoft Visual Studio/2017/Enterprise/MSBuild/15.0/Bin/MSBuild.exe"
$SolutionPath = "SeleniumAdvanced-second-lection/SeleniumAdvanced-second-lection.sln"
# Define additional variables here (MSBuild path, etc.)

Function DownloadNuGet()
{
    if (-Not (Test-Path $NugetExe)) 
    {
        Write-Output "Installing NuGet from $NugetUrl..."
        Invoke-WebRequest $NugetUrl -OutFile $NugetExe -ErrorAction Stop
    }
}

Function RestoreNuGetPackages()
{
    DownloadNuGet
    Write-Output 'Restoring NuGet packages...'
    C:/Jars/NUnit.org/nuget.exe restore SeleniumAdvanced-second-lection/SeleniumAdvanced-second-lection.sln
    # NuGet.exe call here
}

Function BuildSolution()
{
    Write-Output "Building '$Solution' solution..."
    $MSBuildPath $SolutionPath /p:Configuration=$Configuration /p:Platform=$Platform /p:OutputPath=$OutputPath  
    # MSBuild.exe call here
}

Function CopyBuildArtifacts()
{
    param
    (
        [Parameter(Mandatory)]
        [String] $SourceFolder,
        [Parameter(Mandatory)]
        [String] $DestinationFolder
    )

    # Copy all files from $SourceFolder to $DestinationFolder
    #
    # Useful commands:
    #   Test-Path - check if path exists
    #   Remove-Item - remove folders/files
    #   New-Item - create folder/file
    #   Get-ChildItem - gets items from specified path
    #   Copy-Item - copies item into destination folder
    #
    #     NOTE: you can chain methods using pipe (|) symbol. For example:
    #           Get-ChildItem ... | Copy-Item ...
    #
    #           which will get items (Get-ChildItem) and will copy them (Copy-Item) to the target folder
}

foreach ($Task in $TaskList) {
    if ($Task.ToLower() -eq 'restorepackages')
    {
        RestoreNuGetPackages
    }
    if ($Task.ToLower() -eq 'build')
    {
        BuildSolution
    }
    if ($Task.ToLower() -eq 'copyartifacts')
    {
        CopyBuildArtifacts "YOUR FOLDER WITH BUILT DLLS" "$BuildArtifactsFolder"
    }
}