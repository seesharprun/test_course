FROM node:13

WORKDIR /lint

COPY config.json /config.json

RUN npm install --global markdownlint-cli

ENTRYPOINT ["dir"]