# Exploding Kittens Game - Project Reference Guide

## Project Overview
An online multiplayer card game based on the popular Exploding Kittens card game, developed using .NET 9.0 for the backend and React for the frontend. The game follows the official rules where players take turns drawing cards until someone draws an Exploding Kitten and loses, unless they have a Defuse card.

### Timeline
- **3-week project** with focused milestones:
  - Week 1: Setup, Database, and Basic Backend
  - Week 2: Game Logic and Frontend Structure
  - Week 3: Integration, Polish, and Deployment

## Technical Stack

### Backend
- **.NET 9.0** with Clean Architecture pattern
- **Repository Pattern** for data access
- **MongoDB** for database (document-based)
- **SignalR** for real-time communications
- **JWT Authentication** for user management
- **xUnit** for testing

### Frontend
- **React** for UI components
- **PropTypes and JSDoc** for type safety (alternative to TypeScript)
- **Axios** for API calls
- **SignalR client** for real-time updates
- **Jest** for testing

### Deployment
- **Azure** cloud platform
  - App Service for backend
  - Static Web Apps for frontend
  - Cosmos DB (MongoDB API) for database

## Game Rules Summary

### Basic Gameplay
1. Players are dealt 7 cards + 1 Defuse card
2. Players take turns playing cards or drawing from the deck
3. If a player draws an Exploding Kitten, they must use a Defuse card or they're eliminated
4. Last player remaining wins

### Card Types
- **Exploding Kitten**: Eliminates the player who draws it unless defused
- **Defuse**: Allows player to save themselves from an Exploding Kitten
- **Attack**: Ends turn without drawing; forces next player to take 2 turns
- **Skip**: Ends turn without drawing a card
- **Shuffle**: Randomly shuffles the draw pile
- **See Future**: Privately view the top 3 cards of the draw pile
- **Favor**: Force another player to give you a card of their choice
- **Cat Cards**: Powerless on their own, but can be played as pairs to steal random cards

### Special Combos
- **Two of a Kind**: Play any matching pair to steal a random card from another player
- **Three of a Kind**: Play any matching trio to name and take a specific card from another player

## Database Schema

### Collections
1. **User Collection**
   - `_id`: string
   - `username`: string
   - `email`: string
   - `passwordHash`: string
   - `createdAt`: date
   - `gamesPlayed`: int
   - `gamesWon`: int

2. **Game Collection**
   - `_id`: string
   - `name`: string
   - `players`: array of user IDs
   - `status`: string ("waiting", "in_progress", "completed")
   - `currentPlayerId`: string
   - `turnNumber`: int
   - `createdAt`: date
   - `updatedAt`: date

3. **GameState Collection**
   - `_id`: string
   - `gameId`: string
   - `drawPile`: array of card IDs
   - `discardPile`: array of card IDs
   - `playerHands`: object (player ID -> array of card IDs)
   - `explodedPlayers`: array of player IDs
   - `attackCount`: int
   - `lastAction`: string
   - `updatedAt`: date

4. **Card Collection**
   - `_id`: string
   - `type`: string
   - `name`: string
   - `effect`: string
   - `imageUrl`: string

5. **PlayerAction Collection**
   - `_id`: string
   - `gameId`: string
   - `playerId`: string
   - `actionType`: string
   - `cardsPlayed`: array of card IDs
   - `targetPlayerId`: string
   - `timestamp`: date

## Project Structure

### Backend Structure
```
Backend/
├── ExplodingKittens.API/                      # Web API project
│   ├── Controllers/                           # API endpoints
│   ├── Middleware/                            # Custom middleware
│   ├── Hubs/                                  # SignalR hubs
│   ├── Auth/                                  # JWT authentication
│   └── Program.cs                             # Application entry point
│
├── ExplodingKittens.Application/              # Application layer
│   ├── DTOs/                                  # Data transfer objects
│   ├── Interfaces/                            # Service interfaces
│   ├── Mappings/                              # Object mapping profiles
│   └── Services/                              # Business logic implementation
│
├── ExplodingKittens.Domain/                   # Domain layer
│   ├── Entities/                              # Domain entities
│   ├── Enums/                                 # Enumerations
│   └── Constants/                             # Game constants
│
├── ExplodingKittens.Infrastructure/           # Infrastructure layer
│   ├── Data/                                  # Data access
│   │   ├── Context/                           # MongoDB context
│   │   ├── Repositories/                      # Repository implementations
│   │   └── Configurations/                    # MongoDB configurations
│   ├── Services/                              # External service integrations
│   └── Extensions/                            # Service collection extensions
│
├── ExplodingKittens.GameEngine/               # Game logic
│   ├── Actions/                               # Game actions
│   ├── Rules/                                 # Game rules
│   ├── States/                                # Game states
│   ├── Combos/                                # Special combo handling
│   └── GameEngine.cs                          # Main game logic
│
└── ExplodingKittens.Tests/                    # Test projects
    ├── Unit/                                  # Unit tests
    └── Integration/                           # Integration tests
```

