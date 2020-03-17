FROM node:13-slim

WORKDIR /lint

RUN npm install --global markdownlint-cli

COPY config.json /config.json

ENTRYPOINT ["dir"]