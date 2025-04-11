FROM node:alpine as build

WORKDIR /app

COPY package*.json ./

RUN npm install

RUN npm install -g @angular/cli

COPY . .

RUN node_modules/.bin/ng build --configuration=production

FROM nginx:latest

COPY ./nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/dist/ffxiv-raid-loot-web-app/browser /usr/share/nginx/html

EXPOSE 80
