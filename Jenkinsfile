pipeline {
    agent any
    environment {
        REGISTRY = 'guyedri'  // Change to your Docker Hub username
        IMAGE_NAME = 'helloworldapp'  // The name of your Docker image
        DOCKER_IMAGE = "${REGISTRY}/${IMAGE_NAME}:latest"
    }
    stages {
        stage('Checkout') {
            steps {
                // Checkout the code from GitHub
                git 'https://github.com/GuyEdri/elta.git'
            }
        }

        stage('Build Docker Image') {
            steps {
                script {
                    // Build the Docker image for the .NET Core application
		    sh 'cd HelloWorldApp'
                    sh 'docker build -t ${DOCKER_IMAGE} .'
                }
            }
        }

        stage('Push Docker Image to Registry') {
            steps {
                script {
                    // Log in to Docker Hub and push the image
                    withDockerRegistry([credentialsId: 'dockerhub', url: 'https://index.docker.io/v1/']) {
                        sh 'docker push ${DOCKER_IMAGE}'
                    }
                }
            }
        }

        stage('Deploy to Kubernetes') {
            steps {
                script {
                    // Use kubectl to deploy the application into the `deploy-namespace`
                    sh """
                        kubectl --namespace=deploy set image deployment/helloworldapp helloworldapp=${DOCKER_IMAGE}
                        kubectl --namespace=deploy rollout status deployment/helloworldapp
                    """
                }
            }
        }
    }
    post {
        success {
            echo 'Deployment to Kubernetes was successful!'
        }
        failure {
            echo 'Something went wrong. Please check the logs.'
        }
    }
}

