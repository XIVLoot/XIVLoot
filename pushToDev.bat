docker build -f FFXIV-RaidLootAPI/Dockerfile -t ffxiv-raidlootapi .
docker tag ffxiv-raidlootapi rickchinois/ffxivraidlootapi:dev
docker push rickchinois/ffxivraidlootapi:dev