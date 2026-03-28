# Item Management System 

A full-stack ASP.NET MVC application for managing hierarchical item structures with weight validation and user authentication.

##  Features

- **Hierarchical Items**: Create parent-child relationships between items.
- **Recursive Processing**: Traverse and process entire item trees.
- **Weight Validation**: Ensure child weights do not exceed parent weight limits.
- **Search & Filter**: Quickly find items by name.
- **User Authentication**: Secure login system with session management.
- **Native ADO.NET**: Efficient database operations using stored procedures.

##  Tech Stack

- **Frontend**: HTML5, CSS3, JavaScript, Razor Views
- **Backend**: ASP.NET MVC (.NET Framework 4.7.2), C#
- **Database**: SQL Server (ADO.NET, Stored Procedures)

##  Project Structure

- `Controllers/`: Handles application logic and routing.
- `Models/`: Data structures representing Items and Users.
- `Views/`: User interface components.
- `Helpers/`: Core logic for database interactions (`DbHelper.cs`).
- `SQL/`: Database schema and stored procedure scripts.

##  Setup Instructions

### 1. Database Configuration
1. Open SQL Server Management Studio (SSMS).
2. Execute the script found in [SQL/DatabaseScript.sql](SQL/DatabaseScript.sql) to create the database, tables, and stored procedures.

### 2. Application Configuration
1. Open the project in Visual Studio.
2. Navigate to `Web.config`.
3. Update the `ItemHierarchyDB` connection string if your SQL Server instance is not `localhost` or uses specific credentials.
   ```xml
   <add name="ItemHierarchyDB" 
        connectionString="Data Source=YOUR_SERVER;Initial Catalog=ItemHierarchyDB;Integrated Security=True" 
        providerName="System.Data.SqlClient" />
   ```

### 3. Running the App
1. Build the solution in Visual Studio.
2. Press `F5` to run using IIS Express.

##  Default Credentials

- **Email**: `admin@example.com`
- **Password**: `admin123`

