import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import React, { createContext, useContext, useEffect, useState } from "react";
import { gameApi } from "../api/gameApi";
import { useAuth } from "./AuthContext";

// Create context
const GameContext = createContext();

export const useGame = () => useContext(GameContext);

export const GameProvider = ({ children }) => {
  // State variables
  const { user } = useAuth();
  const [currentGame, setCurrentGame] = useState(null);
  const [gameState, setGameState] = useState(null);
  const [playerStatus, setPlayerStatus] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [connection, setConnection] = useState(null);
  const [seeFutureCards, setSeeFutureCards] = useState([]);
  const [isExploding, setIsExploding] = useState(false);
  const [winner, setWinner] = useState(null);
  const [gameActivity, setGameActivity] = useState([]);
  const [selectedCards, setSelectedCards] = useState([]);
  const [targetPlayer, setTargetPlayer] = useState(null);

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

  // Regularly poll for game status
  useEffect(() => {
    let statusInterval;

    if (currentGame && gameState && user && !isExploding) {
      // Poll for status every 3 seconds
      statusInterval = setInterval(() => {
        getPlayerStatus();
      }, 3000);
    }

    return () => {
      if (statusInterval) {
        clearInterval(statusInterval);
      }
    };
  }, [currentGame, gameState, user, isExploding]);

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

        // Update activity log
        if (updatedGameState.lastAction) {
          addActivity(updatedGameState.lastAction);
        }

        // Check for winner
        if (updatedGameState.gameStatus === "completed") {
          const winnerName = getWinnerName(updatedGameState);
          setWinner(winnerName);
        }
      });

      newConnection.on("CardPlayed", (data) => {
        addActivity(
          `Player ${getPlayerName(data.playerId)} played ${data.cardName}`
        );
      });

      newConnection.on("CardDrawn", (data) => {
        addActivity(`Player ${getPlayerName(data.playerId)} drew a card`);
      });

      newConnection.on("PlayerExploded", (data) => {
        addActivity(`Player ${getPlayerName(data.playerId)} exploded!`);
        if (data.playerId === user.id) {
          setIsExploding(true);
        }
      });

      newConnection.on("PlayerTurn", (data) => {
        addActivity(`It's player ${getPlayerName(data.playerId)}'s turn`);
      });

      newConnection.on("GameEnded", (data) => {
        addActivity(`Game ended. Player ${getPlayerName(data.winnerId)} wins!`);
        setWinner(getPlayerName(data.winnerId));
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

  // Get player's current status
  const getPlayerStatus = async () => {
    if (!currentGame || !user) return;

    try {
      const status = await gameApi.getPlayerStatus(currentGame.id, user.id);
      setPlayerStatus(status);

      // Update game status if it's completed
      if (
        status.gameStatus === "completed" &&
        currentGame.status !== "completed"
      ) {
        setCurrentGame({
          ...currentGame,
          status: "completed",
        });
      }
    } catch (err) {
      console.error("Error fetching player status:", err);
    }
  };

  // Add activity to log
  const addActivity = (activity) => {
    setGameActivity((prev) => [
      ...prev,
      { text: activity, timestamp: new Date() },
    ]);
  };

  // Get player name
  const getPlayerName = (playerId) => {
    if (playerId === user.id) return "You";
    return playerId;
  };

  // Get winner name
  const getWinnerName = (gameState) => {
    const explodedPlayers = gameState.explodedPlayers || [];
    const allPlayers = currentGame?.players || [];

    if (explodedPlayers.length === allPlayers.length - 1) {
      const winnerId = allPlayers.find((id) => !explodedPlayers.includes(id));
      return getPlayerName(winnerId);
    }

    return null;
  };

  // Select a card
  const selectCard = (card) => {
    if (isSelected(card.id)) {
      setSelectedCards((prev) => prev.filter((c) => c.id !== card.id));
    } else {
      // For combos, only select cards with the same name
      if (selectedCards.length > 0 && selectedCards[0].name !== card.name) {
        return;
      }

      setSelectedCards((prev) => [...prev, card]);
    }
  };

  // Check if a card is selected
  const isSelected = (cardId) => {
    return selectedCards.some((c) => c.id === cardId);
  };

  // Select a target player
  const selectTarget = (playerId) => {
    setTargetPlayer(playerId);
  };

  // Clear selections
  const clearSelections = () => {
    setSelectedCards([]);
    setTargetPlayer(null);
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

      // Get player status
      await getPlayerStatus();
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
      clearSelections();
      return true;
    } catch (err) {
      setError("Failed to play card");
      console.error("Error playing card:", err);
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Play selected cards
  const playSelectedCards = async () => {
    if (selectedCards.length === 0) return;

    if (selectedCards.length === 1) {
      // Single card
      return playCard(selectedCards[0].id, targetPlayer);
    } else {
      // Combo
      return playCombo(
        selectedCards.map((c) => c.id),
        targetPlayer
      );
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
      clearSelections();
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
      setIsExploding(false);
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
    playerStatus,
    loading,
    error,
    seeFutureCards,
    isExploding,
    winner,
    gameActivity,
    selectedCards,
    targetPlayer,
    fetchGame,
    createGame,
    joinGame,
    startGame,
    playCard,
    drawCard,
    playCombo,
    seeFuture,
    defuseKitten,
    selectCard,
    isSelected,
    selectTarget,
    clearSelections,
    playSelectedCards,
    getPlayerName,
  };

  return <GameContext.Provider value={value}>{children}</GameContext.Provider>;
};
