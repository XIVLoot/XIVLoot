docker build -f FFXIV-RaidLootAPI/Dockerfile -t ffxiv-raidlootapi .
docker tag ffxiv-raidlootapi rickchinois/ffxivraidlootapi
docker push rickchinois/ffxivraidlootapi