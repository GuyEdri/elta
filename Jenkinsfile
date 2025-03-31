pipeline {
    agent {
        kubernetes {
            yaml """
apiVersion: v1
kind: Pod
metadata:
  name: jenkins-agent
spec:
  containers:
  - name: jnlp
    image: guyedri/elta-agent
    volumeMounts:
    - mountPath: /home/jenkins
      name: workspace-volume
  volumes:
  - name: workspace-volume
    emptyDir: {}
  initContainers:
  - name: init-permissions
    image: busybox
    command:
    - sh
    - -c
    - "mkdir -p /home/jenkins && chmod -R 777 /home/jenkins"
    volumeMounts:
    - mountPath: /home/jenkins
      name: workspace-volume

            """
        }
    }

    environment {
        DOCKER_IMAGE = 'guyedri/helloworldapp:latest'
        BUILD_NAMESPACE = 'build'
        PROD_NAMESPACE = 'production'
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

