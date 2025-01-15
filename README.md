# Monster Trading Cards Game
This is my solution to the MTCG Semester Project game in C#. 
## Instructions to startup
1. Start docker container:
  - Open a cmd in the main implementation folder, where the docker-compose.yml file is
  - Type: `docker compose up -d` (this will start the docker container as a background process)
  - Type: `docker exec -it mtcg-postgres-1 psql -U admin -d mtcg_db` (this will enable you to use psql instructions on the database while it's running)
2. Start server:
  - Build project in an IDE
  - Run the application
