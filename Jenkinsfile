pipeline {
    agent {
        kubernetes {
            yaml """
apiVersion: v1
kind: Pod
metadata:
  labels:
    jenkins/agent: "true"
spec:
  containers:
  - name: jnlp
    image: jenkins/inbound-agent:latest
    args: ['\${computer.jnlpmac}', '\${computer.name}']
    volumeMounts:
      - mountPath: /home/jenkins/agent
        name: workspace
  - name: docker
    image: docker:20.10
    command: [ "sleep", "infinity" ]  # Keeps container alive for executing commands
    volumeMounts:
      - mountPath: /var/run/docker.sock
        name: docker-socket
  volumes:
    - name: workspace
      emptyDir: {}
    - name: docker-socket
      hostPath:
        path: /var/run/docker.sock
        type: Socket
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
                    checkout scm
                }
            }
        }

        stage('Build Docker Image') {
            steps {
                container('docker') { // Run this stage inside the 'docker' container
                    script {
                        dir('HelloWorldApp') {
                            sh 'docker build -t $DOCKER_IMAGE .'

                            withCredentials([usernamePassword(credentialsId: 'dockerhub', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                                sh 'echo $DOCKER_PASSWORD | docker login -u $DOCKER_USERNAME --password-stdin'
                            }

                            sh 'docker push $DOCKER_IMAGE'
                        }
                    }
                }
            }
        }

        stage('Deploy to Build Namespace') {
            steps {
                script {
                    sh "kubectl get namespace $BUILD_NAMESPACE || kubectl create namespace $BUILD_NAMESPACE"
                    sh "kubectl apply -f helloworldapp.yaml -n $BUILD_NAMESPACE"
                    sh "kubectl rollout status deployment/helloworldapp -n $BUILD_NAMESPACE"
                }
            }
        }

        stage('Move to Production Namespace') {
            steps {
                script {
                    sh "kubectl get namespace $PROD_NAMESPACE || kubectl create namespace $PROD_NAMESPACE"
                    sh "kubectl apply -f helloworldapp.yaml -n $PROD_NAMESPACE"
                    sh "kubectl rollout status deployment/helloworldapp -n $PROD_NAMESPACE"
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

