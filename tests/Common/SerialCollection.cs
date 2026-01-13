using Xunit;

namespace Eleon.Tests.Common;

[CollectionDefinition("Serial", DisableParallelization = true)]
public sealed class SerialCollection : ICollectionFixture<SerialFixture>
{
}

public sealed class SerialFixture
{
}
