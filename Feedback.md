### Feedback

*Please add below any feedback you want to send to the team*

## Configuration docker image movies-api:3

While debugging why the movies-api application throws errors. I found a log that caught my attention. The log was that the application could not find **movies-db.json** in the container. I tried mounting the directory to the image, but unfortunately the image did not scan subfolders.

Log: 
```
2024-00-00 00:00:00 api-1    | warn: Lodgify.Api.Challenge.Repository.MoviesRepository[0]
2024-00-0 00:00:00 api-1    |       File not found movies-db.json inside the container
```

Configuration in docker-compose:
```
    volumes:
      - ./app-container:/app/db
```