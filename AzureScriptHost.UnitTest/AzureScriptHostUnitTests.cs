using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AzureScriptHost.UnitTest
{
    [TestClass]
    public class AzureScriptHostUnitTests
    {
        [TestMethod]
        public void InitializeScriptHost()
        {
            AzureScriptHost.Rules rules = new AzureScriptHost.Rules("DefaultEndpointsProtocol=https;AccountName=YourAccount;AccountKey=YourAccountKey", "rules");
            dynamic rule = rules.RequestNewRuleSet("BizRules");
            var test = rule.GetMethod("TestFunction");
            Assert.AreEqual("hi testing", test("hi"));
        }
    }
}
