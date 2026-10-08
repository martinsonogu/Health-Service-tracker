# Health-Service-tracker
# Community Health Service Tracker

A portfolio web application for recording, managing and monitoring fictional community health service referrals.

Built with **ASP.NET Core Razor Pages, C#, Entity Framework Core, SQLite, HTML, CSS and Bootstrap**, the project demonstrates full-stack development, automated testing, responsive interface design and accessibility-focused frontend practices.

> **Disclaimer:** This is an independent portfolio project, not an official NHS application. All referral information used for demonstration and testing is fictional. No real patient data should be entered.

## Features

- Dashboard showing total referrals, referral statuses, urgent referrals and recent activity.
- Create, view, edit and delete referrals.
- Search referrals by reference, service or assigned team.
- Filter referrals by status and priority.
- Responsive navigation, dashboard cards, forms and referral tables.
- Form validation with required-field messages.
- Accessibility-focused features including skip navigation, keyboard focus indicators, semantic landmarks, labelled forms and descriptive action links.

## Technology stack

| Area | Technology |
|---|---|
| Application framework | ASP.NET Core Razor Pages (.NET 10) |
| Programming language | C# |
| Database | SQLite |
| Data access | Entity Framework Core |
| Frontend | HTML, CSS, Bootstrap, Razor |
| Automated testing | xUnit, EF Core with in-memory SQLite |
| Code coverage | Coverlet |

## Getting started

### Prerequisites

- .NET 10 SDK
- Git

### Run locally

Clone the repository:

```bash
git clone https://github.com/martinsonogu/Health-Service-tracker.git
cd Health-Service-tracker
```

Restore dependencies:

```bash
dotnet restore
```

Create or update the local database:

```bash
dotnet ef database update
```

Run the application:

```bash
dotnet run
```

Open the local URL shown in the terminal.

The SQLite database is stored locally and is excluded from version control.

## Automated testing

Run the test suite:

```bash
dotnet test CommunityHealthServiceTracker.Tests/CommunityHealthServiceTracker.Tests.csproj
```

The project currently has **33 passing automated tests**, covering referral validation, filtering, creation, editing, deletion, details and dashboard behaviour.

### Code coverage

Generate a coverage report:

```bash
dotnet test CommunityHealthServiceTracker.Tests/CommunityHealthServiceTracker.Tests.csproj --collect:"XPlat Code Coverage" --settings coverage.runsettings
```

The latest measured results were:

- **87.4% line coverage**
- **82.3% branch coverage**

These figures apply to the configured coverage scope, which excludes generated Razor and migration files. They are not a measure of overall frontend accessibility or visual testing.

## Accessibility

The interface includes accessibility-focused design and implementation practices:

- Semantic HTML landmarks and navigation.
- Skip-to-main-content link.
- Visible keyboard focus indicators.
- Active-page identification using `aria-current`.
- Accessible labels and validation messages for required fields.
- Table column and row headings.
- Keyboard-focusable horizontally scrollable referral tables.
- Responsive layouts for smaller screens.

Accessibility testing is ongoing. **The application has not been certified or fully audited for WCAG 2.2 conformance.**

## Project purpose

This project was developed to demonstrate practical software engineering skills relevant to community health and public-sector digital services, including maintainable C# code, data-driven interfaces, automated testing, usability and accessibility.

## Future improvements

- Further keyboard and screen-reader accessibility testing.
- Authentication and role-based authorisation.
- Continuous integration using GitHub Actions.
- Additional frontend and integration tests.
- Deployment to a hosted environment.

## Data and privacy

This application is intended for demonstration purposes only. It uses a local SQLite database and is not designed or approved for storing real patient or sensitive personal information.