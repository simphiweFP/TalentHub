# TalentHub

TalentHub is a Clean Architecture foundation containing:

- `frontend/TalentHub.Web` - Angular frontend application
- `backend/TalentHub.sln` - .NET backend solution

The backend projects are organized to keep dependencies pointing inward:

- `TalentHub.Domain`
- `TalentHub.Application`
- `TalentHub.Infrastructure`
- `TalentHub.Web.Main`
- `TalentHub.Web.API`
- `TalentHub.Integration.*`
- `TalentHub.Tests`

No business logic is implemented yet. This repository is a foundation for future development.
