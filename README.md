# ELTA Project

Welcome to the ELTA project! This repository contains configurations for setting up a Jenkins environment on a **three-node Minikube cluster (Docker driver)**, deploying a sample application, and exposing Jenkins to the public internet using **ngrok** for GitHub webhook integration.

## Prerequisites

Before you begin, ensure you have the following installed:

- [Minikube](https://minikube.sigs.k8s.io/docs/start/)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)
- [ngrok](https://ngrok.com/download)
- [Docker](https://www.docker.com/get-started)

## Setup Instructions

### 1. Start Minikube with Multiple Nodes

Since this setup uses the Docker driver and three nodes, start Minikube with:

```bash
minikube start --driver=docker --nodes=3
```

### 2. Create a Namespace

Create a new namespace named `devops`:

```bash
kubectl create namespace devops
```

### 3. Deploy Jenkins

Apply the deployment configuration for Jenkins:
Make sure to CD to jenkins folder

```bash
kubectl apply -f .
```

### 4. Verify Deployment

Check if all nodes and pods are running:

```bash
kubectl get nodes
kubectl get pods -n devops
```

Retrieve the Jenkins administrator password:

```bash
kubectl logs <jenkins-pod-name> -n devops
```

Replace `<jenkins-pod-name>` with the actual name of your Jenkins pod.

## Exposing Jenkins with ngrok

To enable GitHub webhooks, expose Jenkins to the public internet using **ngrok**.

### 1. Install and Configure ngrok

```bash
ngrok config add-authtoken <your-ngrok-auth-token>
```

Replace `<your-ngrok-auth-token>` with your actual ngrok authentication token from your [ngrok dashboard](https://dashboard.ngrok.com/get-started/your-authtoken).

### 2. Get Minikube's NodePort for Jenkins

Check the service details:

```bash
kubectl get svc jenkins -n devops
```

Look for the **NodePort** assigned to Jenkins.

Get the Minikube cluster's IP:

```bash
minikube ip
```

### 3. Start ngrok

Expose Jenkins to the internet by running:

```bash
ngrok http <minikube-ip>:<node-port>
```

Replace `<minikube-ip>` with the output of `minikube ip`, and `<node-port>` with the NodePort from the service details.

ngrok will generate a public URL like `https://random-string.ngrok.io`.

## Configuring GitHub Webhooks

1. Go to **GitHub Repository → Settings → Webhooks → Add webhook**.
2. Set the **Payload URL** to your ngrok URL followed by `/github-webhook/`.
   Example: `https://random-string.ngrok.io/github-webhook/`
3. Set **Content type** to `application/json`.
4. Choose **Just the push event** (or select relevant triggers).
5. Click **Add webhook**.

## Repository Structure

- `HelloWorldApp/` - Sample application code.
- `jenkins/` - Jenkins deployment configurations.
- `Jenkinsfile` - Defines the pipeline for CI/CD.
- `helloworldapp.yaml` - Kubernetes deployment & service for the sample app.

---
With this setup, Jenkins is running on a **multi-node Minikube cluster (Docker driver)**, accessible via **ngrok**, and integrated with **GitHub webhooks** for automated builds.
# Jenkins CI/CD Pipeline for ELTA Project

This Jenkins pipeline automates the process of building, pushing, and deploying the HelloWorldApp to a **Minikube Kubernetes cluster** using Docker.

## Pipeline Overview

The pipeline consists of the following stages:

1. **Clone Repository** - Fetches the latest code from GitHub.
2. **Build Docker Image** - Builds and pushes the Docker image to Docker Hub.
3. **Deploy to Build Namespace** - Deploys the application to a build namespace in Kubernetes.
4. **Move to Production Namespace** - Promotes the application to a production namespace in Kubernetes.

## Prerequisites

Before running this pipeline, ensure you have:

- A **Jenkins** setup with the **Kubernetes plugin**.
- A **Minikube cluster** with **three nodes** (Docker driver).
- A **Docker Hub** account for pushing images.
- Proper **Kubernetes configurations** (`kubectl` configured to access the cluster).
- Jenkins credentials for **GitHub** and **Docker Hub**.

### Agent Configuration

The pipeline runs inside a **Kubernetes agent pod** labeled `jenkins-agent`.
