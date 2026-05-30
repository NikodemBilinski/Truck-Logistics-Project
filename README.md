# Truck Logistics Project

# About

Full-stack logistics management system built with .NET. </br>
The system enables efficient management of Users, Trucks, Transport Jobs, Clients and Invoices through a desktop client communicating with a server.

# Requirements

<li style="margin:15px; font-size:16px">Visual Studio 2022</li>
<li style="margin:15px; font-size:16px">.NET SDK: .NET 10 or newer</li>
<li style="margin:15px; font-size:16px">SQL Server Express 22</li>
<li style="margin:15px; font-size:16px">SQL Server Management Studio 22 for database GUI (optional)</li>

# Architecture

The Project follows a Client-Server model:

<li style="margin:15px; font-size:16px">Server: ASP.NET Core Web Api (Rest API).</li>
<li style="margin:15px; font-size:16px">Client: .NET MAUI (Multi-platform App UI). </li>
<li style="margin:15px; font-size:16px">Database: SQL Server / Entity Framework Core.</li>
<li style="margin:15px; font-size:16px">Database GUI: SQL Server Management Studio 22.</li>

# Technologies

<li style="margin:15px; font-size:16px">Backend: ASP.NET Core Web Api | Entity Framework Core.</li>
<li style="margin:15px; font-size:16px">Frontend: .NET MAUI | XAML | Data Binding.</li>
<li style="margin:15px; font-size:16px">PDF Generating Engine: QuestPDF.</li>

# Setup

<li style="margin:15px; font-size:16px">Clone the repository</li>
<li style="margin:15px; font-size:16px">Database Configuration</li>

Install SQL Server Express and update the connection string in `appsettings.json` :<br>
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=TruckDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

<li style="margin:15px; font-size:16px">Network Configuration</li>

By default the server listens on `0.0.0.0:5160` which means it is accessible in from any device in your local network

If you wish to change it to your local machine only, change `appliactionUrl` in `launchSettings.json`

```json
// accessible from local network
"applicationUrl": "http://0.0.0.0:5160"
```

```json
// accessible only on local machine
"applicationUrl": "http://localhost:5160"
```

<li style="margin:15px; font-size:16px">JWT Configuration</li>

Change the default JWT key in `appsettings.json` to your own random string (min 32 characters):
```json
"Jwt": {
    "Key": "CHANGE_THIS_TO_YOUR_OWN_RANDOM_SECRET_KEY"
}
```
<li style="margin:15px; font-size:16px">Run database migrations</li>

Open Terminal in the server project and run: 
```bash
dotnet ef database update
```
<li style="margin:15px; font-size:16px">Run the server and configure apiurl</li>

Open the solution in Visual Studio 2022 and run the server project in http,<br>
Copy the listening link and paste it in `set http link for api` section in LogInPage.xaml.cs<br>
```csharp
Preferences.Set("api_url", "YOUR_HTTP");
```
<li style="margin:15px; font-size:16px">First User</li>

After running migrations and configuring, add your first admin user via Swagger UI at:
`http://Your_Link:Your_Port/Swagger`
Use `POST /api/Users/Add_User` with role set to `"admin"`


# Key Features

<li style="margin:15px; font-size:16px">Data Management on Users, Trucks, Jobs, Clients and Invoices.</li>
<li style="margin:15px; font-size:16px">User - Manage and edit personal data.</li>
<li style="margin:15px; font-size:16px">User - Applying for job if met all requirements of it.</li>
<li style="margin:15px; font-size:16px">Admin - Managing Users: Assigned Trucks, Assigned Jobs.</li>
<li style="margin:15px; font-size:16px">Admin - Managing Trucks: Assigned Users.</li>
<li style="margin:15px; font-size:16px">Admin - Managing Jobs: Assign User and Client.</li>
<li style="margin:15px; font-size:16px">Admin - Managing Clients.</li>
<li style="margin:15px; font-size:16px">Admin - Creating and Generating Invoices for a specific client and job, exporting them to a .PDF file.</li>
<li style="margin:15px; font-size:16px">Pagination for Users, Trucks, Jobs, Clients and Invoices</li>
<li style="margin:15px; font-size:16px">Soon adding filtering too</li>