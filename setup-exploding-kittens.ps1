# Exploding Kittens Game Setup Script with Suggested Improvements
# PowerShell Version for Windows

# Create root directory
New-Item -Path "ExplodingKittens" -ItemType Directory -Force
Set-Location -Path "ExplodingKittens"

# Backend setup with enhanced structure
$backendFolders = @(
    "Backend\ExplodingKittens.API\Controllers",
    "Backend\ExplodingKittens.API\Middleware",
    "Backend\ExplodingKittens.API\Hubs",
    "Backend\ExplodingKittens.API\Auth",
    "Backend\ExplodingKittens.Application\DTOs",
    "Backend\ExplodingKittens.Application\Interfaces",
    "Backend\ExplodingKittens.Application\Mappings",
    "Backend\ExplodingKittens.Application\Services",
    "Backend\ExplodingKittens.Domain\Entities",
    "Backend\ExplodingKittens.Domain\Enums",
    "Backend\ExplodingKittens.Domain\Constants",
    "Backend\ExplodingKittens.Infrastructure\Data\Context",
    "Backend\ExplodingKittens.Infrastructure\Data\Repositories",
    "Backend\ExplodingKittens.Infrastructure\Data\Configurations",
    "Backend\ExplodingKittens.Infrastructure\Services",
    "Backend\ExplodingKittens.Infrastructure\Extensions",
    "Backend\ExplodingKittens.GameEngine\Actions",
    "Backend\ExplodingKittens.GameEngine\Rules",
    "Backend\ExplodingKittens.GameEngine\States",
    "Backend\ExplodingKittens.GameEngine\Combos",
    "Backend\ExplodingKittens.Tests\Unit",
    "Backend\ExplodingKittens.Tests\Integration"
)

foreach ($folder in $backendFolders) {
    New-Item -Path $folder -ItemType Directory -Force
}

# Frontend setup with testing and PropTypes support
$frontendFolders = @(
    "Frontend\public\images",
    "Frontend\public\sounds",
    "Frontend\src\api",
    "Frontend\src\components\common",
    "Frontend\src\components\game",
    "Frontend\src\components\auth",
    "Frontend\src\components\layout",
    "Frontend\src\contexts",
    "Frontend\src\hooks",
    "Frontend\src\pages",
    "Frontend\src\utils",
    "Frontend\src\tests\unit",
    "Frontend\src\tests\integration"
)

foreach ($folder in $frontendFolders) {
    New-Item -Path $folder -ItemType Directory -Force
}

# Deployment setup
New-Item -Path "Deployment" -ItemType Directory -Force

# Create solution file
Set-Location -Path "Backend"
dotnet new sln -n ExplodingKittens -o .

# Create projects
dotnet new webapi -n ExplodingKittens.API -o ExplodingKittens.API
dotnet new classlib -n ExplodingKittens.Application -o ExplodingKittens.Application
dotnet new classlib -n ExplodingKittens.Domain -o ExplodingKittens.Domain
dotnet new classlib -n ExplodingKittens.Infrastructure -o ExplodingKittens.Infrastructure
dotnet new classlib -n ExplodingKittens.GameEngine -o ExplodingKittens.GameEngine
dotnet new xunit -n ExplodingKittens.Tests -o ExplodingKittens.Tests

# Add projects to solution
dotnet sln add ExplodingKittens.API\ExplodingKittens.API.csproj
dotnet sln add ExplodingKittens.Application\ExplodingKittens.Application.csproj
dotnet sln add ExplodingKittens.Domain\ExplodingKittens.Domain.csproj
dotnet sln add ExplodingKittens.Infrastructure\ExplodingKittens.Infrastructure.csproj
dotnet sln add ExplodingKittens.GameEngine\ExplodingKittens.GameEngine.csproj
dotnet sln add ExplodingKittens.Tests\ExplodingKittens.Tests.csproj

