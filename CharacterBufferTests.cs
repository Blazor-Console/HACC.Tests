using HACC.VirtualConsoleBuffer;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

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

            var effectsPastEndLineOne = characterBuffer.CharacterEffectsAt(x: expectedLineOne.Length, y: 0);
            var effectsPastEndLineTwo = characterBuffer.CharacterEffectsAt(x: expectedLineTwo.Length, y: 1);
            var dirtySections = characterBuffer.DirtyRangeValues(includeEffectsChanges: true);

            Assert.AreEqual(
                expected: expectedLineOne,
                actual: characterBuffer.GetLine(x: 0, y: 0));

            Assert.AreEqual(
                expected: expectedLineTwo,
                actual: characterBuffer.GetLine(x: 0, y: 1));

            Assert.AreEqual(
                expected: new List<(int xStart, int xEnd, int y, string value, CharacterEffects effects)>() {
                    {( xStart: 0, xEnd: expectedLineOne.Length, y: 0, value: expectedLineOne, effects: new CharacterEffects(bold: true) )},
                    {( xStart: 0, xEnd: expectedLineTwo.Length, y: 1, value: expectedLineOne, effects: new CharacterEffects(bold: false) )},
                },
                actual: dirtySections);

            Assert.AreEqual(
                expected: new CharacterEffects(),
                actual: effectsPastEndLineOne);

            Assert.AreEqual(
                expected: new CharacterEffects(),
                actual: effectsPastEndLineTwo);
        }
    }
}