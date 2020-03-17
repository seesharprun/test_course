FROM node:13-slim

COPY config.json ./

RUN npm install --global markdownlint-cli

ENTRYPOINT ["markdownlint", "**/*.md", "--config", "./config.json", "--ignore", "readme.md"]