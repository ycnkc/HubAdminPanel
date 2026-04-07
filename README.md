# Hub Admin Panel

Hub Admin Panel is a comprehensive management system developed for modern web applications, offering **Dynamic Role-Based Access Control (RBAC)** and **Automatic Endpoint Management**. This project aims to decouple security from code complexity, making it entirely manageable through the database and a user-friendly interface.

---

## Key Features

* **Automatic Endpoint Discovery:** Scans all Controllers and Actions in the project using the C# Reflection library. Every time the application runs, newly added endpoints are automatically saved to the database (SQL Server).
* **Dynamic Role-Based Access Control (RBAC):** Permissions are checked at runtime via a custom-developed **Middleware**, rather than being statically hardcoded (`[Authorize(Roles="...")]`).
* **Smart Refresh Token System:** Utilizing an Axios Interceptor architecture, when the Access Token expires, a new token is automatically fetched in the background without logging the user out, and the pending request is completed seamlessly.
* **Advanced Server-Side Pagination:** Handles thousands of endpoint or user records effortlessly using a `PagedResult` structure, ensuring data is presented in pages without performance degradation.
* **Modern UI/UX:** A fully responsive management interface powered by Bootstrap and Material Design Icons.

---

## Tech Stack

| Layer | Technology |
| :--- | :--- |
| **Backend** | .NET 8 Web API, Entity Framework Core |
| **Frontend** | HTML5, CSS3, JavaScript (ES6+), Axios |
| **Database** | Microsoft SQL Server |
| **Security** | JWT (JSON Web Token), Custom Authorization Middleware |
| **UI Components** | Bootstrap 5, SweetAlert2, MDI Icons |

---

## Security and Authorization Flow

The system filters every incoming request through the following steps:
1. **Authentication:** Does the request contain a valid JWT? (If expired, an automatic Refresh Token process is triggered).
2. **Discovery Check:** Is the requested route registered in the database? (If not, returns 403 Forbidden).
3. **Role Match:** Does at least one of the user's current roles exist in the list of allowed roles for that endpoint?

---

## Setup and Installation

### 1. Prerequisites
* .NET 8.0 SDK
* SQL Server (LocalDB or Express)
* Visual Studio 2022 / VS Code

### 2. Database Configuration
Update the Connection String field in the `appsettings.json` file according to your local server:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=HubAdminDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Running the Application
Apply the migrations via terminal and run the project:
```bash
dotnet ef database update
dotnet run
```
When the project runs for the first time, the `EndpointDiscoveryService` will execute and register all API routes into the database.

---

## User Guide

1. **Login:** Log in to the system using a user with "Admin" privileges.
2. **Endpoint Management:** Navigate to the "Endpoint Management" page from the menu. All active API endpoints in the system are listed here.
3. **Role Assignment:** Click the **"Manage Role"** button next to any endpoint.
4. **Access Permission:** Select the roles (e.g., Admin, Manager, Editor) that should have access to that operation (e.g., `/api/Users - GET`) from the modal that appears, and save it.
5. **Real-time Security:** Changes are instantly detected by the Middleware. Permissions are updated immediately without needing to restart the application or service.
