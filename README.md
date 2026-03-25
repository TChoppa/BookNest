# 📚 BookNest – Smart Library & Collaboration Platform  

> ⚡ A role-based web application enabling students to reserve books digitally, join academic clubs, and collaborate effectively  

![.NET](https://img.shields.io/badge/.NET-MVC-blue)
![Database](https://img.shields.io/badge/SQL%20Server-DB-red)
![Status](https://img.shields.io/badge/Status-Completed-success)

---

## 📌 Overview
**BookNest** is a web-based platform designed to modernize traditional library systems by enabling **digital book reservations**, **academic collaboration**, and **fair resource distribution**.

It creates a digital “nest” where students can:
- 📖 Reserve books online  
- 👥 Join academic clubs  
- 💬 Participate in discussions  

---

## 🎯 Problem Statement
In traditional library systems:

- ❌ Limited book access leads to unfair distribution  
- ❌ Manual reservation processes are slow  
- ❌ No collaboration platform for students  
- ❌ Lack of transparency in book allocation  

---

## 💡 Solution
BookNest solves these issues by providing:

- ✅ Digital book reservation system with Order ID  
- ✅ Role-based access (Admin, Librarian, User)  
- ✅ Academic clubs & discussion forums  
- ✅ Centralized book inventory management  
- ✅ Transparent and fair allocation system  

---

## 💼 Real-World Impact

This project was designed to solve real educational challenges:

- 📈 Improved access fairness for limited resources  
- ⏱️ Reduced manual workload for librarians  
- 🤝 Encouraged student collaboration  
- 🔄 Digitized traditional library workflows  

---

## ⚙️ Features

### 👤 User Features
- Search and explore books  
- Reserve books digitally  
- Join academic clubs  
- Participate in discussions  

### 📚 Librarian Features
- Manage book inventory (CRUD)  
- Verify reservations  
- Issue books using Order ID  

### 🛠️ Admin Features
- Manage users and roles  
- Control book categories  
- Monitor system activities  

---

## 🔐 Authentication & Authorization
- Secure login system  
- Role-Based Access Control (RBAC)  
- Dynamic UI rendering based on roles  

---

## 🧩 System Modules
- Authentication Module  
- Admin Module  
- Librarian Module  
- User Module  
- Club & Discussion Module  
- Reservation & Order Management Module  

---

## 🏗️ Architecture
- MVC (Model-View-Controller) Pattern  
- Separation of concerns  
- Scalable and maintainable design  
- Database-driven application  

---

## 🧠 Key Design Decisions
- Implemented **Order ID system** for tracking reservations  
- Used **role-based UI visibility** for better UX  
- Designed **club system** to enhance collaboration  
- Used **stored procedures** for efficient DB operations  

---

## 🧰 Tech Stack

- **Frontend:** HTML, CSS, Bootstrap, JavaScript, jQuery  
- **Backend:** ASP.NET MVC (.NET Framework)  
- **Database:** SQL Server  
- **Architecture:** MVC + RBAC  

---

## 📊 Workflow

1. User logs in  
2. Searches for books  
3. Reserves book → Order ID generated  
4. Librarian verifies request  
5. Book issued to user  

