FROM node:13-slim

WORKDIR /config

COPY config.json ./

RUN npm install --global markdownlint-cli

ENTRYPOINT ["markdownlint", "**/*.md", "--ignore", "readme.md", "--config", "/config/config.json"]