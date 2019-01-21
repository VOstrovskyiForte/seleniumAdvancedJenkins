properties([
    parameters([
        string (name: 'branchName', defaultValue: 'master', description: 'Branch to get the tests from')
    ])
])

def isFailed = false
def branch = params.branchName
def buildArtifactsFolder = "C:/BuildPackagesFromPipeline/$BUILD_ID"
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
    }

    stage('Build Solution')
    {
        powershell '.\\build.ps1 BuildSolution'  
    }

    stage('Copy Artifacts')
    {
        bat "(robocopy SeleniumAdvanced-second-lection/SeleniumAdvanced-second-lection/bin/Debug $buildArtifactsFolder /MIR /XO) ^& IF %ERRORLEVEL% LEQ 1 exit 0"
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