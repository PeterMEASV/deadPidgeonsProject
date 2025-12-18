# Dead Pidgeons Project

## Technology Stack
### Backend (.NET 9.0)
- **Framework**: ASP.NET Core Web API
- **Language**: C#
- **Database**: PostgreSQL
- **Authentication**: JWT tokens with role-based authorization
- **Password Security**: Argon2id hashing

### Frontend 
- **Framework**: React 19.1.1
- **Language**: TypeScript 5.8.3
- **Routing**: React Router 7.8.1
- **State Management**: Jotai 2.13.1
- **Styling**: Tailwind CSS 3.4.18 with DaisyUI 5.1.26
- **HTTP Client**: Axios 1.11.0


### Created by Peteris Meiris, David Alfred Fyhn Jonas, Victor Soelberg Møller Jensen

3rd Semester Exam project at EASV

This project includes 2 independent applications, a server side application and a client application.
The project is based on a game played by a local sports club Jerne IF. The game involves players guessing 5-8 numbers and paying accordingly.
Once the payment and the player choices have been collected, the hosts randomly choose 3 winning numbers. If the player has guessed the correct numbers, they receive a portion of the winnings.
70% of the winnings are split between the winning players, and the rest is kept by Jerne IF.

This project is the digital equvivalent of the game, which is designed to be played alongside the physical version.

The application operates on a role-based authorization model, given to the 2 functional roles. (Player and Admin)
All admin-side http calls require the Admin authorization header in the message. This header is given upon login with the correct credentials.
This includes GET http requests such as viewing user information and transaction information.
This also includes POST, PUT and PATCH http requests that directly impact the status of the game in any way.

The user side requires a valid user session token to be accessed, otherwise the user will be sent back to the original Login screen.
The http requests are not protected via authorization, but require user information to access. Without this information, the individual methods will not produce results.

The session tokens assigned upon login are set to last for 10 minutes before expiring. 


Modern React 19 + TypeScript application using Vite, with automated CI/CD pipeline for quality assurance.
Linting options include:
- strict
- noUnusedLocals
- noUnusedParameters
- noFallthroughCasesInSwitch
- noUncheckedSideEffectImports
- only linting .tsx and .ts files
- react-hooks
- react-refresh
- no-misused-promises

CI workflow includes:
- Checkout code
- Setup Node.js
- Installing dependencies
- npm run build
- npm run lint

The applications are deployed via fly.io to the sites:
- **Client:** http://mindst-2-commits-client.fly.dev/
- **Server:** http://mindst-2-commits-server.fly.dev/

The localhost sites are run on:
- **Client:**  http://localhost:5173
- **Server:**  http://localhost:5211

Required configuration sections:
- `AppOptions:DBConnectionString` - PostgreSQL connection string
- `JwtKey` - Base64 encoded key for JWT token signing

General development information
- **Swagger/OpenAPI**: Available in development environment
- **Title**: "Dead Pidgeons API"
- **Version**: v0.1
- **TypeScript Client**: Auto-generates client code to `client/src/generated-ts-client.ts`



**Current state of the application**

The application is still in development, and cannot be declared ready for deployment.
While there is a graphical user interface for the users, and a functional database in the backend,
there needs to be more feedback to the user surrounding their actions and the effect of the backend and database.
Some database interactions and general logic could be improved, and has to be inspected for future development.

All required features by the exam project have been implemented, but the project still lacks features to increase the user experience.



