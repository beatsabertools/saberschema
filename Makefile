.PHONY: init update validate check repl tests format lint lint-ci clean nuke

init:
	poetry install

update:
	poetry update
	poetry lock

validate: check lint-ci tests

check:
	poetry check
	poetry run isort --recursive --check-only .
	poetry run black --check .

repl:
	poetry run python -m ptpython

tests:
	poetry run pytest --numprocesses=auto

format:
	poetry run isort --recursive .
	poetry run black .

lint:
	poetry run flake8

lint-ci:
	poetry run flake8

clean:
	git clean -xd --force --exclude .vscode

nuke:
	git clean -xd --force
