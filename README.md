# PostsAPI
A clinics management API for managing Clinics, Doctors, Specializations, and Appointments Reservations, Built with ASP.NET Core, EF Core, SQL Server, JWT, and Layered Architecture.

## Features
- **Authentication system & role based authorization:** Full identity and role system for different roles.
- **Doctors membership requests system:** Ability for doctors to apply a become-a-doctor request and get either approved or rejected by admins.
- **Fully functional appointments reservation system:** Booking appointemtns for patients with doctors in clinics with deep schedule validation.
- **Specializations and clinics system:** A doctor get to be assigned to specializations and clinics.
- **Data fetching with searching and pagination:** Accessible endpoints for searching clinics and doctors.
- **Secured sensitive system endpoints:** Such as creation and manipulation of clinics and specializations as well as navigating through unapproved doctors.

## Run it
To run the full API locally, follow these steps:

1- Make sure to add the database connection string and the JWT key to user secrets:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your string>"
dotnet user-secrets set "Jwt:SecretKey" "<a long random string>"
```
2- Migrate to the database
```bash
dotnet ef database update
```
2- Run the project:
```bash
dotnet run
```

## API overview
| Method | Route | Auth | What it does |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | none | registers a new user using email and password |
| POST | `/api/auth/login` | none | logs in with credentials and returns a JWT token |
| GET | `/api/clinics` | none | returns all clinics |
| GET | `/api/clinics/{id}` | none | returns a clinic of specific ID |
| POST | `/api/clinics` | admin only | creates a new clinic |
| PUT | `/api/clinics/{id}` | admin only | updates a clinic of specific ID |
| DELETE | `/api/clinics/{id}` | admin only | deletes a clinic of specific ID |
| POST | `/api/clinics/{id}/assign-doctor/{doc_id}` | admin only | assigns a doctor to a clinic |
| POST | `/api/clinics/{id}/unassign-doctor/{doc_id}` | admin only | removes a doctor from a clinic |
| GET | `/api/doctors` | none | returns all doctors |
| GET | `/api/doctors/{id}` | none | returns a doctor of specific ID |
| POST | `/api/doctors/become-a-doctor` | authorized | submits a doctor application, which stays pending until an admin approves it |
| GET | `/api/doctors/pending-doctors` | admin only | returns doctor applications waiting for approval |
| PATCH | `/api/doctors/approve-doctor/{id}` | admin only | approves a pending doctor application |
| PATCH | `/api/doctors/reject-doctor/{id}` | admin only | rejects a pending doctor application |
| DELETE | `/api/doctors/delete-doctor/{id}` | admin only | deletes a doctor of specific ID |
| GET | `/api/specializations` | none | returns all specializations |
| GET | `/api/specializations/{id}` | none | returns a specialization of specific ID |
| POST | `/api/specializations` | admin only | creates a new specialization |
| PUT | `/api/specializations/{id}` | admin only | updates a specialization of specific ID |
| DELETE | `/api/specializations/{id}` | admin only | deletes a specialization of specific ID |
| GET | `/api/appointments/{id}` | patients and doctors only | returns an appointment of specific ID |
| POST | `/api/appointments` | patients only | books an appointment with specified doctor in specific clinic |

## This app is built using layered architecture
