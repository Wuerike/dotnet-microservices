SHELL := /bin/bash

test:
	cd tests/unit && dotnet test

run:
	set -a && source .env && set +a && cd src && dotnet run