# WertyMusic
Your personal and free music downloader that runs on your computer.

### How to run WertyMusic on Linux:
1) Install Docker

Check if Docker is already installed:
```
docker --version
```
If Docker is not installed, follow the official installation page:
https://docs.docker.com/engine/install/ubuntu/ or try
```
sudo apt-get update
sudo apt-get install docker-compose
```

2) Clone and run the application
```
git clone https://github.com/DanisimoDanik228/WertyMusic.git
cd WertyMusic
git checkout dev
sudo docker compose -f compose-view.yaml up --build
```

3) Verify Docker containers are running
```
docker ps -a
```
You should see output similar to this:
```
CONTAINER ID   IMAGE                               COMMAND                  CREATED         STATUS         PORTS                                                             NAMES
ab09ad5b077c   werty2648/wertymusic:1.0.0         "dotnet View.dll"        4 seconds ago   Up 2 seconds   8081/tcp, 0.0.0.0:5000->8080/tcp, [::]:5000->8080/tcp             werty-music
1a7f9185d12f   selenium/standalone-chrome:latest   "/opt/bin/entry_poin…"   5 seconds ago   Up 3 seconds   5900/tcp, 0.0.0.0:4444->4444/tcp, [::]:4444->4444/tcp, 9000/tcp   wertymusic-selenium-1
99021bed2458   postgres:16                         "docker-entrypoint.s…"   5 seconds ago   Up 3 seconds   0.0.0.0:5432->5432/tcp, [::]:5432->5432/tcp                       db-werty-music
```

4) Access WertyMusic

Open your browser and navigate to:
```
http://localhost:5000
```
