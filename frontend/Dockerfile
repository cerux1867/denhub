# Build
FROM node:12-alpine as build
ARG DENHUB_API_URL
ENV DENHUB_API_URL=${DENHUB_API_URL}
RUN mkdir -p /app
WORKDIR /app
COPY package.json ./
RUN npm install
COPY . ./
RUN npm run build:prod

# Publish
FROM nginx:1.19.7-alpine
COPY --from=build /app/dist/denhub-web-app /usr/share/nginx/html