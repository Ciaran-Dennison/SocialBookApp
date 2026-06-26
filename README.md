## Project Structure
SocialBookAppDomain          - Domain models and enums

SocialBookAppApplication     - Interfaces and service contracts

SocialBookAppInfrastructure  - EF Core DbContext and service implementations

SocialBookAppAPI             - ASP.NET Core Web API

SocialBookAppCLI             - Console application

SocialBookAppVue             - Vue.js frontend

## Setup

### Database

Update the connection string in `SocialBookAppCLI/appsettings.json` and `SocialBookAppAPI/appsettings.json` to point to your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SocialBookApp;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Then open the Package Manager Console in Visual Studio and run:
Update-Database -Project SocialBookAppInfrastructure -StartupProject SocialBookAppCLI

### Running the API

Set `SocialBookAppAPI` as the startup project and press F5.

The API will start on `http://localhost:5121`. If it starts on a different port update `baseURL` in `SocialBookAppVue/src/services/api.ts`.

### Running the Frontend

Open a terminal in the `SocialBookAppVue` folder and run:
npm install

npm run dev

The frontend will be available at `http://localhost:5173`.

## Notes

- The '#' character in folder paths breaks Vite, so ensure the solution is not place in a folder path containing this character (e.g. `C:\Git\C#\` won't work, rename to CSharp)
- Use Visual Studio for editing/viewing the backend, Visual Studio Code for editing/viewing the vue application/frontend
- CORS is configured to allow requests from `http://localhost:5173`
- If your API runs on a different port update `baseURL` in `SocialBookAppVue/src/services/api.ts`
