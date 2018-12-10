namespace SecondMonitor.DataModelTests.BasicProperties
{
    using DataModel.BasicProperties;
    using NUnit.Framework;

    [TestFixture]
    public class VelocityTests
    {
        [Test]
        [TestCase(3.6)]
        [TestCase(235332.32)]
        [TestCase(0)]
        public void Create_WhenFromKph_ThenValueInMsCorrect(double value)
        {
            // Arrange
            Velocity _testee = Velocity.FromKph(value);

            // Act
            double inMs = _testee.InMs;

            // Assert
            Assert.That(inMs, Is.EqualTo(value / 3.6).Within(0.001));
        }

        [Test]
        [TestCase(3.6)]
        [TestCase(235332.32)]
        [TestCase(0)]
        public void Mph_WhenConvertedToMph_ThenCalueInMphIsCorrect(double valueInMs)
        {
            // Arrange
            Velocity _testee = Velocity.FromMs(valueInMs);

            // Act
            double inMph = _testee.InMph;

            // Assert
            Assert.That(inMph, Is.EqualTo(valueInMs * 2.23694).Within(0.001));
        }
    }
}