# elta
minikube start
kubectl create namespace devops
kubectl apply -f .
kubectl get pods -n devops
kubectl logs <Jenkins-Pod> to retrive the password

# Install ngrok to expose jenkins publicity to be able to get a webhook from github
curl -sSL https://ngrok-agent.s3.amazonaws.com/ngrok.asc \
	| sudo tee /etc/apt/trusted.gpg.d/ngrok.asc >/dev/null \
	&& echo "deb https://ngrok-agent.s3.amazonaws.com buster main" \
	| sudo tee /etc/apt/sources.list.d/ngrok.list \
	&& sudo apt update \
	&& sudo apt install ngrok

ngrok config add-authtoken 2v5TUkePJkYCmHf8JgH9D3PN5JU_6qzUnd4W4NhuEGrcMBVC7
ngrok http http://minikubeip:NodePort
