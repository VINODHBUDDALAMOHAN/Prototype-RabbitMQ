Setting up docker RabbitMQ docker on local machine
1. run docker command
docker run -d --name acq-rabbit -p 5672:5672 -p 15672:15672  -e RABBITMQ_MNESIA_BASE=/var/lib/rabbitmq/mnesia rabbitmq:3-management --restart always

2.dotnet run ConfigureRabbitMQ.

