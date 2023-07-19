using OpenAiTests.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiTests.Tests
{
    [TestClass]
    public class Environment
    {
        [TestMethod]
        public void OpenAiKeyFileExists()
        {
            Assert.IsTrue(File.Exists(TestEnvironmentHelper.OpenAiKeyPath), $"The {TestEnvironmentHelper.OpenAiKeyPath} file could not be found");
        }

        [TestMethod]
        public void OpenAiKeyFileHasContent()
        {
            Assert.IsTrue(!string.IsNullOrEmpty(File.ReadAllText(TestEnvironmentHelper.OpenAiKeyPath)), $"The {TestEnvironmentHelper.OpenAiKeyPath} file is missing content");
        }
    }
}
