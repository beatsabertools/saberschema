using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json;
using Manatee.Json.Schema;
using Xunit;

namespace tests
{
    public class ObstacleSchemaTests
    {
        private readonly JsonSchema schema;
        private readonly JsonObject json;

        public ObstacleSchemaTests()
        {
            JsonSchemaOptions.Download = uri => {
                var localUri = uri.Replace("https://raw.githubusercontent.com/ryanwersal/saberschema/master/", string.Empty);
                return File.ReadAllText(Path.Combine("..", "..", "..", "..", localUri));
            };

            schema = JsonSchemaRegistry.Get("schemas/obstacle.schema.json");

            json = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "_time", new JsonValue(0) },
                { "_lineIndex", new JsonValue(0) },
                { "_type", new JsonValue(0) },
                { "_duration", new JsonValue(0) },
                { "_width", new JsonValue(1) }
            });
        }

        [Fact]
        public void Schema_IsValid()
        {
            var results = schema.ValidateSchema();
            Assert.Empty(results);
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
        public void LineIndex_InvalidValue_IsInvalid(int value)
        {
            json["_lineIndex"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void LineIndex_ValidValues_IsValid(int value)
        {
            json["_lineIndex"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Type_LessThanZero_IsInvalid()
        {
            json["_type"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        // TODO: Add test for _type once valid values are figured out.

        [Fact]
        public void Duration_LessThanZero_IsInvalid()
        {
            json["_duration"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void Duration_ZeroOrGreater_IsValid(int value)
        {
            json["_duration"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Width_LessThanOne_IsInvalid()
        {
            json["_width"] = new JsonValue(0);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void Width_GreaterThanFour_IsInvalid()
        {
            json["_width"] = new JsonValue(5);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void Width_ValidValue_IsValid(int value)
        {
            json["_width"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("_time")]
        [InlineData("_lineIndex")]
        [InlineData("_type")]
        [InlineData("_duration")]
        [InlineData("_width")]
        public void Missing_RequiredField_IsInvalid(string field)
        {
            json.Remove(field);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }
    }
}
