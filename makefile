SHELL := /bin/bash

kube-secrets:
	kubectl delete secret kube-env --ignore-not-found
	kubectl create secret generic kube-env --from-env-file=.env

deploy:
	make kube-secrets
	kubectl apply -f .kubernetes/platforms-api.yaml
	kubectl apply -f .kubernetes/commands-api.yaml
	kubectl apply -f .kubernetes/ingress-srv.yaml
	kubectl apply -f .kubernetes/local-pvc.yaml
	kubectl apply -f .kubernetes/mssql-platform-depl.yaml
