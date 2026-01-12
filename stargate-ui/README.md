# Stargate UI

Simple Angular frontend for the Stargate Astronaut Career Tracking System.

## Features

- **People Page**: View all people, add new people, delete people (with cascade delete for astronauts)
- **Astronauts Page**: View astronauts with their duty history, add new astronauts, add duties

## Development

### Prerequisites
- Node.js 18+
- npm

### Running the Application

1. Install dependencies:
```bash
npm install
```

2. Start the development server:
```bash
npm start
```

The application will be available at `http://localhost:4200`

### Building for Production

```bash
npm run build
```

The built files will be in the `dist/stargate-ui` directory.

## API Configuration

The API URL is configured in `src/app/services/api.service.ts`. By default, it points to:
- `http://localhost:5204`

Update this if your API is running on a different URL or port.
