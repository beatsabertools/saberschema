using System;
using System.IO;
using Xunit;
using Manatee.Json;
using Manatee.Json.Schema;
using System.Collections.Generic;

namespace tests
{
    public class NoteSchemaTests
    {
        private readonly JsonSchema schema;
        private readonly JsonObject json;

        public NoteSchemaTests()
        {
            JsonSchemaOptions.Download = uri => {
                var localUri = uri.Replace("https://raw.githubusercontent.com/ryanwersal/saberschema/master/", string.Empty);
                return File.ReadAllText(Path.Combine("..", "..", "..", "..", localUri));
            };

            schema = JsonSchemaRegistry.Get("schemas/note.schema.json");

            json = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "_lineLayer", new JsonValue(0) },
                { "_lineIndex", new JsonValue(0) },
                { "_type", new JsonValue(0) },
                { "_time", new JsonValue(0) },
                { "_cutDirection", new JsonValue(0) }
            });
        }

        [Fact]
        public void ShouldBeValidSchema()
        {
            var results = schema.ValidateSchema();
            Assert.Empty(results);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(3)]
        public void LineLayer_InvalidValues_IsInvalid(int value)
        {
            json["_lineLayer"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void LineLayer_BetweenZeroAndTwo_IsValid(int value)
        {
            json["_lineLayer"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        public void LineIndex_InvalidValues_IsInvalid(int value)
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

        [Theory]
        [InlineData(-1)]
        [InlineData(2)]
        [InlineData(10)]
        public void Type_InvalidValues_IsInvalid(int value)
        {
            json["_type"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void Type_ValidValues_IsValid(int value)
        {
            json["_type"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(9)]
        public void CutDirection_InvalidValues_IsInvalid(int value)
        {
            json["_cutDirection"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void CutDirection_ValidValues_IsValid(int value)
        {
            json["_cutDirection"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("_lineLayer")]
        [InlineData("_lineIndex")]
        [InlineData("_type")]
        [InlineData("_time")]
        [InlineData("_cutDirection")]
        public void Missing_RequiredField_IsInvalid(string field)
        {
            json.Remove(field);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }
    }
}
