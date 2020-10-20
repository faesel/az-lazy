using Xunit;

namespace az_lazy.test
{
    [CollectionDefinition("LocalStorage")]
    public class TestStartupCollection : ICollectionFixture<LocalStorageFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the tests
    }
}
