using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibraryTests
{
    public class RangeDataSource
    {
        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                var json = TestDataUtility.GetResourceText("testdata.json");

                var data = JsonConvert.DeserializeObject<IEnumerable<RangeTestData>>(json);

                return data.Select(d => new TestCaseData(d.Gallons, d.Mpg, d.Range));
            }
        }
    }
}