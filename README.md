# About Pets
About Pets

# About Pets WebShop

About Pets WebShop is a web-based platform that facilitates buying products online. It provides a seamless user experience for users to list products, search for items, and users can send notification to Admin.

## Admin Features

In addition to the core functionalities, the platform includes several admin features to manage and enhance the user experience:

- **Add Products:** Admins can add new products to the webshop, including details such as product name, description, price, and images.
- **Modify Orders:** Admins have the ability to view and modify orders placed by users, including updating order statuses and handling customer inquiries.
- **Modify UserProfile:** Admins can modify user profiles.
- **Add Bonuses:** Admins can add bonuses or discounts to user accounts, allowing for promotional campaigns and rewards.

## Table of Contents

- [Overview](#overview)
- [Backend (ASP.NET)](#backend-aspnet)
- [Frontend (React.js)](#frontend-reactjs)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)

## Overview

The About Pets WebShop aims to create a user-friendly environment for both buyers and sellers. It streamlines the process of listing and finding products and order .

## Backend (ASP.NET)

The backend of the About Pets WebShop is built with ASP.NET. It provides essential functionalities for product management and user authentication.

### Features

- User authentication for secure login and registration.
- Endpoints for creating, editing, and deleting product listings.
- Authentication middleware to ensure certain actions are available only to logged-in users.

### Technologies Used

- ASP.NET Core
- Entity Framework Core
- Authentication using JWT tokens
- SQL Server for database storage

### Setup

1. Clone the repository.
2. Navigate to the `backend/WebShopAPI` directory.
3. Run `dotnet restore` to install dependencies.
4. Set up your SQL Server database and update the connection string in `appsettings.json`.
5. Run `dotnet ef database update` to apply database migrations.
6. Run `dotnet run` to start the backend server.

## Frontend (React.js)

The frontend of the About Pets WebShop is built with React.js. It offers an intuitive user interface for browsing products, searching, and ordering.

### Features

- Product listing with details, images, and prices.
- User-friendly navigation and category filtering.
- User Profile making.
- Order making.
- Bonus 

### Technologies Used

- React.js
- React Router for navigation
- Context API for state management
- Fetch for API communication
- Chat feature (upcoming): WebSocket or similar technology
- Image uploading (upcoming): Appropriate libraries

### Setup

1. Clone the repository.
2. Navigate to the `frontend` directory.
3. Run `dotnet restore` to install dependencies.
4. Update API base URL in the configuration.
5. Run `npm start` to start the frontend development server.

## Installation

To run the About Pets WebShop, follow these steps:

1. Clone this repository.
2. Set up and run the backend server (follow instructions in the [Backend section](#backend-aspnet)).
3. Set up and run the frontend (follow instructions in the [Frontend section](#frontend-reactjs)).
4. Access the application in your web browser.

## Usage

- Register or log in to your account.
- Browse products, search for items, and view product details.
- Create, edit, or delete product listings.
- Enjoy a seamless buying .
- If you want to try Admin selection => first you can be login (admin@admin.com, admin1234). 

## Contributing

We welcome contributions from the community. If you have suggestions or find issues, please create an issue or submit a pull request.
