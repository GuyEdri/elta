pipeline {
     agent {
        kubernetes {
            yaml """
apiVersion: v1
kind: Pod
metadata:
  labels:
    jenkins-agent: "true"
spec:
  containers:
    - name: docker-builder
      image: docker:24.0
      command:
        - cat
      tty: true
      volumeMounts:
        - name: docker-socket
          mountPath: /var/run/docker.sock
  volumes:
    - name: docker-socket
      hostPath:
        path: /var/run/docker.sock
"""
        }
    }

    environment {
        DOCKER_IMAGE = 'guyedri/helloworldapp:latest'  // Replace with your Docker Hub image
        BUILD_NAMESPACE = 'build'  // The namespace where the build will occur
        PROD_NAMESPACE = 'production'  // The namespace where the application will be deployed to production
    }

    stages {
        stage('Clone Repository') {
            steps {
                script {
                    // Checkout the Git repository
                    checkout scm
                }
            }
        }
    stage('Build Docker Image') {
    	steps {
       		script {
                    // Navigate to the app directory
                    dir('HelloWorldApp') {
            	    sh 'docker build -t $DOCKER_IMAGE .'

            // Log in to Docker Hub using Jenkins credentials
            withCredentials([usernamePassword(credentialsId: 'dockerhub', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                // Log in to Docker Hub
                sh 'echo $DOCKER_PASSWORD | docker login -u $DOCKER_USERNAME --password-stdin'
            }

            // Push the Docker image to the registry
            sh 'docker push $DOCKER_IMAGE'
        }
    }
 }
}
        stage('Deploy to Build Namespace') {
            steps {
                script {
                    // Ensure the build namespace exists
                    sh "kubectl get namespace $BUILD_NAMESPACE || kubectl create namespace $BUILD_NAMESPACE"
                    
                    // Deploy to the build namespace
                    sh """
                    kubectl apply -f helloworldapp.yaml -n $BUILD_NAMESPACE
                    """
                    
                    // Wait for the deployment to complete
                    sh """
                    kubectl rollout status deployment/helloworldapp -n $BUILD_NAMESPACE
                    """
                }
            }
        }

        stage('Move to Production Namespace') {
            steps {
                script {
                    // Ensure the production namespace exists
                    sh "kubectl get namespace $PROD_NAMESPACE || kubectl create namespace $PROD_NAMESPACE"
                    
                    // Deploy to the production namespace
                    sh """
                    kubectl apply -f helloworldapp.yaml -n $PROD_NAMESPACE
                    """
                    
                    // Wait for the deployment to complete in the production namespace
                    sh """
                    kubectl rollout status deployment/helloworldapp -n $PROD_NAMESPACE
                    """
                }
            }
        }
    }

    post {
        success {
            echo 'Deployment was successful!'
        }
        failure {
            echo 'Deployment failed.'
        }
    }
}

