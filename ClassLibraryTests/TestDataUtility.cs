using System.IO;
using System.Reflection;

namespace ClassLibraryTests
{
    public class TestDataUtility
    {
        public static string GetResourceText(string resourceName)
        {
            var resourceText = string.Empty;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourceName}"))
            using (var reader = new StreamReader(stream))
            {
                resourceText = reader.ReadToEnd();
            }

            return resourceText;
        }
    }

    public interface IFileTextData
    {
        string ResourceName { get; }
    }
}