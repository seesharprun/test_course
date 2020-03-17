FROM node:13-slim

WORKDIR /lint

COPY ./config.json ./

RUN npm install --global markdownlint-cli

ENTRYPOINT ["dir"]