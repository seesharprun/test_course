FROM node:13

COPY config.json /config.json
COPY entrypoint.sh /entrypoint.sh

RUN npm install --global markdownlint-cli

ENTRYPOINT ["/entrypoint.sh"]