# TalentHub Architecture

## Overview

TalentHub uses a Clean Architecture-oriented layout with an Angular frontend and a .NET backend.

## Frontend

- `frontend/TalentHub.Web`
- Angular standalone application
- Presentation and client-side features belong here

## Backend

- `backend/TalentHub.sln`
- `src/TalentHub.Web.API`
- `src/TalentHub.Web.Main`
- `src/TalentHub.Application`
- `src/TalentHub.Domain`
- `src/TalentHub.Infrastructure`
- `src/TalentHub.Integration.Sql`
- `src/TalentHub.Integration.Communication`
- `src/TalentHub.Integration.RemoteOK`
- `src/TalentHub.Integration.Greenhouse`
- `src/TalentHub.Integration.Lever`
- `src/TalentHub.Integration.OpenAI`
- `tests/TalentHub.Tests`

## Dependency Direction

- `Domain` is the innermost layer.
- `Application` depends on `Domain`.
- `Infrastructure` depends on `Application` and `Domain`.
- `Web.Main` depends on `Application`.
- `Web.API` depends on `Web.Main`, `Application`, and `Infrastructure`.
- Integration projects depend on `Application` and, where appropriate, `Infrastructure`.
- Tests depend on the application and domain layers.

## Notes

All projects include a `DependencyInjection.cs` entry point and placeholder types only. No business logic is implemented.
