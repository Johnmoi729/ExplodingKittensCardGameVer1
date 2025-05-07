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
