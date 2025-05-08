import PropTypes from "prop-types";
import React, { useEffect, useState } from "react";
import { useGame } from "../../../contexts/GameContext";
import ActivityLog from "../ActivityLog/ActivityLog";
import Card from "../Card/Card";
import GameInfo from "../GameInfo/GameInfo";
import DefuseKittenModal from "../Modals/DefuseKittenModal";
import GameOverModal from "../Modals/GameOverModal";
import SeeFutureModal from "../Modals/SeeFutureModal";
import PlayerHand from "../PlayerHand/PlayerHand";
import PlayersList from "../PlayersList/PlayersList";
import "./GameBoard.css";

const GameBoard = ({ gameId }) => {
  const {
    currentGame,
    gameState,
    playerStatus,
    loading,
    error,
    fetchGame,
    playCard,
    drawCard,
    seeFutureCards,
    isExploding,
    winner,
    gameActivity,
    selectedCards,
    targetPlayer,
    selectCard,
    isSelected,
    selectTarget,
    clearSelections,
    playSelectedCards,
    getPlayerName,
  } = useGame();

  const [showSeeFutureModal, setShowSeeFutureModal] = useState(false);
  const [showDefuseModal, setShowDefuseModal] = useState(false);
  const [showGameOverModal, setShowGameOverModal] = useState(false);

  // Load game data on component mount
  useEffect(() => {
    if (gameId) {
      fetchGame(gameId);
    }
  }, [gameId, fetchGame]);

  // Show defuse modal when player is exploding
  useEffect(() => {
    if (isExploding) {
      setShowDefuseModal(true);
    } else {
      setShowDefuseModal(false);
    }
  }, [isExploding]);

  // Show game over modal when there's a winner
  useEffect(() => {
    if (winner) {
      setShowGameOverModal(true);
    }
  }, [winner]);

  if (loading && !gameState) {
    return (
      <div className="loading-container">
        <div className="loading-spinner"></div>
        <p>Loading game...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="error-container">
        <h2>Error</h2>
        <p>{error}</p>
        <button onClick={() => (window.location.href = "/")}>
          Return to Main Menu
        </button>
      </div>
    );
  }

  if (!currentGame || !gameState) {
    return (
      <div className="error-container">
        <h2>Game Not Found</h2>
        <p>
          The game you're looking for doesn't exist or you don't have access to
          it.
        </p>
        <button onClick={() => (window.location.href = "/")}>
          Return to Main Menu
        </button>
      </div>
    );
  }

  // Get current player data
  const currentUser = JSON.parse(localStorage.getItem("user"));
  const isCurrentPlayersTurn = gameState.currentPlayerId === currentUser.id;
  const isGameCompleted = currentGame.status === "completed";
  const currentPlayerHand = gameState.playerHands?.[currentUser.id] || [];

  // Get other players data
  const otherPlayers = currentGame.players.filter(
    (id) => id !== currentUser.id
  );

  // Handle playing a card
  const handlePlayCard = (card) => {
    if (!isCurrentPlayersTurn) return;

    // For cards that need targeting
    if (
      card.type === "favor" ||
      (selectedCards.length > 0 && selectedCards[0].type === "cat_card")
    ) {
      selectCard(card);
    } else if (card.type === "see_future") {
      // Show the see future modal after playing the card
      playCard(card.id).then(() => {
        setShowSeeFutureModal(true);
      });
    } else {
      playCard(card.id);
    }
  };

  // Handle drawing a card
  const handleDrawCard = () => {
    if (!isCurrentPlayersTurn || isGameCompleted) return;

    drawCard();
  };

  // Get card playability status
  const isCardPlayable = (card) => {
    if (!isCurrentPlayersTurn || isGameCompleted) return false;

    // Check for cards that require target selection
    if ((card.type === "favor" || card.type === "cat_card") && !targetPlayer) {
      return false;
    }

    return true;
  };

  return (
    <div className="game-board">
      <div className="game-header">
        <GameInfo
          gameName={currentGame.name}
          gameStatus={currentGame.status}
          turnNumber={gameState.turnNumber}
          currentPlayer={getPlayerName(gameState.currentPlayerId)}
          lastAction={gameState.lastAction}
        />
      </div>

      <div className="game-content">
        <div className="left-panel">
          <ActivityLog activities={gameActivity} />
        </div>

        <div className="center-panel">
          <div className="deck-area">
            <div className="draw-pile">
              <h3>Draw Pile ({gameState.drawPileCount})</h3>
              <div
                className={`card card-back ${
                  isCurrentPlayersTurn && !isGameCompleted
                    ? "card-playable"
                    : ""
                }`}
                onClick={handleDrawCard}
              />
              {isCurrentPlayersTurn && !isGameCompleted && (
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

          {playerStatus && (
            <div className="game-status">
              <p>
                {playerStatus.isPlayerTurn
                  ? "It's your turn!"
                  : `Your turn in ${playerStatus.turnsUntilPlayerTurn} turns`}
              </p>
              {playerStatus.isPlayerExploded && (
                <p className="exploded-status">
                  You have exploded! Game over for you.
                </p>
              )}
              <p>Players remaining: {playerStatus.remainingPlayers}</p>
              <p>Cards in deck: {playerStatus.remainingCards}</p>
            </div>
          )}

          {selectedCards.length > 0 && (
            <div className="selected-cards-info">
              <h3>Selected Cards</h3>
              <div className="selected-cards-container">
                {selectedCards.map((card) => (
                  <div key={card.id} className="selected-card">
                    <Card card={card} isPlayable={false} />
                  </div>
                ))}
              </div>
              {targetPlayer ? (
                <div className="target-info">
                  <p>Target: {getPlayerName(targetPlayer)}</p>
                  <button className="play-button" onClick={playSelectedCards}>
                    Play {selectedCards.length > 1 ? "Combo" : "Card"}
                  </button>
                  <button className="cancel-button" onClick={clearSelections}>
                    Cancel
                  </button>
                </div>
              ) : (
                <p>Select a target player</p>
              )}
            </div>
          )}
        </div>

        <div className="right-panel">
          <PlayersList
            players={currentGame.players}
            currentPlayer={currentUser.id}
            currentTurn={gameState.currentPlayerId}
            explodedPlayers={gameState.explodedPlayers || []}
            targetPlayer={targetPlayer}
            onSelectTarget={selectTarget}
            getPlayerName={getPlayerName}
            playerHands={gameState.playerHands}
          />
        </div>
      </div>

      <div className="player-hand-container">
        <PlayerHand
          cards={currentPlayerHand}
          isCurrentPlayer={isCurrentPlayersTurn}
          onPlayCard={handlePlayCard}
          isCardPlayable={isCardPlayable}
          selectedCards={selectedCards.map((c) => c.id)}
          onSelectCard={selectCard}
          isGameCompleted={isGameCompleted}
        />
      </div>

      {/* Modals */}
      <SeeFutureModal
        isOpen={showSeeFutureModal}
        onClose={() => setShowSeeFutureModal(false)}
        cards={seeFutureCards}
      />

      <DefuseKittenModal
        isOpen={showDefuseModal}
        onClose={() => setShowDefuseModal(false)}
        deckSize={gameState.drawPileCount}
        defuseCards={currentPlayerHand.filter((c) => c.type === "defuse")}
      />

      <GameOverModal
        isOpen={showGameOverModal}
        onClose={() => setShowGameOverModal(false)}
        winner={winner}
        isWinner={winner === "You"}
      />
    </div>
  );
};

GameBoard.propTypes = {
  gameId: PropTypes.string.isRequired,
};

export default GameBoard;
