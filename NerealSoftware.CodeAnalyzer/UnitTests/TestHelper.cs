using System.IO;
using System.Reflection;

namespace UnitTests
{
    public static class TestHelper
    {
        public static string GetBasePath(string dirName = null)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var directoryName = Path.GetDirectoryName(location);
            var path = Path.Combine(directoryName,"..\\..");
            if (!string.IsNullOrEmpty(dirName)) path = Path.Combine(path, dirName);
            return path;
        }

        public static string ReadResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}