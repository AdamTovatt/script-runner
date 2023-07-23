using ScriptRunner.Models;

namespace ScriptRunnerTests
{
    [TestClass]
    public class XmlCommentTests
    {
        [TestMethod]
        public void ParseXmlCommentString()
        {
            string rawString = @"/// <summary>
                                    /// This is a test script that multiplies two numbers and returns the result.
                                    /// </summary>
                                    /// <param name=""factor1"">The first number to multiply</param>
                                    /// <param name=""factor2"">The second number to multiply</param>
                                    /// <param name=""userName"">The username of the user</param>
                                    /// <returns>A string</returns>";

            XmlComment xmlComment = new XmlComment(rawString);

            Assert.IsNotNull(xmlComment);
            Assert.IsNotNull(xmlComment.Parameters);
            Assert.AreEqual("This is a test script that multiplies two numbers and returns the result.", xmlComment.Summary);
            Assert.AreEqual("The first number to multiply", xmlComment.Parameters["factor1"]);
            Assert.AreEqual("The second number to multiply", xmlComment.Parameters["factor2"]);
            Assert.AreEqual("The username of the user", xmlComment.Parameters["userName"]);
            Assert.AreEqual("A string", xmlComment.Returns);
        }

        [TestMethod]
        public void ParseXmlCommentString2()
        {
            string rawString = @"/// <summary>
                                    /// This is a test script that multiplies two numbers and returns the result.
                                    /// </summary>";

            XmlComment xmlComment = new XmlComment(rawString);

            Assert.IsNotNull(xmlComment);
            Assert.IsNull(xmlComment.Parameters);
            Assert.AreEqual("This is a test script that multiplies two numbers and returns the result.", xmlComment.Summary);
        }
    }
}
