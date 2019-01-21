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
        bat '"C:/Program Files (x86)/NUnit.org/nunit-console/nunit3-console.exe" "C:/BuildPackagesFromPipeline/$BUILD_ID/PhpTravels.UITests.dll" $condition --result=$reportName'
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
        bat '"C:/Program Files (x86)/Jenkins/apps/nuget.exe" restore SeleniumAdvanced-second-lection/SeleniumAdvanced-second-lection.sln'
    }

    stage('Build Solution')
    {
        bat '"C:/Program Files (x86)/Microsoft Visual Studio/2017/Enterprise/MSBuild/15.0/Bin/MSBuild.exe" SeleniumAdvanced-second-lection/SeleniumAdvanced-second-lection.sln'
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
                RunNUnitTests("C:/BuildPackagesFromPipeline/$BUILD_ID/SeleniumAdvanced-second-lection.dll", "--where cat==FirstTest", "TestResult1.xml")
            }
        }, SecondTest: {
            node('Slave') {
                RunNUnitTests("C:/BuildPackagesFromPipeline/$BUILD_ID/SeleniumAdvanced-second-lection.dll", "--where cat==SecondTest", "TestResult2.xml")
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

        if(isFailed)
        {
            slackSend color: 'danger', message: 'Tests failed.'
        }
        else
        {
            slackSend color: 'good', message: 'Tests passed.'
        }
    }
}