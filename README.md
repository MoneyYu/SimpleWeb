# SimpleWeb

[![Build Status](https://github.com/MoneyYu/SimpleWeb/actions/workflows/01.build.yml/badge.svg)](https://github.com/MoneyYu/SimpleWeb/actions/workflows/01.build.yml)

A demo ASP.NET Core 10.0 web application showcasing modern DevOps practices, cloud deployment strategies, and Infrastructure as Code (IaC) patterns.

> 📖 [繁體中文版本 (Traditional Chinese Version)](README_zh-TW.md)

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Running Tests](#running-tests)
- [Docker Support](#docker-support)
- [CI/CD Pipelines](#cicd-pipelines)
- [Infrastructure as Code](#infrastructure-as-code)
- [Kubernetes Deployment](#kubernetes-deployment)
- [Contributing](#contributing)
- [License](#license)

## Overview

SimpleWeb is a demonstration project designed to showcase best practices for:

- Building ASP.NET Core web applications
- Implementing CI/CD pipelines with Azure DevOps and GitHub Actions
- Container deployment using Docker and Kubernetes (AKS)
- Infrastructure provisioning using Terraform and Bicep
- Security scanning and code quality analysis

## Features

- **ASP.NET Core 10.0 MVC** - Modern web framework with MVC architecture
- **Health Check Endpoint** - Built-in health monitoring at `/health`
- **File Upload** - Support for local and Azure Blob Storage
- **Application Insights** - Telemetry and monitoring integration
- **User Authentication** - Azure AD authentication header support
- **Flexible Storage** - Configurable storage providers (Local/Azure)

## Project Structure

```
SimpleWeb/
├── src/
│   ├── SimpleWeb/                    # Main web application
│   │   ├── Controllers/              # MVC controllers
│   │   ├── Models/                   # Data models
│   │   ├── Views/                    # Razor views
│   │   ├── wwwroot/                  # Static files
│   │   ├── Dockerfile                # Container definition
│   │   └── appsettings.json          # Application configuration
│   ├── SimpleWeb.UnitTest/           # Unit tests
│   ├── SimpleWeb.IntegrationTest/    # Integration tests
│   └── SimpleWeb.UITest/             # UI automation tests
├── ci/                               # Azure DevOps pipeline definitions
│   ├── 01.build.yml                  # Basic build pipeline
│   ├── 01.prwithlimitbranch.yml      # PR validation with branch restriction
│   ├── 02.packagescan.yml            # Security scanning (Snyk)
│   ├── 03.sonarcloud.yml             # Code quality analysis
│   ├── 04.publish.artifacts.yml      # Artifact publishing
│   ├── 05.multistagerelease.yml      # Multi-stage release
│   ├── 06.dockerseperate.yml         # Docker separate build
│   ├── 07.dockerbuildandpush.yml     # Docker build and push
│   ├── 08.aks.yml                    # AKS deployment
│   ├── 09.terraform.build.yml        # Terraform build
│   ├── 09.terraform.release.yml      # Terraform release
│   └── 10.bicep.yml                  # Bicep deployment
├── bicep/                            # Azure Bicep templates
│   ├── main.bicep                    # Main infrastructure template
│   └── parameters.json               # Parameters file
├── tf/                               # Terraform configurations
│   └── infra.tf                      # Azure infrastructure
├── manifests/                        # Kubernetes manifests
│   ├── deployment.yml                # K8s deployment
│   └── service.yml                   # K8s service (LoadBalancer)
├── scripts/                          # Utility scripts
│   └── TestifyZeroDowntime.ps1       # Zero-downtime testing
└── .github/workflows/                # GitHub Actions
    ├── 01.build.yml                  # Build and test workflow
    ├── 01.prwithlimitbranch.yml      # PR validation with branch restriction
    ├── 02.packagescan.yml            # Security scanning (Snyk)
    ├── 03.sonarcloud.yml             # Code quality analysis (SonarCloud)
    ├── 04.publish.artifacts.yml      # Artifact publishing
    ├── 05.multistagerelease.yml      # Multi-stage release to Azure Web App
    ├── 06.dockerseperate.yml         # Docker build and push to GHCR
    ├── 07.dockerbuildandpush.yml     # Docker build and push to ACR
    ├── 08.aks.yml                    # Deploy to Azure Kubernetes Service
    ├── 09.terraform.build.yml        # Terraform build workflow
    ├── 09.terraform.release.yml      # Terraform release workflow
    └── 10.bicep.yml                  # Bicep deployment workflow
```

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [Docker](https://www.docker.com/get-started) (for containerization)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli) (for Azure deployments)
- [Terraform](https://www.terraform.io/downloads) (for IaC with Terraform)
- [kubectl](https://kubernetes.io/docs/tasks/tools/) (for Kubernetes deployments)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/MoneyYu/SimpleWeb.git
cd SimpleWeb
```

### Restore Dependencies

```bash
dotnet restore src/SimpleWeb.sln
```

### Build the Application

```bash
dotnet build src/SimpleWeb.sln --configuration Release
```

### Run the Application

```bash
cd src/SimpleWeb
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`.

## Configuration

### Application Settings

The application supports the following configuration options in `appsettings.json`:

```json
{
  "Storage": {
    "Type": "Local",           // Options: "Local" or "Azure"
    "FileName": "sample.jpg",
    "Azure": {
      "ConnectionString": ""   // Azure Storage connection string
    }
  },
  "APPINSIGHTS_CONNECTIONSTRING": ""  // Application Insights connection
}
```

### User Secrets (Development)

For secure storage of sensitive configuration in development:

```bash
# Initialize user secrets
dotnet user-secrets init

# Set Azure Storage connection string
dotnet user-secrets set "Storage:Azure:ConnectionString" "your-connection-string"

# Set Application Insights connection
dotnet user-secrets set "APPINSIGHTS_CONNECTIONSTRING" "your-connection-string"
```

For more information, see [Safe storage of app secrets in development in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets).

### Environment Variables

| Variable | Description |
|----------|-------------|
| `Storage__Type` | Storage provider type (`Local` or `Azure`) |
| `Storage__Azure__ConnectionString` | Azure Blob Storage connection string |
| `APPINSIGHTS_CONNECTIONSTRING` | Application Insights connection string |

## Running Tests

### Unit Tests

```bash
dotnet test src/SimpleWeb.UnitTest/SimpleWeb.UnitTest.csproj --verbosity normal
```

### Integration Tests

```bash
dotnet test src/SimpleWeb.IntegrationTest/SimpleWeb.IntegrationTest.csproj --verbosity normal
```

### All Tests

```bash
dotnet test src/SimpleWeb.sln --verbosity normal
```

## Docker Support

### Build Docker Image

```bash
cd src/SimpleWeb
docker build -t simpleweb:latest .
```

### Run Container

```bash
docker run -d -p 8080:80 --name simpleweb simpleweb:latest
```

Access the application at `http://localhost:8080`.

### Docker Compose (Optional)

```bash
# Build and run
docker-compose up -d

# Stop and remove
docker-compose down
```

## CI/CD Pipelines

This project includes multiple CI/CD pipeline definitions for Azure DevOps:

| Pipeline | Description |
|----------|-------------|
| `01.build.yml` | Basic build and test pipeline |
| `01.prwithlimitbranch.yml` | PR validation requiring dev branch |
| `02.packagescan.yml` | Security scanning with Snyk |
| `03.sonarcloud.yml` | Code quality analysis with SonarCloud |
| `04.publish.artifacts.yml` | Build artifacts publishing |
| `05.multistagerelease.yml` | Multi-stage deployment to Azure Web App |
| `06.dockerseperate.yml` | Separate Docker build stages |
| `07.dockerbuildandpush.yml` | Docker image build and push to ACR |
| `08.aks.yml` | Deploy to Azure Kubernetes Service |
| `09.terraform.*.yml` | Infrastructure deployment with Terraform |
| `10.bicep.yml` | Infrastructure deployment with Bicep |

### GitHub Actions

The project includes comprehensive GitHub Actions workflows (`.github/workflows/`) that mirror the Azure DevOps pipelines:

| Workflow | Description |
|----------|-------------|
| `01.build.yml` | Build and test on push to any branch |
| `01.prwithlimitbranch.yml` | PR validation requiring PRs to master from dev branch |
| `02.packagescan.yml` | Security scanning with Snyk |
| `03.sonarcloud.yml` | Code quality analysis with SonarCloud |
| `04.publish.artifacts.yml` | Build artifacts publishing |
| `05.multistagerelease.yml` | Multi-stage deployment to Azure Web App |
| `06.dockerseperate.yml` | Docker build and push to GitHub Container Registry |
| `07.dockerbuildandpush.yml` | Docker image build and push to Azure Container Registry |
| `08.aks.yml` | Build, push to ACR, and deploy to Azure Kubernetes Service |
| `09.terraform.build.yml` | Build and prepare Terraform artifacts |
| `09.terraform.release.yml` | Deploy infrastructure with Terraform and deploy app |
| `10.bicep.yml` | Deploy Azure infrastructure with Bicep |

## Infrastructure as Code

### Terraform

Deploy Azure infrastructure using Terraform:

```bash
cd tf

# Initialize Terraform
terraform init

# Preview changes
terraform plan

# Apply changes
terraform apply
```

This creates:
- Azure Resource Group
- App Service Plan (Linux, Standard S1)
- Azure App Service (with .NET 10.0)

### Bicep

Deploy Azure infrastructure using Bicep:

```bash
# Using Azure CLI
az deployment group create \
  --resource-group <resource-group-name> \
  --template-file bicep/main.bicep \
  --parameters bicep/parameters.json

# Using PowerShell
New-AzResourceGroupDeployment \
  -ResourceGroupName <resource-group-name> \
  -TemplateFile bicep/main.bicep
```

This creates:
- Virtual Network with Subnet
- Storage Account
- Public IP Address
- Network Interface
- Windows Server Virtual Machine

## Kubernetes Deployment

Deploy to Kubernetes (AKS or any K8s cluster):

```bash
# Apply deployment
kubectl apply -f manifests/deployment.yml

# Apply service (LoadBalancer)
kubectl apply -f manifests/service.yml

# Check deployment status
kubectl get deployments
kubectl get pods
kubectl get services
```

### Kubernetes Resources

- **Deployment**: Creates pods running the SimpleWeb container
- **Service**: Exposes the application via LoadBalancer on port 80

## API Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/` | GET | Home page |
| `/Home/Privacy` | GET | Privacy policy page |
| `/Home/Upload` | GET | File upload page |
| `/Home/Upload` | POST | Upload file |
| `/health` | GET | Health check endpoint (JSON) |

## Health Check

The application exposes a health check endpoint at `/health` that returns a JSON response:

```json
{
  "status": "Healthy",
  "results": {}
}
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is open source and available under the [MIT License](LICENSE).