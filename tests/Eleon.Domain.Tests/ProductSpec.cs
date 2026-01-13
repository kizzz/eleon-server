// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProductSpec.cs" company="Eleon">
// Licensed under the MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Eleon.Domain.Tests
{
    using Eleon.Domain.Entities;
    using Xunit;

    public class ProductSpec
    {
        [Fact]
        public void Ctor_sets_properties()
        {
            var product = new Product("A", 1m);
            Assert.Equal("A", product.Name);
            Assert.Equal(1m, product.Price);
        }
    }
}