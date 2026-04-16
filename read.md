# 🏨 Hotel Booking System (MVP)

A full-stack **Hotel Booking Web Application** built using **.NET Web API, Angular, and MySQL**.
This project enables users to browse hotels, view rooms, check facilities, and book rooms.


### 🔹 Backend

* ASP.NET Core Web API
* Entity Framework Core (Database First)
* JWT Authentication

### 🔹 Frontend

* Angular
* HttpClient for API integration
* Basic HTML/CSS

### 🔹 Database

* MySQL
* InnoDB Storage Engine
* Normalized relational schema

---

## 🎯 Features (MVP)

* Browse hotels and rooms
* View room facilities (AC, WiFi, etc.)
* Search and filter rooms
* User authentication (Login/Register)
* Book rooms with date selection
* Availability update after booking
* Secure REST APIs

---

## 🗄️ Database Design

### Tables Used:

* Users
* Hotels
* Rooms
* Facilities
* RoomFacilities (Many-to-Many)
* Bookings

### Key Concepts:

* Foreign Keys for relationships
* Many-to-Many mapping (Rooms ↔ Facilities)
* InnoDB for transactions and data integrity

---

2. Configure connection string in `appsettings.json`

3. Run database scaffolding


Scaffold-DbContext "<connection_string>" Pomelo.EntityFrameworkCore.MySql -OutputDir Models




5. test using swagger.

```
https://localhost:5001/swagger
```

---

## 🔌 API Endpoints (Sample)

### Hotels

* GET /api/hotels
* GET /api/hotels/{id}

### Rooms

* GET /api/rooms/{hotelId}
* GET /api/rooms/search

### Bookings

* POST /api/bookings
* GET /api/bookings/user/{id}

### Auth

* POST /api/auth/login
* POST /api/auth/register


## 📂 Frontend Structure

```
src/app/
 ├── components/
 │     ├── hotels
 │     ├── rooms
 │     ├── booking
 │     ├── login
 │
 ├── services/
 │     ├── hotel.service.ts
 │     ├── room.service.ts
 │     ├── booking.service.ts
 │     ├── auth.service.ts
```

---

## 🔄 Application Flow

```
User → Browse Hotels → View Rooms → Select Dates → Book Room → Confirmation


## Security

* JWT-based authentication
* Protected APIs
* Basic validation

---

## 🐳 Future Enhancements

* Payment Integration
* Email Confirmation
* Booking History
* Promotions & Discounts





---
