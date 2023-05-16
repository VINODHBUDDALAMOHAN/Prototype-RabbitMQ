Setting up docker RabbitMQ docker on local machine
1. run docker command
docker run -d --name acq-rabbit -p 5672:5672 -p 15672:15672  -e RABBITMQ_MNESIA_BASE=/var/lib/rabbitmq/mnesia rabbitmq:3-management --restart always

2.dotnet run ConfigureRabbitMQ.

3. Build RabbitMQ image with MQTT plugin
docker build -t rabbitmq-mqtt-image:latest .

4. Run with RabbitMQ plugin
docker run -d --name rabbitmq-mqtt -p 5672:5672 -p 15672:15672 -p 1883:1883  rabbitmq-mqtt-image