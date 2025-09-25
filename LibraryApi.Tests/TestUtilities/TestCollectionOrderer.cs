using Xunit.Abstractions;
using Xunit.Sdk;

namespace LibraryApi.Tests.TestUtilities;

public class TestCollectionOrderer : ITestCollectionOrderer
{
    public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
    {
        return testCollections.OrderBy(collection => collection.DisplayName);
    }
}
