using System;
using System.Linq;
using HACC.Models;
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
            logger: loggerMock.Object);

        // Act
        characterBuffer.WriteLine(line: expectedLineOne, characterEffects: new CharacterEffects(bold: true));

        // Assert cursor position moved automatically.
        Assert.AreEqual(
            expected: 0,
            actual: characterBuffer.Cursor.Position.X);
        Assert.AreEqual(
            expected: 1,
            actual: characterBuffer.Cursor.Position.Y);

        // Act / finish acting, step 2
        characterBuffer.WriteLine(line: expectedLineTwo, characterEffects: new CharacterEffects());


        // Assert
        var effectsPastEndLineOne = characterBuffer.CharacterEffectsAt(x: expectedLineOne.Length, y: 0);
        var effectsPastEndLineTwo = characterBuffer.CharacterEffectsAt(x: expectedLineTwo.Length, y: 1);
        var dirtySections = characterBuffer.DirtyRangeValues();

        Assert.AreEqual(
            expected: 2,
            actual: dirtySections.Count());

        Assert.AreEqual(
            expected: expectedLineOne,
            actual: characterBuffer.GetLine(x: 0, y: 0));

        var firstElement = dirtySections.ElementAt(index: 0); // this call is a library call, not our code.
        Assert.AreEqual(
            expected: 0,
            actual: firstElement.XStart);
        Assert.AreEqual(
            expected: expectedLineOne.Length - 1,
            actual: firstElement.XEnd);
        Assert.AreEqual(
            expected: 0,
            actual: firstElement.Y);
        Assert.AreEqual(
            expected: expectedLineOne,
            actual: firstElement.Value);
        Assert.IsTrue(condition: firstElement.CharacterEffects.Equals(
            other: new CharacterEffects(bold: true)));

        Assert.AreEqual(
            expected: new CharacterEffects(),
            actual: effectsPastEndLineOne);

        Assert.AreEqual(
            expected: expectedLineTwo,
            actual: characterBuffer.GetLine(x: 0, y: 1));

        var secondElement = dirtySections.ElementAt(index: 1); // this call is a library call, not our code.
        Assert.AreEqual(
            expected: 0,
            actual: secondElement.XStart);
        Assert.AreEqual(
            expected: expectedLineTwo.Length - 1,
            actual: secondElement.XEnd);
        Assert.AreEqual(
            expected: 1,
            actual: secondElement.Y);
        Assert.AreEqual(
            expected: expectedLineTwo,
            actual: secondElement.Value);
        Assert.AreEqual(
            expected: new CharacterEffects(bold: false),
            actual: secondElement.CharacterEffects);


        Assert.AreEqual(
            expected: new CharacterEffects(),
            actual: effectsPastEndLineTwo);
    }

    [TestMethod]
    public void EmptyScreen()
    {
        // Arrange
        var loggerMock = new Mock<ILogger>();

        var characterBuffer = new CharacterBuffer(
            logger: loggerMock.Object);

        var dirtySections = characterBuffer.DirtyRangeValues();

        Assert.AreEqual(
            expected: 0,
            actual: dirtySections.Count());
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
            logger: loggerMock.Object);


        // Act
        characterBuffer.WriteLine(
            line: expectedLineOne,
            characterEffects: expectedEffectsOne,
            automaticNewLine: false);

        // Assert cursor position moved automatically.
        Assert.AreEqual(
            expected: expectedLineOne.Length,
            actual: characterBuffer.Cursor.Position.X);
        Assert.AreEqual(
            expected: 0,
            actual: characterBuffer.Cursor.Position.Y);

        // Act / finish acting, step 2
        characterBuffer.WriteLine(
            line: expectedLineOnePartTwo,
            characterEffects: expectedEffectsTwo,
            automaticNewLine: true);


        // Assert
        Assert.AreEqual(
            expected: 0,
            actual: characterBuffer.Cursor.Position.X);
        Assert.AreEqual(
            expected: 1,
            actual: characterBuffer.Cursor.Position.Y);

        var dirtySections = characterBuffer.DirtyRangeValues();

        Assert.AreEqual(
            expected: 2,
            actual: dirtySections.Count());

        var combinedStrings = expectedLineOne + expectedLineOnePartTwo;
        Assert.AreEqual(
            expected: combinedStrings,
            actual: characterBuffer.GetLine(x: 0, y: 0));

        var firstElement = dirtySections.ElementAt(index: 0); // this call is a library call, not our code.
        Assert.AreEqual(
            expected: 0,
            actual: firstElement.XStart);
        Assert.AreEqual(
            expected: expectedLineOne.Length - 1,
            actual: firstElement.XEnd);
        Assert.AreEqual(
            expected: 0,
            actual: firstElement.Y);
        Assert.AreEqual(
            expected: expectedLineOne,
            actual: firstElement.Value);
        Assert.AreEqual(
            expected: expectedEffectsOne,
            actual: firstElement.CharacterEffects);

        var secondElement = dirtySections.ElementAt(index: 1); // this call is a library call, not our code.
        Assert.AreEqual(
            expected: expectedLineOne.Length,
            actual: secondElement.XStart);
        Assert.AreEqual(
            expected: combinedStrings.Length - 1,
            actual: secondElement.XEnd);
        Assert.AreEqual(
            expected: 0,
            actual: secondElement.Y);
        Assert.AreEqual(
            expected: expectedLineOnePartTwo,
            actual: secondElement.Value);
        Assert.AreEqual(
            expected: expectedEffectsTwo,
            actual: secondElement.CharacterEffects);
    }
}