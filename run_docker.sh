#!/bin/sh

docker build . --tag tahakhan/ipaddressreporter
docker run -d -v $(pwd)/container:/app/ip -it tahakhan/ipaddressreporter