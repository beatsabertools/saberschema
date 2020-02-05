from hypothesis import Verbosity, settings


settings.register_profile(
    "dev", parent=settings.default, deadline=None, max_examples=1000, verbosity=Verbosity.debug,
)
settings.load_profile("dev")
