import PropTypes from "prop-types";
import React, { useEffect, useState } from "react";
import { useGame } from "../../../contexts/GameContext";
import Card from "../Card/Card";
import PlayerHand from "../PlayerHand/PlayerHand";
import "./GameBoard.css";

/**
 * GameBoard component representing the main game board
 * @param {Object} props - Component props
 */
const GameBoard = ({ gameId }) => {
  const {
    currentGame,
    gameState,
    loading,
    error,
    fetchGame,
    playCard,
    drawCard,
    playCombo,
    seeFuture,
    seeFutureCards,
  } = useGame();

  const [selectedTargetPlayer, setSelectedTargetPlayer] = useState(null);
  const [showSeeFutureCards, setShowSeeFutureCards] = useState(false);

  // Load game data on component mount
  useEffect(() => {
    if (gameId) {
      fetchGame(gameId);
    }
  }, [gameId, fetchGame]);

  // Reset selected target player when turns change
  useEffect(() => {
    if (gameState) {
      setSelectedTargetPlayer(null);
    }
  }, [gameState?.currentPlayerId]);

  if (loading) {
    return <div className="loading">Loading game...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  if (!currentGame || !gameState) {
    return <div className="error">Game not found</div>;
  }

  // Get current player data
  const currentUser = JSON.parse(localStorage.getItem("user"));
  const isCurrentPlayersTurn = gameState.currentPlayerId === currentUser.id;
  const currentPlayerHand = gameState.playerHands?.[currentUser.id] || [];

  // Get other players data
  const otherPlayers = currentGame.players.filter(
    (id) => id !== currentUser.id
  );

  // Handle playing a card
  const handlePlayCard = async (card) => {
    if (!isCurrentPlayersTurn) return;

    // Check if the card needs a target player
    if (card.type === "favor" || card.type === "cat_card") {
      if (!selectedTargetPlayer) {
        alert("Please select a target player");
        return;
      }

      await playCard(card.id, selectedTargetPlayer);
    } else {
      await playCard(card.id);
    }
  };

  // Handle playing a combo
  const handlePlayCombo = async (cards) => {
    if (!isCurrentPlayersTurn || !selectedTargetPlayer) {
      alert("Please select a target player");
      return;
    }

    await playCombo(
      cards.map((c) => c.id),
      selectedTargetPlayer
    );
  };

  // Handle See Future card effect
  const handleSeeFuture = async () => {
    if (!isCurrentPlayersTurn) return;

    await seeFuture();
    setShowSeeFutureCards(true);
  };

  // Handle drawing a card
  const handleDrawCard = async () => {
    if (!isCurrentPlayersTurn) return;

    await drawCard();
  };

  return (
    <div className="game-board">
      <div className="game-info">
        <h2>{currentGame.name}</h2>
        <div className="game-status">
          <p>Status: {currentGame.status}</p>
          <p>Turn: {gameState.turnNumber}</p>
          <p>
            Current Player:{" "}
            {gameState.currentPlayerId === currentUser.id
              ? "You"
              : gameState.currentPlayerId}
          </p>
        </div>
      </div>

      <div className="game-area">
        <div className="deck-area">
          <div className="deck">
            <div className="draw-pile">
              <h3>Draw Pile ({gameState.drawPileCount})</h3>
              <div className="card card-back" onClick={handleDrawCard} />
              {isCurrentPlayersTurn && (
                <button className="draw-button" onClick={handleDrawCard}>
                  Draw a Card
                </button>
              )}
            </div>

            <div className="discard-pile">
              <h3>Discard Pile ({gameState.discardPile?.length || 0})</h3>
              {gameState.discardPile?.length > 0 ? (
                <Card
                  card={gameState.discardPile[gameState.discardPile.length - 1]}
                />
              ) : (
                <div className="card-placeholder"></div>
              )}
            </div>
          </div>

          <div className="game-actions">
            <h3>Actions</h3>
            <p>{gameState.lastAction}</p>
          </div>
        </div>

        <div className="players-area">
          <h3>Other Players</h3>
          <div className="other-players">
            {otherPlayers.map((playerId) => {
              const isExploded = gameState.explodedPlayers?.includes(playerId);
              const handSize = gameState.playerHands?.[playerId]?.length || 0;

              return (
                <div
                  key={playerId}
                  className={`player ${isExploded ? "exploded" : ""} ${
                    selectedTargetPlayer === playerId ? "selected" : ""
                  }`}
                  onClick={() => setSelectedTargetPlayer(playerId)}
                >
                  <p>Player: {playerId}</p>
                  <p>Cards: {handSize}</p>
                  {isExploded && <p className="exploded-label">EXPLODED</p>}
                  {selectedTargetPlayer === playerId && (
                    <div className="target-indicator">TARGET</div>
                  )}
                </div>
              );
            })}
          </div>
        </div>
      </div>

      <PlayerHand
        cards={currentPlayerHand}
        isCurrentPlayer={isCurrentPlayersTurn}
        onPlayCard={handlePlayCard}
        onPlayCombo={handlePlayCombo}
      />

      {/* See Future Modal */}
      {showSeeFutureCards && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h3>Top 3 Cards</h3>
            <div className="future-cards">
              {seeFutureCards.map((card, index) => (
                <div key={index} className="future-card">
                  <Card card={card} />
                  <p>Position: {index + 1}</p>
                </div>
              ))}
            </div>
            <button onClick={() => setShowSeeFutureCards(false)}>Close</button>
          </div>
        </div>
      )}
    </div>
  );
};

GameBoard.propTypes = {
  gameId: PropTypes.string.isRequired,
};

export default GameBoard;
