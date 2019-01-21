using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeleniumAdvanced_second_lection
{
    [TestFixture]
    class TestSuite
    {
        private IWebDriver driver;
        private IJavaScriptExecutor executor;
        private string downloadPath;


        [SetUp]
        public void SetUp()
        {
            ChromeOptions options = new ChromeOptions();
            downloadPath = Path.Combine(@"C:\", "SeleniumDownloads", DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss"));
            options.AddUserProfilePreference("download.default_directory", downloadPath);
            options.AddArgument("--start-fullscreen");
            driver = new ChromeDriver(options);
            
        }

        [Test]
        [Ignore("not used in homework")]
        //[Category("FirstTest")]
        public void SeleniumAdvancedSecondLectionTest()
        {
            //int a;

            driver.Navigate().GoToUrl(@"https://unsplash.com/search/photos/test");

            executor = (IJavaScriptExecutor)driver;

            //To scroll to the bottom of page,
            //can use any of these two methods by uncommenting one of them, 
            // details in Framework.cs :

            executor.ScrollToBottomByFindingElement(driver);
            //executor.ScrollToBottomByPosition();

            var downloadPhotoButtons = driver.FindElements(By.XPath("//a[@title='Download photo']/span"));

            //Search for the lowest download button by comparing their positions
            IWebElement lastDownloadPhotoButton = null;
            int lastDownloadPhotoButtonPosition = 0;

            foreach (IWebElement element in downloadPhotoButtons)
            {
                if (element.Location.Y > lastDownloadPhotoButtonPosition)
                {
                    lastDownloadPhotoButton = element;
                    lastDownloadPhotoButtonPosition = element.Location.Y;
                }
            }

            if (lastDownloadPhotoButton != null)
                executor.ExecuteScript($"arguments[0].click()", lastDownloadPhotoButton);
            else
                throw new Exception("Last photo download button is not found");

            //Wait until file is downloaded
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(p => (Directory.Exists(@downloadPath) && (Directory.GetFiles(downloadPath).Length > 0)));

            int numberOfFilesInCurrentDownloadFolder = Directory.GetFiles(@downloadPath).Length;
            Assert.That(numberOfFilesInCurrentDownloadFolder, Is.EqualTo(1));

        }

        [Test]
        [Category("FirstTest")]
        public void FirstTest()
        {
            Assert.IsTrue(1 == 1);
        }

        [Test]
        [Category("SecondTest")]
        public void SecondTest()
        {
            Assert.IsTrue(1 == 2);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Dispose();
        }
    }
}
