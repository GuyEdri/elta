pipeline {
    agent {
        kubernetes {
            label 'jenkins-agent'
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
                    checkout([
                        $class: 'GitSCM',
                        branches: [[name: '*/main']],
                        userRemoteConfigs: [[
                            url: 'https://github.com/GuyEdri/elta.git',
                            credentialsId: 'github'
                        ]]
                    ])
                }
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

