docker build -f FFXIV-RaidLootAPI/Dockerfile -t ffxiv-raidlootapi .
cd .\FFXIV-RaidLootAPI\docker-db\
docker compose up -d
cd ..\..