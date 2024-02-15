Write-Output "Fixing issue with movies-db.json"
docker container exec -it moviesapichallenge-api-1 mv amovies-db.json movies-db.json