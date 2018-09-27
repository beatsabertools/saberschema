using System;
using System.Collections.Generic;
using System.IO;
using Manatee.Json;
using Manatee.Json.Schema;
using Xunit;

namespace tests
{
    public class BeatmapSchemaTests
    {
        private readonly JsonSchema schema;
        private readonly JsonObject json;

        public BeatmapSchemaTests()
        {
            JsonSchemaOptions.Download = uri =>
            {
                var localUri = uri.Replace("https://raw.githubusercontent.com/ryanwersal/saberschema/master/", string.Empty);
                return File.ReadAllText(Path.Combine("..", "..", "..", "..", localUri));
            };

            schema = JsonSchemaRegistry.Get("schemas/beatmap.schema.json");

            json = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "_version", new JsonValue(1.5) },
                { "_beatsPerMinute", new JsonValue(0) },
                { "_beatsPerBar", new JsonValue(0) },
                { "_shuffle", new JsonValue(0) },
                { "_shufflePeriod", new JsonValue(0) },
                { "_noteJumpSpeed", new JsonValue(0) },
                { "_events", new JsonArray() },
                { "_notes", new JsonArray() },
                { "_obstacles", new JsonArray() }
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
            json["_beatsPerMinute"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void BeatsPerMinute_ZeroOrGreater_IsValid(int value)
        {
            json["_beatsPerMinute"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void BeatsPerBar_LessThanZero_IsInvalid()
        {
            json["_beatsPerBar"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void BeatsPerBar_ZeroOrGreater_IsValid(int value)
        {
            json["_beatsPerBar"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Shuffle_LessThanZero_IsInvalid()
        {
            json["_shuffle"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void Shuffle_ZeroOrGreater_IsValid(int value)
        {
            json["_shuffle"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void ShufflePeriod_LessThanZero_IsInvalid()
        {
            json["_shufflePeriod"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void ShufflePeriod_ZeroOrGreater_IsValid(int value)
        {
            json["_shufflePeriod"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void NoteJumpSpeed_LessThanZero_IsInvalid()
        {
            json["_noteJumpSpeed"] = new JsonValue(-1);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(100)]
        public void NoteJumpSpeed_ZeroOrGreater_IsValid(int value)
        {
            json["_noteJumpSpeed"] = new JsonValue(value);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Events_ValidEvent_IsValid()
        {
            var eventObj = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "_time", new JsonValue(0) },
                { "_type", new JsonValue(0) },
                { "_value", new JsonValue(0) }
            });

            json["_events"].Array.Add(eventObj);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Notes_ValidNote_IsValid()
        {
            var note = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "_lineLayer", new JsonValue(0) },
                { "_lineIndex", new JsonValue(0) },
                { "_type", new JsonValue(0) },
                { "_time", new JsonValue(0) },
                { "_cutDirection", new JsonValue(0) }
            });

            json["_notes"].Array.Add(note);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Obstacles_ValidObstacle_IsValid()
        {
            var obstacle = new JsonObject(new Dictionary<string, JsonValue>
            {
                { "_time", new JsonValue(0) },
                { "_lineIndex", new JsonValue(0) },
                { "_type", new JsonValue(0) },
                { "_duration", new JsonValue(0) },
                { "_width", new JsonValue(1) }
            });

            json["_obstacles"].Array.Add(obstacle);

            var result = schema.Validate(json);
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("_version")]
        [InlineData("_beatsPerMinute")]
        [InlineData("_beatsPerBar")]
        [InlineData("_shuffle")]
        [InlineData("_shufflePeriod")]
        [InlineData("_noteJumpSpeed")]
        [InlineData("_notes")]
        public void Missing_RequiredField_IsInvalid(string field)
        {
            json.Remove(field);

            var result = schema.Validate(json);
            Assert.False(result.IsValid);
        }
    }
}