# Pushing the changes to production
Open the terminal in the ffxiv-raid-loot-frontend folder and execute the following scripts below.
Make sure you are logged into your docker hub account to be able to push to the docker hub repo or you will have permission errors.

## Build the front-end

``` shell
docker build -t xivloot-app .
```

## Tag the image

``` shell
docker tag xivloot-app rickchinois/xivloot-app
```

## Push the image

``` shell
docker push rickchinois/xivloot-app
```
