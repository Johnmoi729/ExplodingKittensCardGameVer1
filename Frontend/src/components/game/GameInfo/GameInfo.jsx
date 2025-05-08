import PropTypes from "prop-types";
import React from "react";
import "./GameInfo.css";

const GameInfo = ({
  gameName,
  gameStatus,
  turnNumber,
  currentPlayer,
  lastAction,
}) => {
  const getStatusClass = () => {
    switch (gameStatus) {
      case "waiting":
        return "status-waiting";
      case "in_progress":
        return "status-in-progress";
      case "completed":
        return "status-completed";
      default:
        return "";
    }
  };

  const getStatusText = () => {
    switch (gameStatus) {
      case "waiting":
        return "Waiting for players";
      case "in_progress":
        return "Game in progress";
      case "completed":
        return "Game completed";
      default:
        return gameStatus;
    }
  };

  return (
    <div className="game-info">
      <h2 className="game-name">{gameName}</h2>
      <div className="game-stats">
        <div className={`game-status ${getStatusClass()}`}>
          <span className="label">Status:</span>
          <span className="value">{getStatusText()}</span>
        </div>
        <div className="game-turn">
          <span className="label">Turn:</span>
          <span className="value">{turnNumber}</span>
        </div>
        <div className="current-player">
          <span className="label">Current Player:</span>
          <span className="value">{currentPlayer}</span>
        </div>
      </div>
      {lastAction && (
        <div className="last-action">
          <span className="label">Last Action:</span>
          <span className="value">{lastAction}</span>
        </div>
      )}
    </div>
  );
};

GameInfo.propTypes = {
  gameName: PropTypes.string.isRequired,
  gameStatus: PropTypes.string.isRequired,
  turnNumber: PropTypes.number.isRequired,
  currentPlayer: PropTypes.string.isRequired,
  lastAction: PropTypes.string,
};

export default GameInfo;
