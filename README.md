<!--v004-->
# Stargate

***

## Astronaut Career Tracking System (ACTS)

ACTS is used as a tool to maintain a record of all the People that have served as Astronauts. When serving as an Astronaut, your *Job* (Duty) is tracked by your Rank, Title and the Start and End Dates of the Duty.

The People that exist in this system are not all Astronauts. ACTS maintains a master list of People and Duties that are updated from an external service (not controlled by ACTS). The update schedule is determined by the external service.

## How to Run

```
git pull https://github.com/m1cr0wav3977/stargate.git
```
```
docker compose up
```

A lightweight web ui will be available at http://localhost

The api port is bound to the host and routed for testing convenience and all API routes are available at http://localhost/api/\<route\> or http://localhost:5204/\<route\>

## Run Unit Tests

Unit tests for the API are available at stargate-api/StargateAPI.Tests

cd into the folder then run with
```
dotnet test
```
## Other Tests

Under stargate-api there are also 2 AI-created bash scripts for quickly checking SQL injection security and business logic:
- quick-integration-test.sh
- sql-injection-test.sh

## Definitions

1. A person's astronaut assignment is the Astronaut Duty.
1. A person's current astronaut information is stored in the Astronaut Detail table.
1. A person's list of astronaut assignments is stored in the Astronaut Duty table.

## Requirements

##### Enhance the Stargate API (Required)

The REST API is expected to do the following:

1. Retrieve a person by name.       [ x ]
2. Retrieve all people.             [ x ]
3. Add/update a person by name.     [ x ] 
4. Retrieve Astronaut Duty by name. [ x ]
5. Add an Astronaut Duty.           [ x ] 

##### Implement a user interface: (Encouraged)

The UI is expected to do the following:

1. Successfully implement a web application that demonstrates production level quality. Angular is preferred.
1. Implement call(s) to retrieve an individual's astronaut duties.
1. Display the progress of the process and the results in a visually sophisticated and appealing manner.

## Tasks

Overview
Examine the code, find and resolve any flaws, if any exist. Identify design patterns and follow or change them. Provide fix(es) and be prepared to describe the changes.

1. Generate the database
   * This is your source and storage location
1. Enforce the rules
1. Improve defensive coding
1. Add unit tests
   * identify the most impactful methods requiring tests
   * reach >50% code coverage
1. Implement process logging
   * Log exceptions
   * Log successes
   * Store the logs in the database

## Rules

1. A Person is uniquely identified by their Name.
1. A Person who has not had an astronaut assignment will not have Astronaut records.
1. A Person will only ever hold one current Astronaut Duty Title, Start Date, and Rank at a time.
1. A Person's Current Duty will not have a Duty End Date.
1. A Person's Previous Duty End Date is set to the day before the New Astronaut Duty Start Date when a new Astronaut Duty is received for a Person.
1. A Person is classified as 'Retired' when a Duty Title is 'RETIRED'.
1. A Person's Career End Date is one day before the Retired Duty Start Date.

## Implementation

Person:
- id
- Name (must be unique)
- AstronautDetail
- AstronautDuties

AstronautDetail
- id
- personID
- CurrentRank
- CurrentDutyTitle
- Career Start Date
- Career End Date
- Person

AstronautDuty
- id
- personID
- Rank
- DutyTitle
- DutyStartDate
- DutyEndDate
- Person

Backend
- when you create a person, it attaches a name only, verifies name is unique
- adding a duty of spaceman by name creates an astronautdetail (enrolling them in astronaut program) and set their start date
- adding a new duty will update relevant details in the astronautdetail, handle dates in the astronautduty
- adding a duty of "retired" will add a career end date 1 day before the retired duty start date
- deleting a person will cascade to include deleting astronautdetails or astronaut duties if they apply

Frontend
- 2 page app with title bar
- first page shows all current employee names, can add a new employee
   - delete button - if astronaut will also delete detail and all duties
- second page shows astronauts and their duties, option to "add astronaut"
   - grabs all employees without an astronaut detail, you select
   - assigns duty of spaceman
- each astronaut has button to add duty
   - add duty menu allows you to enter duty information or select retire

Logging
- log to stdout successes and exceptions 
   - I chose this design change purely based on preference
   - to implement logging to the database, I would implement a new entity and attach it to the Stargate context

Defensive coding
- SQL injection protection in preprocessors?

Add Unit Tests, most critical:
- update duty route
- create person route

write a build script that runs all unit tests then calls the docker compose 

## API Endpoints

### Person Endpoints

| Method | Path | Command/Query | Description |
|--------|------|---------------|-------------|
| `POST` | `/Person` | `CreatePerson` | Create a new person. Validates that name is unique. |
| `GET` | `/Person` | `GetPeople` | Retrieve all people. |
| `GET` | `/Person/name/{name}` | `GetPersonByName` | Retrieve a person by name (includes AstronautDetail if available). |
| `GET` | `/Person/{id}` | `GetPersonById` | Retrieve a person by ID. |
| `PUT` | `/Person/{id}` | `UpdatePerson` | Update a person by ID. |
| `DELETE` | `/Person/{id}` | `DeletePerson` | Delete a person by ID (cascades to AstronautDetail and AstronautDuties). |

### AstronautDetail Endpoints

| Method | Path | Command/Query | Description |
|--------|------|---------------|-------------|
| `GET` | `/AstronautDetail/person/{personId}` | `GetAstronautDetailByPersonId` | Retrieve astronaut detail by person ID. |
| `GET` | `/AstronautDetail/{id}` | `GetAstronautDetailById` | Retrieve astronaut detail by ID. |
| ~~`POST`~~ | ~~`/AstronautDetail`~~ | ~~`CreateAstronautDetail`~~ | ~~Disabled - AstronautDetail is created automatically when first duty is added.~~ |
| ~~`PUT`~~ | ~~`/AstronautDetail/{id}`~~ | ~~`UpdateAstronautDetail`~~ | ~~Disabled - AstronautDetail is updated automatically when duties are added.~~ |
| ~~`DELETE`~~ | ~~`/AstronautDetail/{id}`~~ | ~~`DeleteAstronautDetail`~~ | ~~Disabled - AstronautDetail is deleted via cascade when person is deleted.~~ |

### AstronautDuty Endpoints

| Method | Path | Command/Query | Description |
|--------|------|---------------|-------------|
| `POST` | `/AstronautDuty` | `AddAstronautDuty` | Add a new astronaut duty. First duty must be "Spaceman". Automatically creates/updates AstronautDetail. |
| `GET` | `/AstronautDuty/name/{name}` | `GetAstronautDutiesByName` | Retrieve all astronaut duties for a person by name. |
| `GET` | `/AstronautDuty/{id}` | `GetAstronautDutyById` | Retrieve an astronaut duty by ID. |
| ~~`PUT`~~ | ~~`/AstronautDuty/{id}`~~ | ~~`UpdateAstronautDuty`~~ | ~~Disabled.~~ |
| ~~`DELETE`~~ | ~~`/AstronautDuty/{id}`~~ | ~~`DeleteAstronautDuty`~~ | ~~Disabled - AstronautDuties are deleted via cascade when person is deleted.~~ |

**Note:** Endpoints marked with ~~strikethrough~~ are commented out in the controllers and not available.
