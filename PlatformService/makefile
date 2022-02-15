SHELL := /bin/bash

test:
	cd src/PlatformApiUnitTests && dotnet test

local-run:
	cd src/PlatformApi && dotnet run

docker-build:
	docker build -t platform-api:v1 .

docker-run:
	docker run -it --rm --network=host platform-api:v1

kube-deploy:
	kubectl apply -f .kubernetes/platforms-api.yaml
