properties([
    parameters([
        string (name: 'branchName', defaultValue: 'master', description: 'Branch to get the tests from')
    ])
])

def isFailed = false
def branch = params.branchName
def buildArtifactsFolder = "C:/BuildPackagesFromPipeline/$BUILD_ID"
def debugFolder = "SeleniumAdvanced-second-lection/SeleniumAdvanced-second-lection/bin/Debug"
currentBuild.description = "Branch: $branch"

def RunNUnitTests(String pathToDll, String condition, String reportName)
{
    try
    {
        bat "C:/Jars/NUnit.org/nunit-console/nunit3-console.exe $pathToDll $condition --result=$reportName"
    }
    finally
    {
        stash name: reportName, includes: reportName
    }
}

node('master') 
{
    stage('Checkout')
    {
        git branch: branch, url: 'https://github.com/VOstrovskyiForte/seleniumAdvancedJenkins.git'
    }
    
    stage('Restore NuGet')
    {
        powershell '.\\build.ps1 RestoreNuGetPackages'
        //bat '"C:/Jars/NUnit.org/nuget.exe" restore SeleniumAdvanced-second-lection/SeleniumAdvanced-second-lection.sln'
    }

    stage('Build Solution')
    {
        powershell '.\\build.ps1 BuildSolution'  
        //bat '"C:/Program Files (x86)/Microsoft Visual Studio/2017/Enterprise/MSBuild/15.0/Bin/MSBuild.exe" SeleniumAdvanced-second-lection/SeleniumAdvanced-second-lection.sln'
    }

    stage('Copy Artifacts')
    {         
        powershell ".\\build.ps1 -BuildArtifactsFolder $buildArtifactsFolder CopyBuildArtifacts $debugFolder $buildArtifactsFolder"
    }
}

catchError
{
    isFailed = true
    stage('Run Tests')
    {
        parallel FirstTest: {
            node('master') {
                RunNUnitTests("$buildArtifactsFolder/SeleniumAdvanced-second-lection.dll", "--where cat==FirstTest", "TestResult1.xml")
            }
        }, SecondTest: {
            node('Slave') {
                RunNUnitTests("$buildArtifactsFolder/SeleniumAdvanced-second-lection.dll", "--where cat==SecondTest", "TestResult2.xml")
            }  
        }
    }
    isFailed = false
}

node('master')
{
    stage('Reporting')
    {
        unstash "TestResult1.xml"
        unstash "TestResult2.xml"

        archiveArtifacts '*.xml'
        nunit testResultsPattern: 'TestResult1.xml, TestResult2.xml'
    }
}