# CarMarket

CarMarket is a full-stack car marketplace built with ASP.NET Core MVC. It provides one clear place where visitors can browse and search car advertisements, while authorized users can manage listings.

This project was created by **Haidar Alhaidari** as an individual full-stack student project.

## Project idea

Car advertisements are often spread across different platforms and may contain incomplete or unclear information. CarMarket solves this problem by presenting cars in a consistent format with important details such as brand, model, year, mileage, fuel type, price, description and images.

## Main features

- Browse all available cars
- View complete details for each car
- Search by brand or model
- Filter cars by fuel type
- Register, log in and log out
- Create, read, update and delete car advertisements (CRUD)
- Role-based authorization for protected functions
- Main image and additional car images
- Default image when a car has no image
- Responsive design for desktop, tablet and mobile

## User roles

### Visitor

- Browse car advertisements
- Search and filter cars
- View car details

### Registered user

- Log in securely
- Access functions allowed for the account
- Manage permitted advertisements

### Administrator

- Manage all car advertisements
- Access protected administration functions

## Technologies

- **ASP.NET Core MVC** – application structure and request handling
- **C#** – backend logic
- **Razor Views** – dynamic web pages
- **Entity Framework Core** – communication with the database
- **SQL Server** – permanent data storage
- **ASP.NET Core Identity** – registration, login, password hashing and roles
- **Bootstrap** – responsive user interface
- **HTML and CSS** – page structure and styling
- **Git and GitHub** – version control

## How the application works

CarMarket follows the MVC pattern:

- **Model** describes the application's data, such as a car and its properties.
- **View** displays the information to the user as a web page.
- **Controller** receives requests, works with the database and selects the correct view.

When a visitor opens the car list, the browser sends a request to the application. The controller uses Entity Framework Core and `ApplicationDbContext` to retrieve cars from SQL Server. The information is then sent to a Razor View and displayed as HTML.

## Database

The database stores car advertisements and Identity data.

Important car information includes:

- Brand
- Model
- Year
- Mileage
- Fuel type
- Price
- Description
- Main image
- Additional images

ASP.NET Core Identity uses separate tables for users, roles, login information and password hashes. Passwords are not stored as readable text.

## Project structure

```text
CarMarket/
├── Controllers/       Handles requests and application logic
├── Data/              Contains ApplicationDbContext
├── Models/            Contains data models such as Car
├── Views/             Contains Razor pages shown to users
│   ├── Cars/          Car list, details and CRUD pages
│   └── Shared/        Shared layout and partial views
├── wwwroot/           CSS, JavaScript and images
├── Areas/Identity/    Registration and login pages
├── Migrations/        Entity Framework Core database changes
├── appsettings.json   Application configuration
└── Program.cs         Services, middleware and application startup
```

## Installation

### Requirements

- Visual Studio 2022 or another compatible editor
- .NET SDK compatible with the project
- SQL Server or SQL Server LocalDB

### Run the project

1. Clone the repository:

   ```bash
   git clone https://github.com/HaidarAlhaidari/CarMarket.git
   ```

2. Open the project folder:

   ```bash
   cd CarMarket
   ```

3. Check the database connection string in `appsettings.json`.

4. Restore the required packages:

   ```bash
   dotnet restore
   ```

5. Apply the Entity Framework Core migrations:

   ```bash
   dotnet ef database update
   ```

6. Start the application:

   ```bash
   dotnet run
   ```

7. Open the local address shown in the terminal.

## Security

- Authentication checks who the user is.
- Authorization checks what the user is allowed to do.
- Identity hashes passwords before saving them.
- Protected operations are checked on the server.
- Administrator passwords and other secrets should never be committed to GitHub.

For development secrets, use .NET User Secrets or environment variables instead of writing passwords directly in the source code or `appsettings.json`.

## Future improvements

- Upload image files directly through the application
- Store multiple images in a separate `CarImage` table
- Add favorites
- Improve messaging between buyers and sellers
- Add automated tests
- Improve accessibility and validation
- Deploy the application online

## What I learned

During this project, I learned how the central parts of an ASP.NET Core MVC application work together. I also improved my understanding of CRUD, Entity Framework Core, SQL Server, Identity, role-based authorization, Razor Views, responsive design and step-by-step debugging.

## Author

**Haidar Alhaidari**  
Full-stack developer  
Email: alhaidari1050@gmail.com  
GitHub: [HaidarAlhaidari](https://github.com/HaidarAlhaidari)

## License

This project was created for educational purposes.
