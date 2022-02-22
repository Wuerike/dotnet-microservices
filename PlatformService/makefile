SHELL := /bin/bash

unit-test:
	cd tests/unit && dotnet test

local-run:
	cd src && dotnet run

docker-build:
	docker build -t platform-api:v1 .

docker-run:
	docker run -it --rm --network=host platform-api:v1
