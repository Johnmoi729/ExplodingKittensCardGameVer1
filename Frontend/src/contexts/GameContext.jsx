import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import React, { createContext, useContext, useEffect, useState } from "react";
import { gameApi } from "../api/gameApi";
import { useAuth } from "./AuthContext";

// Create context
const GameContext = createContext();

export const useGame = () => useContext(GameContext);

export const GameProvider = ({ children }) => {
  const { user } = useAuth();
  const [currentGame, setCurrentGame] = useState(null);
  const [gameState, setGameState] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [connection, setConnection] = useState(null);
  const [seeFutureCards, setSeeFutureCards] = useState([]);

  // Initialize or reset SignalR connection when the game changes
  useEffect(() => {
    if (currentGame && user) {
      initializeSignalRConnection();
    }

    return () => {
      if (connection && connection.state === HubConnectionState.Connected) {
        connection.stop();
      }
    };
  }, [currentGame, user]);

  // Initialize SignalR connection
  const initializeSignalRConnection = async () => {
    try {
      // Build connection
      const newConnection = new HubConnectionBuilder()
        .withUrl(`${process.env.REACT_APP_API_URL}/gamehub`, {
          accessTokenFactory: () => localStorage.getItem("token"),
        })
        .withAutomaticReconnect()
        .build();

      // Set up event handlers
      newConnection.on("GameStateUpdated", (updatedGameState) => {
        setGameState(updatedGameState);
      });

      newConnection.on("CardPlayed", (data) => {
        console.log(`Player ${data.playerId} played ${data.cardName}`);
      });

      newConnection.on("CardDrawn", (data) => {
        console.log(`Player ${data.playerId} drew a card`);
      });

      newConnection.on("PlayerExploded", (data) => {
        console.log(`Player ${data.playerId} exploded!`);
      });

      newConnection.on("PlayerTurn", (data) => {
        console.log(`It's player ${data.playerId}'s turn`);
      });

      newConnection.on("GameEnded", (data) => {
        console.log(`Game ended. Player ${data.winnerId} wins!`);
      });

      newConnection.on("SeeFutureResult", (result) => {
        setSeeFutureCards(result.topCards);
      });

      // Start connection
      await newConnection.start();

      // Join game group
      await newConnection.invoke("JoinGameGroup", currentGame.id);

      // Save connection
      setConnection(newConnection);
    } catch (err) {
      setError("Failed to connect to game server");
      console.error("SignalR connection error:", err);
    }
  };

  // Fetch game by ID
  const fetchGame = async (gameId) => {
    setLoading(true);
    setError(null);

    try {
      const fetchedGame = await gameApi.getGame(gameId);
      setCurrentGame(fetchedGame);

      // Also fetch the game state
      const fetchedGameState = await gameApi.getGameState(gameId);
      setGameState(fetchedGameState);
    } catch (err) {
      setError("Failed to fetch game");
      console.error("Error fetching game:", err);
    } finally {
      setLoading(false);
    }
  };

  // Create a new game
  const createGame = async (gameName) => {
    setLoading(true);
    setError(null);

    try {
      const newGame = await gameApi.createGame({
        name: gameName,
        hostPlayerId: user.id,
      });
      setCurrentGame(newGame);
      return newGame;
    } catch (err) {
      setError("Failed to create game");
      console.error("Error creating game:", err);
      return null;
    } finally {
      setLoading(false);
    }
  };

  // Join an existing game
  const joinGame = async (gameId) => {
    setLoading(true);
    setError(null);

    try {
      await gameApi.joinGame(gameId, user.id);
      await fetchGame(gameId);
      return true;
    } catch (err) {
      setError("Failed to join game");
      console.error("Error joining game:", err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Start the game
  const startGame = async () => {
    if (!currentGame) return;

    setLoading(true);
    setError(null);

    try {
      await gameApi.startGame(currentGame.id);
      await fetchGame(currentGame.id);
      return true;
    } catch (err) {
      setError("Failed to start game");
      console.error("Error starting game:", err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Play a card
  const playCard = async (cardId, targetPlayerId = null) => {
    if (!currentGame || !gameState) return;

    setLoading(true);
    setError(null);

    try {
      await gameApi.playCard(currentGame.id, {
        playerId: user.id,
        cardId,
        targetPlayerId,
      });
      return true;
    } catch (err) {
      setError("Failed to play card");
      console.error("Error playing card:", err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Draw a card
  const drawCard = async () => {
    if (!currentGame || !gameState) return;

    setLoading(true);
    setError(null);

    try {
      await gameApi.drawCard(currentGame.id, {
        playerId: user.id,
      });
      return true;
    } catch (err) {
      setError("Failed to draw card");
      console.error("Error drawing card:", err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Play a special combo
  const playCombo = async (cardIds, targetPlayerId) => {
    if (!currentGame || !gameState) return;

    setLoading(true);
    setError(null);

    try {
      await gameApi.playCombo(currentGame.id, {
        playerId: user.id,
        cardIds,
        targetPlayerId,
      });
      return true;
    } catch (err) {
      setError("Failed to play combo");
      console.error("Error playing combo:", err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  // See the future
  const seeFuture = async () => {
    if (!currentGame || !gameState) return;

    setLoading(true);
    setError(null);

    try {
      const result = await gameApi.seeFuture(currentGame.id, user.id);
      setSeeFutureCards(result.topCards);
      return true;
    } catch (err) {
      setError("Failed to see the future");
      console.error("Error seeing the future:", err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Defuse an exploding kitten
  const defuseKitten = async (defuseCardId, position) => {
    if (!currentGame || !gameState) return;

    setLoading(true);
    setError(null);

    try {
      await gameApi.defuseKitten(currentGame.id, {
        playerId: user.id,
        defuseCardId,
        position,
      });
      return true;
    } catch (err) {
      setError("Failed to defuse kitten");
      console.error("Error defusing kitten:", err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Context value
  const value = {
    currentGame,
    gameState,
    loading,
    error,
    seeFutureCards,
    fetchGame,
    createGame,
    joinGame,
    startGame,
    playCard,
    drawCard,
    playCombo,
    seeFuture,
    defuseKitten,
  };

  return <GameContext.Provider value={value}>{children}</GameContext.Provider>;
};
