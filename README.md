# parfumique
Parfumique was developed as a university project for the **Advanced Database Systems (Napredne baze podataka)** course, with a strong emphasis on utilizing Neo4j as the primary database. The project explores graph-based data modeling to efficiently store and query fragrance relationships, leveraging Cypher to implement complex queries for personalized fragrance recommendations.

## Features
- **User Registration & Authentication**: Secure login and registration using JWT.
- **Fragrance Collection Management**: Users can easily add or delete fragrances from their personal collection.
- **Personalized Recommendations**: Get fragrance suggestions based on your preferences and collection.
- **Admin Dashboard**: Admins can manage users and access all features in the system.

## User Roles & Features
### Users
- Register and log in.
- Add and remove fragrances from their personal collection.
- Receive personalized fragrance recommendations based on their collection.
### Admins
- Pre-assigned administrators have full access to all endpoints and functionalities within the application.

## Tech Stack
🚀 **Frontend:** React with TypeScript

⚙️ **Backend:** .NET 8 (C#)

🛢 **Database:** Neo4j (GDMS)

🔐 **Authorization:** JWT (JSON Web Tokens)

## Installation
### Prerequisites:
- **.NET**: [Download .NET](https://dotnet.microsoft.com/en-us/download)
- **Node.js**: [Download Node.js](https://nodejs.org/en/download)
### Steps to run the project:
1. **Clone the repository:** 
```git clone https://github.com/aleksa1205/parfumique``` 
2. **Run the backend:**
```bash
cd Backend
dotnet restore
dotnet run
```
3. **Run the frontend:**
```bash
cd Frontend
npm install
npm start
```
The application should now be running successfully.

## Authors
- [Aleksa Perić](https://github.com/aleksa1205)
- [Jovan Cvetković](https://github.com/CJovan02)

