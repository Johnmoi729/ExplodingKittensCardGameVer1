import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import GameBoard from "../components/game/GameBoard/GameBoard";
import { useGame } from "../contexts/GameContext";
import "./Game.css";

const Game = () => {
  const { gameId } = useParams();
  const navigate = useNavigate();
  const { currentGame, gameState, loading, error, fetchGame, startGame } =
    useGame();
  const [showWaitingRoom, setShowWaitingRoom] = useState(true);

  // Fetch the game data
  useEffect(() => {
    if (gameId) {
      fetchGame(gameId);
    }
  }, [gameId, fetchGame]);

  // Determine if we should show the waiting room or the game board
  useEffect(() => {
    if (currentGame) {
      setShowWaitingRoom(currentGame.status === "waiting");
    }
  }, [currentGame]);

  // Handle start game
  const handleStartGame = async () => {
    const success = await startGame();
    if (success) {
      setShowWaitingRoom(false);
    }
  };

  // Return to the main menu
  const handleReturnToMenu = () => {
    navigate("/");
  };

  if (loading) {
    return <div className="loading-container">Loading game...</div>;
  }

  if (error) {
    return (
      <div className="error-container">
        <h2>Error</h2>
        <p>{error}</p>
        <button onClick={handleReturnToMenu}>Return to Main Menu</button>
      </div>
    );
  }

  if (!currentGame) {
    return (
      <div className="error-container">
        <h2>Game Not Found</h2>
        <p>
          The game you're looking for doesn't exist or you don't have access to
          it.
        </p>
        <button onClick={handleReturnToMenu}>Return to Main Menu</button>
      </div>
    );
  }

  // Get current user
  const currentUser = JSON.parse(localStorage.getItem("user"));
  const isHost = currentUser?.id === currentGame.players[0];

  // Show waiting room if the game hasn't started yet
  if (showWaitingRoom) {
    return (
      <div className="waiting-room">
        <h2>Waiting Room</h2>
        <h3>{currentGame.name}</h3>

        <div className="game-details">
          <p>Status: {currentGame.status}</p>
          <p>Game ID: {currentGame.id}</p>
          <p>Share this ID with your friends to let them join!</p>
        </div>

        <div className="players-list">
          <h3>Players ({currentGame.players.length})</h3>
          <ul>
            {currentGame.players.map((playerId, index) => (
              <li key={playerId}>
                Player {index + 1}:{" "}
                {playerId === currentUser.id ? `${playerId} (You)` : playerId}
                {index === 0 && " (Host)"}
              </li>
            ))}
          </ul>
        </div>

        <div className="waiting-room-actions">
          {isHost && (
            <button
              className="start-game-button"
              onClick={handleStartGame}
              disabled={currentGame.players.length < 2}
            >
              Start Game
            </button>
          )}
          {!isHost && <p>Waiting for the host to start the game...</p>}
          <button className="return-button" onClick={handleReturnToMenu}>
            Return to Main Menu
          </button>
        </div>
      </div>
    );
  }

  // Show the game board if the game has started
  return <GameBoard gameId={gameId} />;
};

export default Game;
