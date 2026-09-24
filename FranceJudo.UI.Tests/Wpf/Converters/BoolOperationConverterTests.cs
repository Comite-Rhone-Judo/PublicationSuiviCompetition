#nullable enable
using System;
using System.Globalization;
using Xunit;
using FluentAssertions;
using FranceJudo.UI.Wpf.Converters;

namespace FranceJudo.UI.Tests.Wpf.Converters
{
    public class BoolOperationConverterTests
    {
        [Theory]
        // [A, B, Operation, ResultatAttendu]
        [InlineData(true, true, "a_and_b", true)]
        [InlineData(true, false, "a_and_b", false)]

        [InlineData(true, true, "a_or_b", true)]
        [InlineData(false, false, "a_or_b", false)]

        [InlineData(true, false, "not_a_and_b", false)]
        [InlineData(false, true, "not_a_and_b", true)]

        [InlineData(true, false, "a_and_not_b", true)]
        [InlineData(true, true, "a_and_not_b", false)]

        [InlineData(false, false, "not_a_and_not_b", true)]
        [InlineData(true, false, "not_a_and_not_b", false)]

        [InlineData(false, true, "not_a_or_b", true)]
        [InlineData(true, false, "not_a_or_b", false)]

        [InlineData(false, true, "not_a_or_not_b", true)]
        [InlineData(true, true, "not_a_or_not_b", false)]

        [InlineData(true, true, "operation_inconnue", false)]
        public void Convert_AppliqueLOperationLogiqueCorrectement(bool ope1, bool ope2, string operation, bool expected)
        {
            var converter = new BoolOperationConverter();
            var values = new object[] { ope1, ope2 };

            var result = converter.Convert(values, typeof(bool), operation, CultureInfo.InvariantCulture);

            result.Should().Be(expected);
        }

        [Fact]
        public void Convert_DonneesInvalides_RetourneFalse()
        {
            var converter = new BoolOperationConverter();

            // Moins de 2 arguments
            converter.Convert(new object[] { true }, typeof(bool), "a_and_b", CultureInfo.InvariantCulture).Should().Be(false);

            // Pas de paramètre d'opération
            converter.Convert(new object[] { true, true }, typeof(bool), null!, CultureInfo.InvariantCulture).Should().Be(false);

            // Mauvais types
            converter.Convert(new object[] { "texte", 123 }, typeof(bool), "a_and_b", CultureInfo.InvariantCulture).Should().Be(false);
        }
    }
}