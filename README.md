# Capital Guardian

Added sA sample application where [Create React App](https://create-react-app.dev/) and [Fable](https://fable.io/) come together.

Source code for https://blog.nojaf.com/2019/12/10/using-create-react-app-with-fable/


## Build

Your need .NET Core 3.0, latest Node.JS and Yarn.

> dotnet tool restore

> dotnet fake run build.fsx


## Develop

> dotnet fake run build.fsx -t Watch

## Format

> dotnet fake run build.fsx -t Format