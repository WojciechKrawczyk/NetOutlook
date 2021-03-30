# NetOutlook
Web e-mail application.
Project was done as a task from "Tworzenie aplikacji webowych z wykorzystaniem .NET Framework" subject in 2020 winter term.
Contributors: Michał Machnio, Miłosz Mazur.

Businee requirements:
- HTML Interface
- Ability to send email to one or more users
- Ability to see inbox 
- Ability to menage users
- Ability to attach multiple file
- Create small desktop app to show notification
- Send perodical email to setup email about new messages

Roles:
User roles:
  - can send emial to person from global contact list
  - can view inbox
  - can open incoming email
  - can change emial as read/unread
  - can filter and search into inbox
Admin roles:
  - can see all users
  - can approve new users requests
  - can disable user
Group owner roles:
  - can create group
  - can manage users in group
 
Architecture:
- website is stored on Azure Web App
- list of email is loaded asynchronously via API using AJAX
- data are stored in SQL Database
- communication between Web App and SQL is done by Entity Framework
- files are stored in Azure Blob Storage
- user authentication is done with Azure AD B2C

Technology stack:
- SCRUM
- git, git-flow
- Azure DevOps
- Visual Studio
- App Service, Azure SQL DB, Azure AD B2C, SendGrid
- ASP.NET COre MVC, ASP.NET Core API, SQL, Entity Framework
- REST, OAuth 2.0
- PowerShell
- Unit Tests, Integration tests, UI tests, API tests
- CD/CI  
