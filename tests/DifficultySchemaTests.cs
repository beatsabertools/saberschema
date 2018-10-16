using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json;
using Manatee.Json.Schema;
using Xunit;

namespace tests
{
    public class DifficultySchemaTests
    {
        private readonly JsonSchema schema;
        private readonly JsonObject json;

        public DifficultySchemaTests()
        {
            JsonSchemaOptions.Download = uri =>
            {
                var localUri = uri.Replace("https://raw.githubusercontent.com/ryanwersal/saberschema/master/", string.Empty);
                return File.ReadAllText(Path.Combine("..", "..", "..", "..", localUri));
            };

            schema = JsonSchemaRegistry.Get("schemas/difficulty.schema.json");

            json = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "difficulty", new JsonValue("Easy") },
                { "difficultyRank", new JsonValue(0) },
                { "jsonPath", new JsonValue("path") }
            });
        }

        [Fact]
        public void Schema_IsValid()
        {
            var results = schema.ValidateSchema();
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("")]
        [InlineData("foo")]
        [InlineData("easy")]
        [InlineData("expert")]
        public void Difficulty_InvalidValue_IsInvalid(string value)
        {
            json["difficulty"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("Easy")]
        [InlineData("Normal")]
        [InlineData("Hard")]
        [InlineData("Expert")]
        [InlineData("Expert+")]
        public void Difficulty_ValidValue_IsValid(string value)
        {
            json["difficulty"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5)]
        [InlineData(100)]
        public void DifficultyRank_InvalidValue_IsInvalid(int value)
        {
            json["difficultyRank"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void DifficultyRank_ValidValue_IsValid(int value)
        {
            json["difficultyRank"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("difficulty")]
        [InlineData("difficultyRank")]
        [InlineData("jsonPath")]
        public void Missing_RequiredField_IsInvalid(string field)
        {
            json.Remove(field);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }
    }
}