#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Xunit;
using FluentAssertions;
using FranceJudo.UI.Wpf.Converters;

namespace FranceJudo.UI.Tests.Wpf.Converters
{
    public class LogicalConvertersTests
    {
        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        #region BoolNotConverter
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void BoolNotConverter_InverseLeBooleen(bool input, bool expected)
        {
            var converter = new BoolNotConverter();
            converter.Convert(input, typeof(bool), null!, _culture).Should().Be(expected);
            converter.ConvertBack(input, typeof(bool), null!, _culture).Should().Be(expected);
        }

        [Fact]
        public void BoolNotConverter_ValeurNonBooleenne_RetourneNull()
        {
            var converter = new BoolNotConverter();
            converter.Convert("Texte", typeof(bool), null!, _culture).Should().BeNull();
            converter.ConvertBack(123, typeof(bool), null!, _culture).Should().BeNull();
        }
        #endregion

        #region BoolToVisibilityConverter
        [Theory]
        [InlineData(true, null, Visibility.Visible)]
        [InlineData(false, null, Visibility.Collapsed)]
        [InlineData(true, "not", Visibility.Collapsed)]
        [InlineData(false, "not", Visibility.Visible)]
        [InlineData(true, "NOT", Visibility.Collapsed)] // Test de la casse
        public void BoolToVisibilityConverter_ConvertitCorrectement(bool input, string? param, Visibility expected)
        {
            var converter = new BoolToVisibilityConverter();
            converter.Convert(input, typeof(Visibility), param!, _culture).Should().Be(expected);
        }

        [Fact]
        public void BoolToVisibilityConverter_ValeurNonBooleenne_RetourneVisibleParDefaut()
        {
            var converter = new BoolToVisibilityConverter();
            converter.Convert("Erreur", typeof(Visibility), null!, _culture).Should().Be(Visibility.Visible);
            converter.ConvertBack(Visibility.Visible, typeof(bool), null!, _culture).Should().BeNull();
        }
        #endregion

        #region ValueToBoolConverter
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        [InlineData("Un texte quelconque", true)]
        [InlineData(12345, true)]
        [InlineData(null, false)]
        public void ValueToBoolConverter_EvalueLaPresenceDeValeur(object? input, bool expected)
        {
            var converter = new ValueToBoolConverter();
            converter.Convert(input!, typeof(bool), null!, _culture).Should().Be(expected);
        }

        [Fact]
        public void ValueToBoolConverter_ConvertBack_LeveException()
        {
            var converter = new ValueToBoolConverter();
            Action act = () => converter.ConvertBack(true, typeof(object), null!, _culture);
            act.Should().Throw<NotImplementedException>();
        }
        #endregion

        #region ListIntToStringConverter
        [Fact]
        public void ListIntToStringConverter_ListeValide_TrieEtConcatene()
        {
            var converter = new ListIntToStringConverter();
            var input = new List<int> { 5, 1, 10, 2 };

            var result = converter.Convert(input, typeof(string), null!, _culture);

            result.Should().Be("1, 2, 5, 10");
        }

        [Fact]
        public void ListIntToStringConverter_ListeVideOuNulle_RetourneAucun()
        {
            var converter = new ListIntToStringConverter();

            converter.Convert(new List<int>(), typeof(string), null!, _culture).Should().Be("Aucun");
            converter.Convert(null!, typeof(string), null!, _culture).Should().Be("Aucun");
            converter.Convert("Mauvais type", typeof(string), null!, _culture).Should().Be("Aucun");
        }
        #endregion

        #region PassthroughConverter
        [Fact]
        public void PassthroughConverter_AssocieLesDeuxPremiersElements()
        {
            var converter = new PassthroughConverter();
            var values = new object[] { "ObjetA", 42, "Ignoré" };

            var result = converter.Convert(values, typeof(Tuple<object, object>), null!, _culture) as Tuple<object, object>;

            result.Should().NotBeNull();
            result!.Item1.Should().Be("ObjetA");
            result.Item2.Should().Be(42);
        }
        #endregion
    }
}