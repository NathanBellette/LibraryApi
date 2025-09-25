using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = false)]
[assembly: TestCollectionOrderer("LibraryApi.Tests.TestUtilities.TestCollectionOrderer", "LibraryApi.Tests")]
