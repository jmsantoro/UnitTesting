using NUnit.Framework;

namespace ClassLibraryTests
{
    public abstract class TestBase
    {
        protected static TestContext _testContext;

        public TestContext TestContext
        {
            get
            {
                return _testContext;
            }
            set
            {
                _testContext = value;
            }
        }
    }
}