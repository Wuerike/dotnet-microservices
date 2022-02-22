SHELL := /bin/bash

unit-test:
	cd tests/unit && dotnet test

local-run:
	cd src && dotnet run

docker-build:
	docker build -t command-api:v1 .

docker-run:
	docker run -it --rm --network=host command-api:v1

kube-deploy:
	kubectl apply -f .kubernetes/commands-api.yaml
