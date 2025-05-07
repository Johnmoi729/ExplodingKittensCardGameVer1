import axios from "./apiClient";

// Base URL for game endpoints
const BASE_URL = "/api/games";

export const gameApi = {
  // Get all active games
  getActiveGames: async () => {
    const response = await axios.get(BASE_URL);
    return response.data;
  },

  // Get a specific game
  getGame: async (gameId) => {
    const response = await axios.get(`${BASE_URL}/${gameId}`);
    return response.data;
  },

  // Get games for a player
  getPlayerGames: async (playerId) => {
    const response = await axios.get(`${BASE_URL}/player/${playerId}`);
    return response.data;
  },

  // Create a new game
  createGame: async (gameData) => {
    const response = await axios.post(BASE_URL, gameData);
    return response.data;
  },

  // Join a game
  joinGame: async (gameId, playerId) => {
    const response = await axios.post(`${BASE_URL}/${gameId}/join`, playerId);
    return response.data;
  },

  // Start a game
  startGame: async (gameId) => {
    const response = await axios.post(`${BASE_URL}/${gameId}/start`);
    return response.data;
  },

  // Get game state
  getGameState: async (gameId) => {
    const response = await axios.get(`${BASE_URL}/${gameId}/state`);
    return response.data;
  },

  // Play a card
  playCard: async (gameId, playCardData) => {
    const response = await axios.post(
      `${BASE_URL}/${gameId}/actions/play-card`,
      playCardData
    );
    return response.data;
  },

  // Draw a card
  drawCard: async (gameId, drawCardData) => {
    const response = await axios.post(
      `${BASE_URL}/${gameId}/actions/draw-card`,
      drawCardData
    );
    return response.data;
  },

  // Play a special combo
  playCombo: async (gameId, playComboData) => {
    const response = await axios.post(
      `${BASE_URL}/${gameId}/actions/play-combo`,
      playComboData
    );
    return response.data;
  },

  // See the future
  seeFuture: async (gameId, playerId) => {
    const response = await axios.get(
      `${BASE_URL}/${gameId}/actions/see-future?playerId=${playerId}`
    );
    return response.data;
  },

  // Defuse an exploding kitten
  defuseKitten: async (gameId, defuseData) => {
    const response = await axios.post(
      `${BASE_URL}/${gameId}/actions/defuse-kitten`,
      defuseData
    );
    return response.data;
  },
};
