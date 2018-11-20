using Effort.Provider;
using NUnit.Framework;
using System.Diagnostics;

namespace UnitTestProject1
{
    [SetUpFixture]
    public class TestAssemblyInit
    {
        [OneTimeSetUp]
        public static void AssemblyInit(TestContext textContext)
        {
            Debug.WriteLine("In AssemblyInit");
            EffortProviderConfiguration.RegisterProvider();
        }
    }
}