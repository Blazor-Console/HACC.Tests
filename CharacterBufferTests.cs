using System.Collections.Generic;
using System.Linq;
using HACC.VirtualConsoleBuffer;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HACC.Tests;

[TestClass]
public class CharacterBufferTests
{
    [TestMethod]
    public void HelloWorld()
    {
        var expectedLineOne = "Hello World";
        var expectedLineTwo = "Spectre is amazing, thanks Patrik";

        var loggerMock = new Mock<ILogger>();

        var characterBuffer = new CharacterBuffer(
            loggerMock.Object,
            Defaults.InitialColumns,
            Defaults.InitialRows);

        characterBuffer.WriteLine(expectedLineOne, new CharacterEffects(bold: true));

        Assert.AreEqual(
            1,
            characterBuffer.Cursor.Position.Y);
        characterBuffer.WriteLine(expectedLineTwo, new CharacterEffects());

        var effectsPastEndLineOne = characterBuffer.CharacterEffectsAt(expectedLineOne.Length, 0);
        var effectsPastEndLineTwo = characterBuffer.CharacterEffectsAt(expectedLineTwo.Length, 1);
        var dirtySections = characterBuffer.DirtyRangeValues();

        Assert.AreEqual(
            expectedLineOne,
            characterBuffer.GetLine(0, 0));

        var firstElement = dirtySections.ElementAt(0);
        Assert.AreEqual(
            expected: 0,
            actual: firstElement.xStart);
        Assert.AreEqual(
            expected: expectedLineOne.Length - 1,
            actual: firstElement.xEnd);
        Assert.AreEqual(
            expected: 0,
            actual: firstElement.y);
        Assert.AreEqual(
            expected: expectedLineOne,
            actual: firstElement.value);
        Assert.AreEqual(
            expected: new CharacterEffects(bold: true),
            actual: firstElement.effects);

        Assert.AreEqual(
            new CharacterEffects(),
            effectsPastEndLineOne);

        Assert.AreEqual(
            expectedLineTwo,
            characterBuffer.GetLine(0, 1));

        var secondElement = dirtySections.ElementAt(1);
        Assert.AreEqual(
            expected: 0,
            actual: secondElement.xStart);
        Assert.AreEqual(
            expected: expectedLineTwo.Length - 1,
            actual: secondElement.xEnd);
        Assert.AreEqual(
            expected: 1,
            actual: secondElement.y);
        Assert.AreEqual(
            expected: expectedLineTwo,
            actual: secondElement.value);
        Assert.AreEqual(
            expected: new CharacterEffects(bold: false),
            actual: secondElement.effects);


        Assert.AreEqual(
            new CharacterEffects(),
            effectsPastEndLineTwo);
    }
}