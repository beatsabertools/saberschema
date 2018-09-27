using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json;
using Manatee.Json.Schema;
using Xunit;

namespace tests
{
    public class InfoSchemaTests
    {
        private readonly JsonSchema schema;
        private readonly JsonObject json;

        public InfoSchemaTests()
        {
            JsonSchemaOptions.Download = uri =>
            {
                var localUri = uri.Replace("https://raw.githubusercontent.com/ryanwersal/saberschema/master/", string.Empty);
                return File.ReadAllText(Path.Combine("..", "..", "..", "..", localUri));
            };

            schema = JsonSchemaRegistry.Get("schemas/info.schema.json");

            json = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "songName", new JsonValue("name") },
                { "songSubName", new JsonValue("sub name") },
                { "songAuthorName", new JsonValue("author") },
                { "beatsPerMinute", new JsonValue(0) },
                { "previewStartTime", new JsonValue(0) },
                { "previewDuration", new JsonValue(0) },
                { "audioPath", new JsonValue("audioPath") },
                { "coverImagePath", new JsonValue("path") },
                { "environmentName", new JsonValue("DefaultEnvironment") },
                { "difficultyLevels", new JsonArray() }
            });
        }

        [Fact]
        public void Schema_IsValid()
        {
            schema.ValidateSchema();
        }

        [Fact]
        public void BeatsPerMinute_LessThanZero_IsInvalid()
        {
            json["beatsPerMinute"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void BeatsPerMinute_ZeroOrGreater_IsValid(int value)
        {
            json["beatsPerMinute"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void PreviewStartTime_LessThanZero_IsInvalid()
        {
            json["previewStartTime"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void PreviewStartTime_ZeroOrGreater_IsValid(int value)
        {
            json["previewStartTime"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void PreviewDuration_LessThanZero_IsInvalid()
        {
            json["previewDuration"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void PreviewDuration_ZeroOrGreater_IsValid(int value)
        {
            json["previewDuration"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("")]
        [InlineData("foo")]
        [InlineData("niceenvironment")]
        public void EnvironmentName_InvalidValue_IsInvalid(string value)
        {
            json["environmentName"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("DefaultEnvironment")]
        [InlineData("BigMirrorEnvironment")]
        [InlineData("TriangleEnvironment")]
        [InlineData("NiceEnvironment")]
        public void Environment_ValidValue_IsValid(string value)
        {
            json["environmentName"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void DifficultyLevels_ValidLevel_IsValid()
        {
            var difficultyLevel = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "difficulty", new JsonValue("Easy") },
                { "difficultyRank", new JsonValue(0) },
                { "jsonPath", new JsonValue("path") }
            });

            json["difficultyLevels"].Array.Add(difficultyLevel);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OneSaber_ValidValue_IsValid(bool value)
        {
            json["oneSaber"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("null")]
        [InlineData("10")]
        [InlineData("{}")]
        public void OneSaber_InvalidValue_IsInvalid(string value)
        {
            json["oneSaber"] = JsonValue.Parse(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.3)]
        [InlineData(0.7)]
        [InlineData(1)]
        public void NoteHitVolume_ValidValue_IsValid(float value)
        {
            json["noteHitVolume"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1.1)]
        [InlineData(2)]
        public void NoteHitVolume_InvalidValue_IsInvalid(float value)
        {
            json["noteHitVolume"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(0.3)]
        [InlineData(0.7)]
        [InlineData(1)]
        public void NoteMissVolume_ValidValue_IsValid(float value)
        {
            json["noteMissVolume"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1.1)]
        [InlineData(2)]
        public void NoteMissVolume_InvalidValue_IsInvalid(float value)
        {
            json["noteMissVolume"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData("songName")]
        [InlineData("songSubName")]
        [InlineData("songAuthorName")]
        [InlineData("beatsPerMinute")]
        [InlineData("previewStartTime")]
        [InlineData("previewDuration")]
        [InlineData("audioPath")]
        [InlineData("coverImagePath")]
        [InlineData("environmentName")]
        [InlineData("difficultyLevels")]
        public void Missing_RequiredField_IsInvalid(string field)
        {
            json.Remove(field);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }
    }
}