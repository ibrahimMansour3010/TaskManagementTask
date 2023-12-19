# TaskManagmentTask For Interview Job

## Description

This project allows you to run and create/seeds required data, apply authorized based on permission and roles, and apply DDD pernciples.
this project has been build using .NET core 7
It includes two controllers:
1. **AccountController**
   - Registeration and login APIs
   - Three users with different roles:
      - SuperAdmin: All permissions for the TaskController
        - Login:
          - Email: superadmin@test.com
          - Password: P@SSW0RD
      - Admin: Permissions.Task.View for TaskController
        - Login:
          - Email: admin@test.com
          - Password: P@SSW0RD
      - Basic: (Additional role if applicable)

   - Authorization Examples:
      - `GetAll`: Authorized based on permission ("Permissions.Task.View") for SuperAdmin.
      - `GetAllPaged`: Authorized based on role (Admin) for admin@test.com.
      - All endpoints require authentication.

2. **TaskController**
   - CRUD operations on the Task model.
   - Based on Domain-Driven Design Architecture (DDD).

## Getting Started

To run the project and set up the required data, follow these steps:

1. Clone the repository:

   ```bash
   https://github.com/ibrahimMansour3010/TaskManagementTask.git
2. Set TaskManagement.WebAPI project as Startup Project and run it
