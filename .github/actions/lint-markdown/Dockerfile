FROM node:13

COPY config.json .

RUN npm install --global markdownlint-cli

ENTRYPOINT [ "markdownlint", "**/*.md", "--config config.json"]