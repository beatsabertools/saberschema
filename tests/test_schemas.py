import json
import os

import pytest
from hypothesis import given, strategies as st
from jsonschema import Draft7Validator, exceptions


@pytest.fixture
def schema_validator():
    def _load_schema(schema_name):
        # Load already created validator if one exists.
        if hasattr(schema_validator, schema_name):
            return getattr(schema_validator, schema_name)

        # Read the schema and create a validator.
        with open(os.path.join("schemas", f"{schema_name}.schema.json")) as f:
            validator = Draft7Validator(json.loads(f.read()))

        setattr(schema_validator, schema_name, validator)
        return validator

    return _load_schema


DIFFICULTIES = {"Easy": 0, "Normal": 1, "Hard": 2, "Expert": 3, "Expert+": 4}


@st.composite
def valid_difficulty(draw):
    return {
        "difficulty": draw(st.sampled_from(sorted(DIFFICULTIES.keys()))),
        "difficultyRank": draw(st.sampled_from(sorted(DIFFICULTIES.values()))),
        "jsonPath": draw(st.text()),
    }


@st.composite
def invalid_difficulty(draw):
    return {
        "difficulty": draw(st.text().filter(lambda x: x not in DIFFICULTIES.keys())),
        "difficultyRank": draw(st.integers().filter(lambda x: x not in DIFFICULTIES.values())),
        "jsonPath": draw(st.text()),
    }


@pytest.fixture
def difficulty_schema(schema_validator):
    return schema_validator("difficulty")


@given(data=valid_difficulty())
def test_difficulty_schema_valid(difficulty_schema, data):
    difficulty_schema.validate(data)


@given(data=invalid_difficulty())
def test_difficulty_schema_invalid(difficulty_schema, data):
    with pytest.raises(exceptions.ValidationError):
        difficulty_schema.validate(data)


@st.composite
def valid_event(draw):
    return {
        "_time": draw(st.integers(min_value=0)),
        "_type": draw(st.integers(min_value=0, max_value=13).filter(lambda x: x not in {5, 6, 7, 10, 11})),
        "_value": draw(st.integers(min_value=0, max_value=7).filter(lambda x: x not in {4})),
    }


@st.composite
def invalid_event(draw):
    return {
        "_time": draw(st.integers(max_value=-1)),
        "_type": draw(st.integers().filter(lambda x: x < 0 or x > 13)),
        "_value": draw(st.integers().filter(lambda x: x < 0 or x > 7)),
    }


@pytest.fixture
def event_schema(schema_validator):
    return schema_validator("event")


@given(data=valid_event())
def test_event_schema_valid(event_schema, data):
    event_schema.validate(data)


@given(data=invalid_event())
def test_event_schema_invalid(event_schema, data):
    with pytest.raises(exceptions.ValidationError):
        event_schema.validate(data)


ENVIRONMENTS = {"DefaultEnvironment", "BigMirrorEnvironment", "TriangleEnvironment", "NiceEnvironment"}


@st.composite
def valid_info(draw):
    return {
        "songName": draw(st.text()),
        "songSubName": draw(st.text()),
        "songAuthorName": draw(st.text()),
        "beatsPerMinute": draw(st.integers(min_value=0)),
        "previewStartTime": draw(st.integers(min_value=0)),
        "previewDuration": draw(st.integers(min_value=0)),
        "audioPath": draw(st.text()),
        "coverImagePath": draw(st.text()),
        "oneSaber": draw(st.booleans()),
        "noteHitVolume": draw(st.floats(min_value=0.0, max_value=1.0)),
        "noteMissVolume": draw(st.floats(min_value=0.0, max_value=1.0)),
        "environmentName": draw(st.sampled_from(sorted(ENVIRONMENTS))),
        "difficultyLevels": draw(st.lists(valid_difficulty())),
    }


@st.composite
def invalid_info(draw):
    return {
        "songName": draw(st.none()),
        "songSubName": draw(st.none()),
        "songAuthorName": draw(st.none()),
        "beatsPerMinute": draw(st.integers(max_value=-1)),
        "previewStartTime": draw(st.integers(max_value=-1)),
        "previewDuration": draw(st.integers(max_value=-1)),
        "audioPath": draw(st.none()),
        "coverImagePath": draw(st.none()),
        "oneSaber": draw(st.none()),
        "noteHitVolume": draw(st.floats().filter(lambda x: x < 0 or x > 1)),
        "noteMissVolume": draw(st.floats().filter(lambda x: x < 0 or x > 1)),
        "environmentName": draw(st.text().filter(lambda x: x not in ENVIRONMENTS)),
        "difficultyLevels": draw(st.lists(invalid_difficulty())),
    }


