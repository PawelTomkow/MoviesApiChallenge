Write-Output "Fixing issue with movies-db.json"
docker container exec -it movies-api mv amovies-db.json movies-db.json