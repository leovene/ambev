# ambev
Prerequisites
To run this project, ensure you have the following installed on your machine:

Docker: Install Docker from Docker's official website.
Node.js: Version 22.13.0 is required. You can download it from Node.js official website.
Getting Started
Follow these steps to set up and run the project:

1. Clone the Repository
Clone this repository to your local machine.

bash
Copy
Edit
git clone <repository-url>
cd <repository-folder>

2. Start Docker Containers
Navigate to the root directory of the project and run the following command to start the required services using Docker Compose:

bash
Copy
Edit
docker-compose up -d
Wait for the services to initialize completely.

3. Run the Application
Run the provided batch file to start the backend (API and Worker) and the frontend (Angular application).

bash
Copy
Edit
build-and-run.bat
4. Access the Application
Once the batch file completes its execution, you can access the application and related tools via the following URLs:

Frontend Application: http://localhost:4200/
Backend API (Swagger UI): https://localhost:7237/swagger/index.html
5. Monitoring Logs
To monitor application logs, you can access Seq, which is set up via Docker:

Seq (Log Viewer): http://localhost:5341
6. RabbitMQ Management
RabbitMQ is also available in the Docker setup for managing messages and events. You can access its management UI at:

RabbitMQ: http://localhost:15672
Use the default credentials:

Username: admin
Password: admin
