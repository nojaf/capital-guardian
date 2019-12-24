# Capital Guardian

[![Greenkeeper badge](https://badges.greenkeeper.io/nojaf/capital-guardian.svg)](https://greenkeeper.io/)

Added sA sample application where [Create React App](https://create-react-app.dev/) and [Fable](https://fable.io/) come together.

Source code for https://blog.nojaf.com/2019/12/10/using-create-react-app-with-fable/


## Prerequisites

Your need .NET Core 3.0, latest Node.JS and Yarn.
For deployment the Azure cli (az) and Azure Function Core tools (func) should be installed.

## Build

> dotnet tool restore

> dotnet fake run build.fsx

## Develop

The following environment variables should be set before running the `Watch` target:

- StorageAccountKey
- StorageAccountName
- REACT_APP_BACKEND

> dotnet fake run build.fsx -t Watch

## Deployment

A parameters file should be created inside the `infrastructure` folder containing all the necessary parameters listed in `azuredeploy.json`.
The name of the parameter file should reflect the stage, f.ex `dev.json`.

### Login to Azure

> dotnet fake run build.fsx -t AzureLogin

### Resources

> dotnet fake run build.fsx -t AzureResources -- env=dev

### Client

> dotnet fake run build.fsx -t DeployClient -- env=dev

### Server

> dotnet fake run build.fsx -t DeployServer -- env=dev

## Format

> dotnet fake run build.fsx -t Format