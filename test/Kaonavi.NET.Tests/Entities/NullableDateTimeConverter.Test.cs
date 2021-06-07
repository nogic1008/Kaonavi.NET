using System;
using System.Globalization;
using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// <see cref="NullableDateTimeConverter"/>の単体テスト
    /// </summary>
    public class NullableDateTimeConverterTest
    {
        private static readonly JsonSerializerOptions _options;
        static NullableDateTimeConverterTest()
        {
            _options = new(JsonSerializerDefaults.Web);
            _options.Converters.Add(new NullableDateTimeConverter());
        }

        private record TestRecord(DateTime? Date);

        /// <summary>
        /// JSON形式に正しくシリアライズできる。
        /// </summary>
        /// <param name="dateTimeString"><see cref="DateTime"/>の文字列表現</param>
        /// <param name="expectedJson">JSON文字列</param>
        [Theory]
        [InlineData(null, "{\"date\":null}")]
        [InlineData("2020/01/01", "{\"date\":\"2020-01-01\"}")]
        [InlineData("2020/01/01 5:00:00", "{\"date\":\"2020-01-01 05:00:00\"}")]
        public void CanSerializeJSON(string? dateTimeString, string expectedJson)
        {
            var record = new TestRecord(
                dateTimeString is null ? null : DateTime.Parse(dateTimeString, CultureInfo.InvariantCulture)
            );

            string json = JsonSerializer.Serialize(record, _options);

            json.Should().Be(expectedJson);
        }

        /// <summary>
        /// JSON形式から正しくデシリアライズできる。
        /// </summary>
        /// <param name="json">JSON文字列</param>
        /// <param name="expectedString"><see cref="DateTime"/>の文字列表現</param>
        [Theory]
        [InlineData("{\"date\":null}", null)]
        [InlineData("{\"date\":\"\"}", null)]
        [InlineData("{\"date\":\"2020-01-01\"}", "01/01/2020 00:00:00")]
        [InlineData("{\"date\":\"2020-01-01 05:00:00\"}", "01/01/2020 05:00:00")]
        public void CanDeserializeJSON(string json, string? expectedString)
        {
            var record = JsonSerializer.Deserialize<TestRecord?>(json, _options)!;

            if (expectedString is null)
                record.Date.Should().BeNull();
            else
                record.Date.GetValueOrDefault().ToString(CultureInfo.InvariantCulture).Should().Be(expectedString);
        }
    }
}