### Frontend Structure
```
Frontend/
├── public/                                    # Static assets
│   ├── images/                                # Card images
│   └── sounds/                                # Game sounds
│
├── src/
│   ├── api/                                   # API client
│   │   ├── gameApi.js                         # Game API methods + SignalR
│   │   ├── userApi.js                         # User/auth API methods
│   │   └── apiClient.js                       # Base API configuration
│   │
│   ├── components/                            # React components
│   │   ├── common/                            # Shared components
│   │   ├── game/                              # Game-specific components
│   │   ├── auth/                              # Authentication components
│   │   └── layout/                            # Layout components
│   │
│   ├── contexts/                              # React contexts
│   │   ├── AuthContext.jsx                    # Authentication state
│   │   └── GameContext.jsx                    # Game state
│   │
│   ├── hooks/                                 # Custom hooks
│   │   ├── useGame.js                         # Game logic hooks
│   │   └── useAuth.js                         # Authentication hooks
│   │
│   ├── pages/                                 # Page components
│   │   ├── Home.jsx                           # Landing page
│   │   ├── Game.jsx                           # Main game page
│   │   ├── Login.jsx                          # Login page
│   │   └── Register.jsx                       # Registration page
│   │
│   ├── tests/                                 # Tests
│   │   ├── unit/                              # Unit tests
│   │   └── integration/                       # Integration tests
│   │
│   ├── utils/                                 # Utility functions
│   │   ├── cardUtils.js                       # Card-related utilities
│   │   └── gameUtils.js                       # Game-related utilities
│   │
│   └── App.jsx                                # Main application component
```

## Implementation Guidelines

### SignalR Integration

SignalR will be used for real-time communication between the server and clients. Key aspects:

1. **GameHub**: Central hub for all real-time game communications
2. **Group Management**: Players in a game join a SignalR group based on game ID
3. **Key Events to Broadcast**:
   - When a card is played
   - When a player's turn begins
   - When game state changes
   - When a player explodes
   - When a player joins/leaves

### Authentication Flow

1. **Registration**: Users provide username, email, and password
2. **Login**: Users authenticate and receive a JWT token
3. **Token Storage**: Store JWT in local storage
4. **Protected Routes**: Game-related routes require authentication
5. **Token Refresh**: Implement token refresh mechanism for longer sessions

### Game Flow Implementation

1. **Game Creation**:
   - User creates a game
   - System generates a unique game ID
   - Creator is designated as host

2. **Game Joining**:
   - Other players join using game ID
   - Players enter waiting room
   - Host initiates game start

3. **Game Initialization**:
   - Deck is created and shuffled
   - Cards are dealt to players
   - Initial game state is saved
   - First player is randomly selected

4. **Turn Execution**:
   - Player can play cards or draw
   - Card effects are processed
   - Turn advances to next player unless interrupted

5. **Game Completion**:
   - Last player standing wins
   - Stats are updated
   - Results are displayed

### Testing Strategy

1. **Backend Testing**:
   - Unit tests for services and repositories
   - Integration tests for API endpoints
   - Game logic tests with various scenarios

2. **Frontend Testing**:
   - Component tests with Jest
   - UI interaction tests
   - Integration tests for API calls

## Key Technologies Explained

### MongoDB in .NET
MongoDB is a document database that stores data in flexible, JSON-like documents. For this project:
- Store game state as documents
- Use MongoDB.Driver package for .NET
- Implement Repository pattern for data access

### SignalR for Real-time Updates
SignalR provides real-time web functionality:
- Automatically selects best transport method (WebSockets, Server-Sent Events, Long Polling)
- Handles reconnection automatically
- Scales across multiple servers

### PropTypes vs TypeScript
PropTypes is a React-specific library for runtime type checking:
- Validates React component props only
- Simpler than TypeScript but less comprehensive
- Combined with JSDoc for better developer experience

### JWT Authentication
JSON Web Tokens for secure authentication:
- Stateless authentication mechanism
- Contains encoded user information
- Verified using a secret key

## Development Workflow

### Week 1: Setup, Database, and Basic Backend
- Day 1-2: Project setup and infrastructure
- Day 3-4: Database schema implementation
- Day 5-7: Core backend services and API endpoints

### Week 2: Game Logic and Frontend Structure
- Day 1-3: Game engine and core mechanics
- Day 4-7: Frontend foundation and basic UI components

### Week 3: Integration, Polish, and Deployment
- Day 1-3: Connect frontend and backend
- Day 4-5: Testing and bug fixes
- Day 6-7: Azure deployment and final adjustments

## Useful Commands

### .NET Commands
```powershell
# Run the API
cd Backend/ExplodingKittens.API
dotnet run

# Run tests
cd Backend/ExplodingKittens.Tests
dotnet test

# Add a migration
dotnet ef migrations add MigrationName
```

### Frontend Commands
```powershell
# Start development server
cd Frontend
npm run dev

# Run tests
npm test

# Build for production
npm run build
```

### MongoDB Commands
```
# Connect to local MongoDB
mongosh

# List databases
show dbs

# Select database
use ExplodingKittens

# List collections
show collections

# Find documents
db.Games.find()
```

## Important Notes

1. **Game Rules Adherence**: Follow the exact rules from the Exploding Kittens rulebook for accurate implementation.

2. **SignalR Connection Management**: Handle connection drops and reconnections gracefully.

3. **Card Effects**: Implement each card effect according to the rules, especially Attack card stacking.

4. **Security**: Validate all game actions server-side to prevent cheating.

5. **Testing Priority**: Focus on testing core game logic thoroughly.

This reference guide contains all the essential information about your Exploding Kittens project. Use it as your primary reference to maintain consistency and focus throughout the development process.
