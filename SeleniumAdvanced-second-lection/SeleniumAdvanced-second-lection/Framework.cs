using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeleniumAdvanced_second_lection
{
    public static class Framework
    {


        //It verifies whether it is the bottom by calculating the position
        // Does not work normally without Thread.Sleep
        public static void ScrollToBottomByPosition(this IJavaScriptExecutor executor)
        {
            bool isAtBottom = false;

            while (!isAtBottom)
            {
                executor.ExecuteScript(@"window.scrollTo(0, document.querySelector('body').scrollHeight)");
                int currentPosition = Convert.ToInt32(executor.ExecuteScript("return document.documentElement.scrollTop"));
                int windowVisiblePartHeight = Convert.ToInt32(executor.ExecuteScript("return window.innerHeight"));
                int fullDocumentHeight = Convert.ToInt32(executor.ExecuteScript("return document.documentElement.scrollHeight"));
                Thread.Sleep(500);
                if (currentPosition + windowVisiblePartHeight == fullDocumentHeight)
                {
                    isAtBottom = true;
                    break;
                }
            }
        }

        //Verifies whether it is bottom of page by searching for "Make something awesome"
        // label. It works without Thread.Sleep but depends on existing of this label
        public static void ScrollToBottomByFindingElement(this IJavaScriptExecutor executor, IWebDriver driver)
        {
            bool isBottomButtonVisible = false;

            while (!isBottomButtonVisible)
            {
                executor.ExecuteScript(@"window.scrollTo(0, document.querySelector('body').scrollHeight)");
                if (driver.FindElements(By.XPath("//p[text()='Make something awesome']")).Count != 0)
                {
                    isBottomButtonVisible = true;
                    break;
                }
            }
        }
    }
}