# Add project references
dotnet add ExplodingKittens.API\ExplodingKittens.API.csproj reference ExplodingKittens.Application\ExplodingKittens.Application.csproj
dotnet add ExplodingKittens.Application\ExplodingKittens.Application.csproj reference ExplodingKittens.Domain\ExplodingKittens.Domain.csproj
dotnet add ExplodingKittens.Application\ExplodingKittens.Application.csproj reference ExplodingKittens.GameEngine\ExplodingKittens.GameEngine.csproj
dotnet add ExplodingKittens.Infrastructure\ExplodingKittens.Infrastructure.csproj reference ExplodingKittens.Domain\ExplodingKittens.Domain.csproj
dotnet add ExplodingKittens.Infrastructure\ExplodingKittens.Infrastructure.csproj reference ExplodingKittens.Application\ExplodingKittens.Application.csproj
dotnet add ExplodingKittens.GameEngine\ExplodingKittens.GameEngine.csproj reference ExplodingKittens.Domain\ExplodingKittens.Domain.csproj
dotnet add ExplodingKittens.Tests\ExplodingKittens.Tests.csproj reference ExplodingKittens.API\ExplodingKittens.API.csproj
dotnet add ExplodingKittens.Tests\ExplodingKittens.Tests.csproj reference ExplodingKittens.Application\ExplodingKittens.Application.csproj
dotnet add ExplodingKittens.Tests\ExplodingKittens.Tests.csproj reference ExplodingKittens.Domain\ExplodingKittens.Domain.csproj
dotnet add ExplodingKittens.Tests\ExplodingKittens.Tests.csproj reference ExplodingKittens.Infrastructure\ExplodingKittens.Infrastructure.csproj
dotnet add ExplodingKittens.Tests\ExplodingKittens.Tests.csproj reference ExplodingKittens.GameEngine\ExplodingKittens.GameEngine.csproj

# Add NuGet packages with new improvements
# API - Add SignalR and JWT Authentication
Set-Location -Path "ExplodingKittens.API"
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.SignalR
dotnet add package Swashbuckle.AspNetCore
dotnet add package MongoDB.Driver
Set-Location -Path ".."

# Application - No changes
Set-Location -Path "ExplodingKittens.Application"
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package FluentValidation
dotnet add package MediatR
Set-Location -Path ".."

# Infrastructure - No changes
Set-Location -Path "ExplodingKittens.Infrastructure"
dotnet add package MongoDB.Driver
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Microsoft.Extensions.Options.ConfigurationExtensions
Set-Location -Path ".."

# GameEngine - No changes
Set-Location -Path "ExplodingKittens.GameEngine"
dotnet add package Microsoft.Extensions.DependencyInjection
Set-Location -Path ".."

# Tests - Add xUnit testing framework
Set-Location -Path "ExplodingKittens.Tests"
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
Set-Location -Path ".."

# Set up frontend with React, PropTypes, and Jest
Set-Location -Path "..\Frontend"
npm init -y
npm install react react-dom react-router-dom axios prop-types
npm install --save-dev jest @testing-library/react @testing-library/jest-dom @testing-library/user-event
npm install --save-dev vite @vitejs/plugin-react eslint eslint-plugin-react eslint-plugin-react-hooks

# Create package.json with testing configuration
$packageJson = @'
{
  "name": "exploding-kittens-frontend",
  "private": true,
  "version": "0.1.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "preview": "vite preview",
    "test": "jest"
  },
  "jest": {
    "testEnvironment": "jsdom",
    "setupFilesAfterEnv": [
      "<rootDir>/src/tests/setupTests.js"
    ],
    "moduleNameMapper": {
      "\\.(css|less|scss|sass)$": "identity-obj-proxy"
    }
  }
}
'@
Set-Content -Path "package.json" -Value $packageJson

# Create Jest setup file
New-Item -Path "src\tests" -ItemType Directory -Force
Set-Content -Path "src\tests\setupTests.js" -Value "import '@testing-library/jest-dom';"

# Create vite.config.js
$viteConfig = @'
import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";

export default defineConfig({
  plugins: [react()],
});
'@
Set-Content -Path "vite.config.js" -Value $viteConfig

# Create index.html
$indexHtml = @'
<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="UTF-8" />
    <link rel="icon" type="image/svg+xml" href="/favicon.svg" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Exploding Kittens</title>
  </head>
  <body>
    <div id="root"></div>
    <script type="module" src="/src/index.jsx"></script>
  </body>
</html>
'@
Set-Content -Path "index.html" -Value $indexHtml

