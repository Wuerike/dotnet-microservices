SHELL := /bin/bash

unit-test:
	cd tests/unit && dotnet test

local-run:
	set -a && source .env && set +a && cd src && dotnet run

docker-build:
	docker build -t command-api:v1 .

docker-run:
	docker run -it --rm --network=host command-api:v1
