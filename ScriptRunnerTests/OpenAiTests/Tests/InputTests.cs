using OpenAiTests.Utilities;
using ScriptRunner.OpenAi;
using ScriptRunner.OpenAi.Models.Completion;
using ScriptRunner.OpenAi.Models.Input;
using ScriptRunner.OpenAi.Models.Input.Types;

namespace ScriptRunnerTests.OpenAiTests.Tests
{
    [TestClass]
    public class InputTests
    {
        public OpenAiApi OpenAi { get { return _openAi ?? throw new NullReferenceException("OpenAiApi is null"); } set { _openAi = value; } }
        private OpenAiApi? _openAi;

        public Conversation Conversation { get { return _conversation ?? throw new NullReferenceException("Conversation is null"); } set { _conversation = value; } }
        private Conversation? _conversation;

        private int notNullCounter = 0;

        [TestInitialize]
        public void InitializeTest()
        {
            _openAi = new OpenAiApi(TestEnvironmentHelper.GetOpenAiApiKey());
            _conversation = new Conversation(_openAi, Model.Default);
        }

        [TestMethod]
        public async Task EnsureNotNull()
        {
            Conversation.Communicator.OnWantsInput += EnsureNotNullEventHandler;

            decimal number = (await Conversation.Input.GetAsync<decimal?>("Write a number: ", true, 4, "That's not a number. Try again: ")).Value;

            Assert.IsNotNull(number);
            Assert.AreEqual(123.4m, number);

            Conversation.Communicator.OnWantsInput -= EnsureNotNullEventHandler;
            Conversation.Communicator.OnWantsInput += EnsureNotNullEventHandler2;

            number = (await Conversation.Input.GetAsync<decimal?>("Write a number: ", true)).Value;

            Conversation.Communicator.OnWantsInput -= EnsureNotNullEventHandler2;
            Conversation.Communicator.OnWantsInput += EnsureNotNullEventHandler3;

            decimal? number2 = await Conversation.Input.GetAsync<decimal?>("Write a number: ");

            Assert.IsNull(number2);

            Conversation.Communicator.OnWantsInput -= EnsureNotNullEventHandler3;
            Conversation.Communicator.OnWantsInput += EnsureNotNullEventHandler4;

            InputException? inputException = null;
            
            try
            {
                await Conversation.Input.GetAsync<decimal?>("Write a number: ", true, 1, "That's not a number. Try again: ");
            }
            catch(InputException exception)
            {
                inputException = exception;
            }

            Assert.IsNotNull(inputException);
        }

        private void EnsureNotNullEventHandler(InputHandler sender, InputInfo inputInfo)
        {
            Assert.IsNotNull(inputInfo);
            Assert.IsTrue(inputInfo.Type == typeof(decimal?).ToString(), "Input info is not of type decimal?");

            Task.Run(async () =>
            {
                await Task.Delay(100);

                if (notNullCounter == 0)
                {
                    sender.AddResponse("hello! :)");
                }
                else
                {
                    sender.AddResponse("123.4");
                }

                notNullCounter++;
            }).Wait();

            if(notNullCounter == 1)
                Assert.AreEqual("Write a number: ", inputInfo.Message);
            else
                Assert.AreEqual("That's not a number. Try again: ", inputInfo.Message);
        }

        private void EnsureNotNullEventHandler2(InputHandler sender, InputInfo inputInfo)
        {
            Assert.IsNotNull(inputInfo);
            Assert.IsTrue(inputInfo.Type == typeof(decimal?).ToString(), "Input info is not of type decimal?");

            Task.Run(async () =>
            {
                await Task.Delay(100);
                sender.AddResponse("123.4");
            });
        }

        private void EnsureNotNullEventHandler3(InputHandler sender, InputInfo inputInfo)
        {
            Assert.IsNotNull(inputInfo);
            Assert.IsTrue(inputInfo.Type == typeof(decimal?).ToString(), "Input info is not of type decimal");

            Task.Run(async () =>
            {
                await Task.Delay(100);
                sender.AddResponse("invalid");
            });
        }

        private void EnsureNotNullEventHandler4(InputHandler sender, InputInfo inputInfo)
        {
            Assert.IsNotNull(inputInfo);
            Assert.IsTrue(inputInfo.Type == typeof(decimal?).ToString(), "Input info is not of type decimal");

            Task.Run(async () =>
            {
                await Task.Delay(100);
                sender.AddResponse("invalid");
            });
        }

        [TestMethod]
        public async Task TakeInputChoiceInput()
        {
            Conversation.Communicator.OnWantsInput += TakeInputChoiceInputEventHandler;

            List<InputChoice> choices = new List<InputChoice>
            {
                new InputChoice("Yes"),
                new InputChoice("No"),
                new InputChoice("Maybe")
            };

            InputChoice? choice = await Conversation.Input.GetAsync<InputChoice>("Do you like this test?", choices);

            Assert.IsNotNull(choice);
        }

        private void TakeInputChoiceInputEventHandler(InputHandler sender, InputInfo inputInfo)
        {
            Assert.IsNotNull(inputInfo);
            Assert.IsTrue(inputInfo.Type == typeof(InputChoice).ToString(), "Input info is not of type InputChoice");

            Task.Run(async () =>
            {
                await Task.Delay(100);
                sender.AddResponse("Yes");
            });
        }

        [TestMethod]
        public async Task TakeIntInput()
        {
            Conversation.Communicator.OnWantsInput += TakeIntInputEventHandler;

            int number = await Conversation.Input.GetAsync<int>("Write a number: ");

            Assert.IsNotNull(number);
            Assert.AreEqual(420, number);
        }

        private void TakeIntInputEventHandler(InputHandler sender, InputInfo inputInfo)
        {
            Assert.IsNotNull(inputInfo);
            Assert.IsTrue(inputInfo.Type == typeof(int).ToString(), "Input info is not of type int");

            Task.Run(async () =>
            {
                await Task.Delay(100);
                sender.AddResponse("420");
            });
        }

        [TestMethod]
        public async Task TakeBoolInput()
        {
            Conversation.Communicator.OnWantsInput += TakeBoolInputEventHandler;

            bool answer = await Conversation.Input.GetAsync<bool>("Yay or nay?");

            Assert.IsNotNull(answer);
            Assert.AreEqual(true, answer);
        }

        private void TakeBoolInputEventHandler(InputHandler sender, InputInfo inputInfo)
        {
            Assert.IsNotNull(inputInfo);
            Assert.IsTrue(inputInfo.Type == typeof(bool).ToString(), "Input info is not of type bool");

            Task.Run(async () =>
            {
                await Task.Delay(100);
                sender.AddResponse("Yes");
            });
        }
    }
}
