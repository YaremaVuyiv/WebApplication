using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace Tests
{
    public class Tests
    {
        private const string BaseUrl = "http://localhost:4200/";

        IWebDriver driver;

        [SetUp]
        public void startBrowser()
        {
            driver = new ChromeDriver(@"C:\Users\Yarema\Documents");
            driver.Url = BaseUrl;
        }

        [Test]
        public void SpecificElementsShouldBeOnStartPage()
        {
            WaitTillSpinnerIsShown();
            Assert.True(driver.FindElement(By.Id("logButton")).Enabled);
            Assert.True(driver.FindElement(By.Id("createButton")).Enabled);
            Assert.True(driver.FindElement(By.Id("accountButton")).Enabled);
            Assert.True(driver.FindElement(By.Id("topic1")).Enabled);
        }

        [Test]
        public void SpecificElementsShouldBeOnLoginPage()
        {
            WaitTillSpinnerIsShown();
            driver.Url = BaseUrl + "login";
            Assert.True(driver.FindElement(By.Id("logButton")).Enabled);
            Assert.True(driver.FindElement(By.Id("loginButton")).Enabled);
            Assert.True(driver.FindElement(By.Id("registerButton")).Enabled);
            Assert.True(driver.FindElement(By.Id("forgotPasswordButton")).Enabled);
        }

        [Test]
        public void SpecificElementsShouldBeOnTopicDetailPage()
        {
            WaitTillSpinnerIsShown();
            driver.Url = BaseUrl + "detail/1";
            Assert.True(driver.FindElement(By.Id("logButton")).Enabled);
            Assert.False(driver.FindElement(By.Id("commentButton")).Enabled);
            Assert.True(driver.FindElement(By.Id("comment")).Displayed);
        }

        [Test]
        public void SpecificElementsShouldBeOnRegisterPage()
        {
            WaitTillSpinnerIsShown();
            driver.Url = BaseUrl + "register";
            Assert.True(driver.FindElement(By.Id("logButton")).Enabled);
            Assert.False(driver.FindElement(By.Id("registerSubmitButton")).Enabled);
            Assert.True(driver.FindElement(By.Id("password")).Displayed);
            Assert.True(driver.FindElement(By.Id("pass")).Displayed);
            Assert.True(driver.FindElement(By.Id("email")).Displayed);
            Assert.True(driver.FindElement(By.Id("username")).Displayed);
        }

        [Test]
        public void RegistertButtonShouldBecomeActiveIfDataIsTyped()
        {
            WaitTillSpinnerIsShown();
            driver.Url = BaseUrl + "register";
            Assert.False(driver.FindElement(By.Id("registerSubmitButton")).Enabled);
            driver.FindElement(By.Id("email")).SendKeys("test@mail.com");
            driver.FindElement(By.Id("username")).SendKeys("user");
            driver.FindElement(By.Id("password")).SendKeys("1111QQqq");
            driver.FindElement(By.Id("pass")).SendKeys("1111QQqq");
            Assert.True(driver.FindElement(By.Id("registerSubmitButton")).Enabled);
        }

        [Test]
        public void CommentButtonShouldBecomeActiveIfCommentIsTyped()
        {
            WaitTillSpinnerIsShown();
            driver.Url = BaseUrl + "detail/1";
            Assert.False(driver.FindElement(By.Id("commentButton")).Enabled);
            driver.FindElement(By.Id("comment")).SendKeys("text");
            Assert.True(driver.FindElement(By.Id("commentButton")).Enabled);
        }

        [Test]
        public void LoginButtonClickShouldRedirectToLoginPage()
        {
            WaitTillSpinnerIsShown();
            driver.FindElement(By.Id("logButton")).Click();
            Assert.AreEqual(BaseUrl + "login", driver.Url);
        }

        [Test]
        public void CreateTopicClickWithoutLoginShouldRedirectToLoginPage()
        {
            WaitTillSpinnerIsShown();
            driver.FindElement(By.Id("createButton")).Click();
            Assert.AreEqual(BaseUrl + "login?returnUrl=%2Fcreate", driver.Url);
        }

        [Test]
        public void AccountClickWithoutLoginShouldRedirectToLoginPage()
        {
            WaitTillSpinnerIsShown();
            driver.FindElement(By.Id("accountButton")).Click();
            Assert.AreEqual(BaseUrl + "login?returnUrl=%2Faccount", driver.Url);
        }

        [Test]
        public void ForgotPasswordClickShouldRedirectToForgotPasswordPage()
        {
            WaitTillSpinnerIsShown();
            driver.FindElement(By.Id("logButton")).Click();
            driver.FindElement(By.Id("forgotPasswordButton")).Click();
            Assert.AreEqual(BaseUrl + "forgotPassword", driver.Url);
        }

        [Test]
        public void RegisterClickShouldRedirectToRegisterPage()
        {
            WaitTillSpinnerIsShown();
            driver.FindElement(By.Id("logButton")).Click();
            driver.FindElement(By.Id("registerButton")).Click();
            Assert.AreEqual(BaseUrl + "register", driver.Url);
        }

        [Test]
        public void CreateShouldOpenCreateUrl()
        {
            Login();
            driver.FindElement(By.Id("createButton")).Click();
            Assert.AreEqual(BaseUrl + "create", driver.Url);
        }

        [Test]
        public void LoginTest()
        {
            WaitTillSpinnerIsShown();
            driver.FindElement(By.Id("logButton")).Click();
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("1111QQqq");
            driver.FindElement(By.Id("loginButton")).Click();
            WaitTillSpinnerIsShown();
            Assert.AreEqual(BaseUrl, driver.Url);
        }

        [Test]
        public void TopicClickShouldOpenTopicDetailsUrl()
        {
            WaitTillSpinnerIsShown();
            driver.FindElement(By.Id("topic1")).Click();
            Assert.AreEqual(BaseUrl + "detail/1", driver.Url);
        }

        [TearDown]
        public void closeBrowser()
        {
            driver.Close();
        }

        private void Login()
        {
            driver.Url = BaseUrl + "login";
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("1111QQqq");
            driver.FindElement(By.Id("loginButton")).Click();

            WaitTillSpinnerIsShown();
        }

        private void WaitTillSpinnerIsShown()
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(2))
                .Until(d => !d.FindElement(By.Id("spinner")).Displayed);
        }
    }
}