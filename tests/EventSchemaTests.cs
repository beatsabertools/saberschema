using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json;
using Manatee.Json.Schema;
using Xunit;

namespace tests
{
    public class EventSchemaTests
    {
        private readonly JsonSchema schema;
        private readonly JsonObject json;

        public EventSchemaTests()
        {
            JsonSchemaOptions.Download = uri =>
            {
                var localUri = uri.Replace("https://raw.githubusercontent.com/ryanwersal/saberschema/master/", string.Empty);
                return File.ReadAllText(Path.Combine("..", "..", "..", "..", localUri));
            };

            schema = JsonSchemaRegistry.Get("schemas/event.schema.json");

            json = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "_time", new JsonValue(0) },
                { "_type", new JsonValue(0) },
                { "_value", new JsonValue(0) }
            });
        }

        [Fact]
        public void Schema_IsValid()
        {
            schema.ValidateSchema();
        }

        [Fact]
        public void Time_LessThanZero_IsInvalid()
        {
            json["_time"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void Time_ZeroOrGreater_IsValid(int value)
        {
            json["_time"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(14)]
        public void Type_InvalidValue_IsInvalid(int value)
        {
            json["_type"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(12)]
        [InlineData(13)]
        public void Type_ValidValue_IsValid(int value)
        {
            json["_type"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        public void Value_InvalidValue_IsInvalid(int value)
        {
            json["_value"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(50)]
        [InlineData(100)]
        public void Value_ValueValue_IsValid(int value)
        {
            json["_value"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("_time")]
        [InlineData("_type")]
        [InlineData("_value")]
        public void Missing_RequiredField_IsInvalid(string field)
        {
            json.Remove(field);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }
    }
}