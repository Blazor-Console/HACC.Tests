using HACC.VirtualConsoleBuffer;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HACC.Tests
{
    [TestClass]
    public class CharacterBufferTests
    {
        [TestMethod]
        public void HelloWorld()
        {
            var expectedLineOne = "Hello World";
            var expectedLineTwo = "Spectre is amazing, thanks Patrik";

            var loggerMock = new Mock<ILogger>();
            
            CharacterBuffer characterBuffer = new CharacterBuffer(
                logger: loggerMock.Object,
                columns: Defaults.InitialColumns,
                rows: Defaults.InitialRows);

            characterBuffer.WriteLine(line: expectedLineOne, new CharacterEffects(bold: true));
            characterBuffer.WriteLine(line: expectedLineTwo, new CharacterEffects());

            Assert.AreEqual(
                expected: expectedLineOne,
                actual: characterBuffer.GetLine(x: 0, y: 0));

            Assert.AreEqual(
                expected: expectedLineTwo,
                actual: characterBuffer.GetLine(x: 0, y: 1));
        }
    }
}