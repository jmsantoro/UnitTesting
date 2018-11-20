using ClassLibrary;
using DataAccess;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibraryTests
{
    [TestFixture]
    public class CarTests : TestBase
    {
        private IEngine _engine;
        private EcuDbContext _db;

        [OneTimeSetUp]
        public static void ClassInit()
        {
        }

        [SetUp]
        public void Setup()
        {
            _engine = Substitute.For<IEngine>();
            _db = Substitute.For<EcuDbContext>();
        }

        [Test]
        [Property("key", "value")]
        public void Start_WhenCalled_ReturnsTrue()
        {
            _engine.Start().Returns(true);

            var car = new Car(_engine, _db);

            var result = car.Start();

            Assert.That(result, Is.True);       // NUnit Assert
            result.Should().BeTrue();           // FluentAssertions

            Assert.That(car.IsRunning, Is.True);// NUnit Assert
            car.IsRunning.Should().BeTrue();    // FluentAssertions
        }

        [Test]
        [Category("HighRisk")]
        public void Start_WhenEngineRaisesException_ThrowsCarStartException()
        {
            // Arrange

            //For non-voids:
            //engine.Start().Returns(x => { throw new EngineStartException(); });

            //For voids and non-voids:
            _engine
                .When(x => x.Start())
                .Do(x => { throw new EngineStartException(); });
            var car = new Car(_engine, _db);

            // Act

            Action act = () => car.Start();

            // NUnit Assert
            Assert.That(() => car.Start(),
                Throws.Exception.TypeOf<CarStartException>()
                .With.Message.EqualTo("Handled EngineStartException"));

            // FluentAssertion
            act.Should().Throw<CarStartException>()
                .WithMessage("Handled EngineStartException");
        }

        [Test]
        public void Start_WhenEngineStartReturnsFalse_ThrowsCarStartException()
        {
            // Arrange
            _engine.Start().Returns(false);

            var car = new Car(_engine, _db);

            // Act
            Action act = () => car.Start();

            // NUnit Assert
            CarStartException ex = Assert.Throws<CarStartException>(
                () => car.Start());
            // OR
            Assert.That(() => car.Start(),
                Throws.Exception.TypeOf<CarStartException>()
                .With.Message.EqualTo("Something went wrong"));

            // NUnit Assert
            Assert.That(ex.Message, Is.EqualTo("Something went wrong"));

            // FluentAssertions
            act.Should().Throw<CarStartException>()
                .WithMessage("Something went wrong");
        }

        [Test]
        public async Task GetSensorData_WithSuppliedSinceDate_ReturnsThreeRecords()
        {
            // Arrange
            ArrangeEcuData();

            var car = new Car(_engine, _db);

            // Act
            var result = await car.GetSensorData(new DateTime(2018, 1, 1, 6, 5, 0));

            // NUnit Assert
            Assert.That(result, Has.Count.EqualTo(3));

            // FluentAssertions
            result.Count().Should().Be(3);
        }

        [Test]
        public void GetSensorData_WithValidSinceDate_DoesNotThrowEcuDataException()
        {
            // Arrange
            ArrangeEcuData();

            var car = new Car(_engine, _db);

            // NUnit Act
            Assert.DoesNotThrowAsync(async () => await car.GetSensorData(new DateTime(2018, 12, 31)));
            // or
            Assert.That(async () => await car.GetSensorData(new DateTime(2018, 12, 31)), Throws.Nothing);

            // Act
            Func<Task> act = async () => { await car.GetSensorData(new DateTime(2018, 12, 31)); };

            // Assert
            act.Should().NotThrow<EcuDataException>();
        }

        [Test]
        public void GetSensorData_WithInvalidSinceDate_ThrowsEcuDataException()
        {
            // Arrange
            ArrangeEcuData();

            // sut = System Under Test
            var sut = new Car(_engine, _db);

            // NUnit Act
            EcuDataException ex = Assert.ThrowsAsync<EcuDataException>(
                async () => await sut.GetSensorData(new DateTime(2017, 12, 31)));

            // Act
            Func<Task> act = async () => { await sut.GetSensorData(new DateTime(2017, 12, 31)); };

            // NUnit Assert
            Assert.That(ex.Message.StartsWith("The sinceTime"));

            // FluentAssertion
            act.Should().Throw<EcuDataException>();
        }

        private static IEnumerable<object[]> DifferentDates =>
            new List<object[]> {
            new object[] { new DateTime(2018, 1, 1, 6, 3, 0), 5 },

            new object[] { new DateTime(2018, 1, 1, 6, 4, 0), 4 },

            new object[] { new DateTime(2018, 1, 1, 6, 1, 0), 7 },
        };

        [Test, Sequential]
        [TestCaseSource(nameof(DifferentDates))]
        public async Task GetSensorData_WithSuppliedSinceDate_ReturnsExpectedCount(DateTime sinceDate, int expectedCount)
        {
            // Arrange
            ArrangeEcuData();

            var car = new Car(_engine, _db);

            // Act
            var result = await car.GetSensorData(sinceDate);

            // NUnit Assert
            Assert.That(result, Has.Count.EqualTo(expectedCount));

            // FluentAssertions
            result.Count().Should().Be(expectedCount);
        }

        [Test, Sequential]
        public void CalculateCurrentRange_WithSuppliedDataRowFuelAndMpg_ReturnsRange(
            [Values(10, 12, 8)] int gallons,
            [Values(28, 30, 25)] int avgMpg,
            [Values(280, 360, 200)] int expectedResult)
        {
            // Arrange
            var car = new Car(_engine, _db);

            // Act
            var result = car.CalculateCurrentRange(gallons, avgMpg);

            // NUnit Assert
            Assert.That(result, Is.EqualTo(expectedResult));

            // FluentAssertions
            result.Should().Be(expectedResult);
        }

        private static IEnumerable<TestCaseData> RangeTestData =>
            new List<TestCaseData> {
            new TestCaseData(10, 28, 280),

            new TestCaseData(12, 30, 360),

            new TestCaseData(8, 25, 200)
        };

        [Test, TestCaseSource(nameof(RangeTestData))]
        public void CalculateCurrentRange_WithSuppliedRangeTestDataFuelAndMpg_ReturnsRange(int gallons, int avgMpg, int expectedResult)
        {
            // Arrange
            var car = new Car(_engine, _db);

            // Act
            var result = car.CalculateCurrentRange(gallons, avgMpg);

            // NUnit Assert
            Assert.That(result, Is.EqualTo(expectedResult));

            // FluentAssertions
            result.Should().Be(expectedResult);
        }

        [Test, TestCaseSource(typeof(RangeDataSource), "TestCases")]
        public void CalculateCurrentRange_WithSuppliedFuelAndMpg_ReturnsRange(int gallons, int avgMpg, int expectedResult)
        {
            // Arrange
            var car = new Car(_engine, _db);

            // Act
            var result = car.CalculateCurrentRange(gallons, avgMpg);

            // NUnit Assert
            Assert.That(result, Is.EqualTo(expectedResult));

            // FluentAssertions
            result.Should().Be(expectedResult);
        }

        private void ArrangeEcuData()
        {
            var ecuDataSet = Substitute.For<DbSet<EcuData>, IQueryable<EcuData>, IDbAsyncEnumerable<EcuData>>()
                 .SetupData(new List<EcuData>
                 {
                    new EcuData { Id = 1, Afr = 14.5M, ManifoldPressureKpa = 10.1M, Timestamp = new DateTime(2018, 1, 1, 6, 0, 0) },
                    new EcuData { Id = 2, Afr = 14.6M, ManifoldPressureKpa = 20.1M, Timestamp = new DateTime(2018, 1, 1, 6, 1, 0) },
                    new EcuData { Id = 3, Afr = 14.8M, ManifoldPressureKpa = 30.1M, Timestamp = new DateTime(2018, 1, 1, 6, 2, 0) },
                    new EcuData { Id = 4, Afr = 14.7M, ManifoldPressureKpa = 40.1M, Timestamp = new DateTime(2018, 1, 1, 6, 3, 0) },
                    new EcuData { Id = 5, Afr = 14.7M, ManifoldPressureKpa = 50.1M, Timestamp = new DateTime(2018, 1, 1, 6, 4, 0) },
                    new EcuData { Id = 6, Afr = 14.6M, ManifoldPressureKpa = 40.1M, Timestamp = new DateTime(2018, 1, 1, 6, 5, 0) },
                    new EcuData { Id = 7, Afr = 14.8M, ManifoldPressureKpa = 50.1M, Timestamp = new DateTime(2018, 1, 1, 6, 6, 0) },
                    new EcuData { Id = 8, Afr = 14.3M, ManifoldPressureKpa = 10.1M, Timestamp = new DateTime(2018, 1, 1, 6, 7, 0) }
                 });

            _db.EcuData.Returns(ecuDataSet);
        }
    }
}