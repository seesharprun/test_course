FROM node:13

COPY config.json .

RUN npm install --global markdownlint-cli

ENTRYPOINT [ "markdownlint", "**/*.md", "--ignore", "readme.md", "--config", "config.json"]