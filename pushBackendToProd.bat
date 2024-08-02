docker build -f FFXIV-RaidLootAPI/Dockerfile -t ffxiv-raidlootapi .
docker tag ffxiv-raidlootapi rickchinois/ffxivraidlootapi:latest
docker push rickchinois/ffxivraidlootapi:latest