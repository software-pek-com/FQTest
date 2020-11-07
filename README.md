# FQTest
An interview test project.

# Prerequisites
-	The database must be MongoDB or elastic search.
-	Event bus can be what you prefer while it is deployable into docker.

# Definition

## User entity
-	Id (Unique, generated by the server)
-	First name (Mandatory)
-	Last name (Mandatory)
-	Username (Mandatory, must be unique in the database)
-	Birthdate
-	Password 
-	Audit
    * Creation date (Mandatory)
    * Creation user (Mandatory)
    * Last update date
    * Last update user
- Last connection date

# ToDo

## Create a WebAPI for handling users

1. Create a new user. Save new user to the database, and send message to the event bus for other services.

1. Update existing user. Save modifications to the database and send message to the event bus for future needs. 

1. List all users.

1. Retrieve a list of users composed by
    - Id (Unique, generated by the server)
    - First name (Mandatory)
    - Last name (Mandatory)
    - Has already connected (Indicate if user have made at least 1 connection)
    - Display name (“First name Last name”)

1. Request can be filtered by 
    - First name
    - Last name
    - User already connected / never connected

1. Request can ask result be ordered by
    -	Last connection date
    - Creation date 
    - Last name (Default order)

1. Get user detail by id. Retrieve sufficient data for editing user in profile page.

1. Delete existing user from database.

## Create service 

When it takes an event from the bus for user creation, service writes to file “Welcome USERNAME.txt” with content as below. WEBSITEURL can be changed depending on the deployment environment.

```
Hello FIRSTNAME LASTNAME, 
Welcome to Finquest candidate test platform, your registering have been approved, and now you can connect to WEBSITEURL/login to use the platform.

FinQuest Team
```

## Nice to have
1. Solution/project must be runnable using a simple docker-compose command with the full environment (database, event broker, dotnet application).
1. Implement security and performance improvements.
