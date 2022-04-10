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
	kubectl apply -f .kubernetes/rabbitmq-depl.yaml

stop:
	kubectl delete deployments platform-api-depl mssql-platform-depl command-api-depl rabbitmq-depl
	kubectl delete services command-clusterip-srv mssql-clusterip-srv mssql-np-srv  platform-clusterip-srv platform-np-srv rabbitmq-clusterip-srv rabbitmq-loadbalancer
	kubectl delete ingress ingress-srv

delete:
	minikube image rm docker.io/wuerike/platform-api:v1
	minikube image rm docker.io/wuerike/command-api:v1
