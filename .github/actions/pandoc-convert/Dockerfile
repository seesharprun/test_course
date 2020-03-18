FROM mcr.microsoft.com/dotnet/core/sdk:3.1

WORKDIR /app

RUN apt-get update

RUN apt-get install --assume-yes pandoc

COPY . ./

RUN dotnet publish --configuration Release --output /app/out

ENTRYPOINT [ "dotnet", "/app/out/app.dll", "--template", "/app/template.docx" ]