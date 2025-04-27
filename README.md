# COMP3000-JimmyMacesanu

---

# 📊 Retail Staff and Sales Management System

This project is a **Retail Store Staff and Sales Management Web Application**, designed for modularity, scalability, and real-world usability. It was developed as part of a final-year Software Engineering project, with the aim of helping retail managers better organize rotas, staff, and sales — replacing traditional manual processes like Excel sheets.

---

## 🚀 Project Overview

The system consists of two main parts:

- **Backend API**: ASP.NET Core Web API (.NET 6)
- **Frontend**: Angular 16 (separated from backend for better scalability)

The backend handles:
- Staff management
- Store management
- Sales tracking
- Rota scheduling
- Secure authentication with role-based access control

The frontend provides:
- A clean, modular, and easy-to-navigate user interface
- Pages for managing stores, staff, rotas, and sales

---

## 🛠️ Technology Stack

| Layer        | Technology                        |
|--------------|-----------------------------------|
| Frontend     | Angular 17                        |
| Backend      | ASP.NET Core Web API (.NET 6)     |
| Database     | Microsoft SQL Server 2022 Express |
| Hosting      | Linux VPS (self-managed)          |
| Domain & SSL | Configured for secure access      |

---

## 📈 Features

- 🔑 User Authentication and Role Management
- 📅 Rota Scheduling and Management
- 🛍️ Sales Entry, Revenue Tracking Per Store
- 🧩 Modular Service Architecture
- 📂 Separation of Concerns (API / Frontend split)
- 🔒 Secure Hosting with SSL

---

## ⚙️ Setup Instructions

### Backend (ASP.NET Core API)

1. Clone the repository
2. Open the backend solution in Visual Studio
3. Update `appsettings.json` with your SQL Server connection string
4. Run migrations if necessary
5. Start the API (`dotnet run`)

### Frontend (Angular)

1. Navigate to the frontend folder
2. Run `npm install`
3. Update environment.ts to point to the correct API URL
4. Start the frontend server with `ng serve`

---

## 🧪 Testing

- ✅ **Unit Testing**: Backend services (e.g., StaffService, RotaService) using xUnit and Moq
- ✅ **Integration Testing**: API endpoints with EF Core In-Memory Database
- ✅ **Manual Testing**: Functional testing on frontend and backend endpoints

---

## 📅 Development Methodology

- Agile-based development with 2-week sprint cycles
- GitHub for version control
- Manual deployment and VPS hosting for simulating production environments

---

## 📜 License

This project is for educational purposes and personal portfolio demonstration.

---

## 📬 Contact

For any questions, feel free to open an issue or contact the project maintainer.

---