# Create SignalR hub
Set-Location -Path "..\Backend\ExplodingKittens.API\Hubs"
$gameHub = @'
// GameHub.cs - SignalR hub for real-time game communication
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ExplodingKittens.API.Hubs
{
    public class GameHub : Hub
    {
        public async Task JoinGame(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        public async Task LeaveGame(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }
    }
}
'@
Set-Content -Path "GameHub.cs" -Value $gameHub

# Create JWT Authentication service stub
Set-Location -Path "..\Auth"
$jwtAuthService = @'
// JwtAuthService.cs - JWT Authentication service
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExplodingKittens.API.Auth
{
    public class JwtAuthService
    {
        // JWT token generation and validation will be implemented here
    }
}
'@
Set-Content -Path "JwtAuthService.cs" -Value $jwtAuthService

# Create example PropTypes and JSDoc usage
Set-Location -Path "..\..\..\Frontend\src\components\game"
New-Item -Path "Card" -ItemType Directory -Force
Set-Location -Path "Card"
$cardJsx = @'
// Card.jsx - Example of PropTypes and JSDoc usage
import React from "react";
import PropTypes from "prop-types";

/**
 * Card component representing a game card
 * @param {Object} props - Component props
 * @param {string} props.id - Unique identifier for the card
 * @param {string} props.type - Type of the card (e.g., "exploding_kitten", "defuse")
 * @param {string} props.name - Display name of the card
 * @param {string} props.imageUrl - URL to the card image
 * @param {Function} props.onPlay - Callback function when card is played
 */
function Card({ id, type, name, imageUrl, onPlay }) {
  return (
    <div className="card" onClick={() => onPlay(id)}>
      <img src={imageUrl} alt={name} />
      <h3>{name}</h3>
    </div>
  );
}

Card.propTypes = {
  id: PropTypes.string.isRequired,
  type: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  imageUrl: PropTypes.string.isRequired,
  onPlay: PropTypes.func.isRequired
};

export default Card;
'@
Set-Content -Path "Card.jsx" -Value $cardJsx

# Create Jest test example
Set-Location -Path "..\..\..\tests\unit"
$cardTest = @'
// Card.test.jsx - Example Jest test
import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import Card from '../../components/game/Card/Card';

describe('Card Component', () => {
  test('renders card with correct name', () => {
    const mockOnPlay = jest.fn();
    render(
      <Card 
        id='card-1'
        type='defuse'
        name='Defuse Card'
        imageUrl='/images/defuse.png'
        onPlay={mockOnPlay}
      />
    );
    
    expect(screen.getByText('Defuse Card')).toBeInTheDocument();
  });

  test('calls onPlay when clicked', () => {
    const mockOnPlay = jest.fn();
    render(
      <Card 
        id='card-1'
        type='defuse'
        name='Defuse Card'
        imageUrl='/images/defuse.png'
        onPlay={mockOnPlay}
      />
    );
    
    fireEvent.click(screen.getByText('Defuse Card'));
    expect(mockOnPlay).toHaveBeenCalledWith('card-1');
  });
});
'@
Set-Content -Path "Card.test.jsx" -Value $cardTest

# Create SignalR client connection
Set-Location -Path "..\..\api"
$gameApi = @'
// gameApi.js - API client with SignalR integration
import * as signalR from '@microsoft/signalr';
import axios from 'axios';

const API_URL = process.env.API_URL || 'http://localhost:5000';

// SignalR connection
let connection = null;

// Initialize SignalR connection
export const initializeSignalR = (gameId, onUpdateGameState) => {
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${API_URL}/gamehub`)
    .withAutomaticReconnect()
    .build();

  connection.on('UpdateGameState', onUpdateGameState);

  connection.start()
    .then(() => connection.invoke('JoinGame', gameId))
    .catch(err => console.error('SignalR Connection Error: ', err));

  return connection;
};

// Game API calls
export const createGame = async (gameName, hostPlayerId) => {
  const response = await axios.post(`${API_URL}/api/games`, {
    name: gameName,
    hostPlayerId
  });
  return response.data;
};

export const joinGame = async (gameId, playerId) => {
  const response = await axios.post(`${API_URL}/api/games/${gameId}/join`, {
    playerId
  });
  return response.data;
};

export const startGame = async (gameId) => {
  const response = await axios.post(`${API_URL}/api/games/${gameId}/start`);
  return response.data;
};

export const playCard = async (gameId, playerId, cardId, targetPlayerId = null) => {
  const response = await axios.post(`${API_URL}/api/games/${gameId}/play`, {
    playerId,
    cardId,
    targetPlayerId
  });
  return response.data;
};

export const drawCard = async (gameId, playerId) => {
  const response = await axios.post(`${API_URL}/api/games/${gameId}/draw`, {
    playerId
  });
  return response.data;
};

// Clean up SignalR connection
export const disconnectSignalR = async (gameId) => {
  if (connection) {
    await connection.invoke('LeaveGame', gameId);
    await connection.stop();
    connection = null;
  }
};
'@
Set-Content -Path "gameApi.js" -Value $gameApi

# Install SignalR client library
Set-Location -Path "..\.."
npm install @microsoft/signalr

Write-Host "Project setup completed successfully!" -ForegroundColor Green
