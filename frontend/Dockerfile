# Build
FROM node:12-alpine as build
RUN mkdir -p /app
WORKDIR /app
COPY package.json /app
RUN npm install
COPY . /app
RUN npm run build --prod

# Publish
FROM nginx:1.19.7-alpine
COPY --from=build /app/dist/denhub-web-app /usr/share/nginx/html