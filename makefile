SHELL := /bin/bash

deploy:
	kubectl apply -f .kubernetes/platforms-api.yaml
	kubectl apply -f .kubernetes/commands-api.yaml
	kubectl apply -f .kubernetes/platforms-np-srv.yaml
