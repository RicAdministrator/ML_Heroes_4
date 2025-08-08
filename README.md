## Project: ML Heroes
Description:\
Simple project that demonstrates CRUD operations for Mobile Legends heroes.

Tech Stack:
1. Api - .Net9, WebAPI, Entity Framework, SQL, Scalar
2. FrontEnd - Angular, Node.js, Javascript, W3.css, CSS

## Database
How to Create a Single Migration File and Check DB:
1. Package Manager Console > Run "cls"
2. In windows file explorer, delete all files in Migrations folder
3. Package Manager Console > Run "Add-Migration Initial"
4. Check Migrations folder
5. Package Manager Console > Run "Drop-Database"
6. A confirmation will appear, input "y" then press enter
7. In SSMS, check if db was deleted
8. Package Manager Console > Run "Update-Database". This will create the db.
9. Check tables

How to Drop Create a DB:
1. Package Manager Console > Run "cls"
2. Package Manager Console > Run "Drop-Database"
3. A confirmation will appear, input "y" then press enter
4. In SSMS, check if db was deleted
5. Package Manager Console > Run "Update-Database". This will create the db.
6. Check tables

## API
How to Run Scalar to Test Endpoints:
1. Open WebApi project in Visual Studio
2. Click play button (https)
3. Go to https://localhost:7179/scalar/v1

How to Create a New Controller:
1. Controllers > Add > Controller > Installed > Common > API > API Controller - Empty

## Front End
How to Run the Angular Web App
1. Open ML_Heroes_4 folder in VS Code
2. E:\Git\ML_Heroes_4\FrontEndAngular> python -m http.server
3. Open http://localhost:8000/MobileLegends.htm in your browser

How to Auto Indent Code:
1. Shift+Alt+F