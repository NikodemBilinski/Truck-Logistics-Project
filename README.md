# Truck Logistics Project

# About

Full-stack logistics management system built with .NET. </br>
The system enables efficient management of Users, Trucks, Transport Jobs, Clients and Invoices through a desktop client communicating with a server.

# Requirements

<li style="margin:15px; font-size:16px">Visual Studio 2022</li>
<li style="margin:15px; font-size:16px">.NET SDK: .NET 8.0 or newer</li>
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


# Key Features

<li style="margin:15px; font-size:16px">Data Management on Users, Trucks, Jobs, Clients and Invoices.</li>
<li style="margin:15px; font-size:16px">User - Manage and edit personal data.</li>
<li style="margin:15px; font-size:16px">User - Applying for job if met all requirements of it.</li>
<li style="margin:15px; font-size:16px">Admin - Managing Users: Assigned Trucks, Assigned Jobs.</li>
<li style="margin:15px; font-size:16px">Admin - Managing Trucks: Assigned Users.</li>
<li style="margin:15px; font-size:16px">Admin - Managing Jobs: Assign User and Client.</li>
<li style="margin:15px; font-size:16px">Admin - Managing Clients.</li>
<li style="margin:15px; font-size:16px">Admin - Creating and Generating Invoices for a specific client and job, exporting them to a .PDF file.</li>