@pytest.fixture
def info_schema(schema_validator):
    return schema_validator("info")


@given(data=valid_info())
def test_info_schema_valid(info_schema, data):
    info_schema.validate(data)


@given(data=invalid_info())
def test_info_schema_invalid(info_schema, data):
    with pytest.raises(exceptions.ValidationError):
        info_schema.validate(data)


@st.composite
def valid_note(draw):
    return {
        "_lineLayer": draw(st.integers(min_value=0, max_value=2)),
        "_lineIndex": draw(st.integers(min_value=0, max_value=4)),
        "_type": draw(st.integers(min_value=0, max_value=3).filter(lambda x: x != 2)),
        "_time": draw(st.integers(min_value=0)),
        "_cutDirection": draw(st.integers(min_value=0, max_value=8)),
    }


@st.composite
def invalid_note(draw):
    return {
        "_lineLayer": draw(st.integers(max_value=-1)),
        "_lineIndex": draw(st.integers(max_value=-1)),
        "_type": draw(st.integers(max_value=-1)),
        "_time": draw(st.integers(max_value=-1)),
        "_cutDirection": draw(st.integers(max_value=-1)),
    }


@pytest.fixture
def note_schema(schema_validator):
    return schema_validator("note")


@given(data=valid_note())
def test_note_schema_valid(note_schema, data):
    note_schema.validate(data)


@given(data=invalid_note())
def test_note_schema_invalid(note_schema, data):
    with pytest.raises(exceptions.ValidationError):
        note_schema.validate(data)


@st.composite
def valid_obstacle(draw):
    return {
        "_time": draw(st.integers(min_value=0)),
        "_lineIndex": draw(st.integers(min_value=0, max_value=4)),
        "_type": draw(st.integers(min_value=0)),
        "_duration": draw(st.integers(min_value=0)),
        "_width": draw(st.integers(min_value=1, max_value=4)),
    }


@st.composite
def invalid_obstacle(draw):
    return {
        "_time": draw(st.integers(max_value=-1)),
        "_lineIndex": draw(st.integers(max_value=-1)),
        "_type": draw(st.integers(max_value=-1)),
        "_duration": draw(st.integers(max_value=-1)),
        "_width": draw(st.integers(max_value=0)),
    }


@pytest.fixture
def obstacle_schema(schema_validator):
    return schema_validator("obstacle")


@given(data=valid_obstacle())
def test_obstacle_schema_valid(obstacle_schema, data):
    obstacle_schema.validate(data)


@given(data=invalid_obstacle())
def test_obstacle_schema_invalid(obstacle_schema, data):
    with pytest.raises(exceptions.ValidationError):
        obstacle_schema.validate(data)


@st.composite
def version_number(draw):
    major = draw(st.integers())
    minor = draw(st.integers())
    patch = draw(st.integers())
    return f"{major}.{minor}.{patch}"


@st.composite
def valid_beatmap(draw):
    return {
        "_version": draw(st.just("1.5.0")),
        "_beatsPerMinute": draw(st.integers(min_value=0)),
        "_beatsPerBar": draw(st.integers(min_value=0)),
        "_shuffle": draw(st.integers(min_value=0)),
        "_shufflePeriod": draw(st.integers(min_value=0)),
        "_noteJumpSpeed": draw(st.integers(min_value=0)),
        "_events": draw(st.lists(valid_event())),
        "_notes": draw(st.lists(valid_note())),
        "_obstacles": draw(st.lists(valid_obstacle())),
    }


@st.composite
def invalid_beatmap(draw):
    return {
        "_version": draw(version_number().filter(lambda x: x != "1.5.0")),
        "_beatsPerMinute": draw(st.integers(max_value=-1)),
        "_beatsPerBar": draw(st.integers(max_value=-1)),
        "_shuffle": draw(st.integers(max_value=-1)),
        "_shufflePeriod": draw(st.integers(max_value=-1)),
        "_noteJumpSpeed": draw(st.integers(max_value=-1)),
        "_events": draw(st.lists(invalid_event())),
        "_notes": draw(st.lists(invalid_note())),
        "_obstacles": draw(st.lists(invalid_obstacle())),
    }


@pytest.fixture
def beatmap_schema(schema_validator):
    return schema_validator("beatmap")


@given(data=valid_beatmap())
def test_beatmap_schema_valid(beatmap_schema, data):
    beatmap_schema.validate(data)


@given(data=invalid_beatmap())
def test_beatmap_schema_invalid(beatmap_schema, data):
    with pytest.raises(exceptions.ValidationError):
        beatmap_schema.validate(data)
