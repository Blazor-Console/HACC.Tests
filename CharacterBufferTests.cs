using System;
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
        // Arrange
        var expectedLineOne = "Hello World";
        var expectedLineTwo = "Spectre is amazing, thanks Patrik";

        var loggerMock = new Mock<ILogger>();

        var characterBuffer = new CharacterBuffer(
            loggerMock.Object,
            Defaults.InitialColumns,
            Defaults.InitialRows);
        
        // Act
        characterBuffer.WriteLine(expectedLineOne, new CharacterEffects(bold: true));

        // Assert cursor position moved automatically.
        Assert.AreEqual(
            0,
            characterBuffer.Cursor.Position.X);
        Assert.AreEqual(
            1,
            characterBuffer.Cursor.Position.Y);

        // Act / finish acting, step 2
        characterBuffer.WriteLine(expectedLineTwo, new CharacterEffects());


        // Assert
        var effectsPastEndLineOne = characterBuffer.CharacterEffectsAt(expectedLineOne.Length, 0);
        var effectsPastEndLineTwo = characterBuffer.CharacterEffectsAt(expectedLineTwo.Length, 1);
        var dirtySections = characterBuffer.DirtyRangeValues();

        Assert.AreEqual(
            expectedLineOne,
            characterBuffer.GetLine(0, 0));

        var firstElement = dirtySections.ElementAt(0); // this call is a library call, not our code.
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

        var secondElement = dirtySections.ElementAt(1); // this call is a library call, not our code.
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

    [TestMethod]
    public void TwoColorNoWrapTest()
    {
        // Arrange
        var expectedLineOne = "Hello World";

        var expectedEffectsOne = new CharacterEffects(
            bold: true,
            italic: false,
            underline: false,
            inverse: false,
            blink: false,
            background: ConsoleColor.Black,
            foreground: ConsoleColor.White);

        var expectedLineOnePartTwo = "Spectre is amazing, thanks Patrik";

        var expectedEffectsTwo = new CharacterEffects(
            bold: false,
            italic: false,
            underline: false,
            inverse: false,
            blink: false,
            background: ConsoleColor.Black,
            foreground: ConsoleColor.Yellow);

        var loggerMock = new Mock<ILogger>();

        var characterBuffer = new CharacterBuffer(
            loggerMock.Object,
            Defaults.InitialColumns,
            Defaults.InitialRows);


        // Act
        characterBuffer.WriteLine(
            line: expectedLineOne,
            characterEffects: expectedEffectsOne,
            automaticWrap: false);

        // Assert cursor position moved automatically.
        Assert.AreEqual(
            expectedLineOne.Length,
            characterBuffer.Cursor.Position.X);
        Assert.AreEqual(
            0,
            characterBuffer.Cursor.Position.Y);

        // Act / finish acting, step 2
        characterBuffer.WriteLine(
            line: expectedLineOnePartTwo,
            characterEffects: expectedEffectsTwo,
            automaticWrap: true);


        // Assert
        Assert.AreEqual(
            0,
            characterBuffer.Cursor.Position.X);
        Assert.AreEqual(
            1,
            characterBuffer.Cursor.Position.Y);

        var dirtySections = characterBuffer.DirtyRangeValues();

        var combinedStrings = expectedLineOne + expectedLineOnePartTwo;
        Assert.AreEqual(
            expected: combinedStrings,
            actual: characterBuffer.GetLine(0, 0));

        var firstElement = dirtySections.ElementAt(0); // this call is a library call, not our code.
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
            expected: expectedEffectsOne,
            actual: firstElement.effects);

        var secondElement = dirtySections.ElementAt(1); // this call is a library call, not our code.
        Assert.AreEqual(
            expected: expectedLineOne.Length,
            actual: secondElement.xStart);
        Assert.AreEqual(
            expected: combinedStrings.Length - 1,
            actual: secondElement.xEnd);
        Assert.AreEqual(
            expected: 0,
            actual: secondElement.y);
        Assert.AreEqual(
            expected: expectedLineOnePartTwo,
            actual: secondElement.value);
        Assert.AreEqual(
            expected: expectedEffectsTwo,
            actual: secondElement.effects);
    }
}