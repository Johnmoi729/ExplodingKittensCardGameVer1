import PropTypes from "prop-types";
import React from "react";
import "./PlayersList.css";

const PlayersList = ({
  players,
  currentPlayer,
  currentTurn,
  explodedPlayers,
  targetPlayer,
  onSelectTarget,
  getPlayerName,
  playerHands,
}) => {
  const handlePlayerClick = (playerId) => {
    if (playerId !== currentPlayer && !explodedPlayers.includes(playerId)) {
      onSelectTarget(playerId);
    }
  };

  const getPlayerClass = (playerId) => {
    let classes = "player-item";

    if (playerId === currentPlayer) {
      classes += " current-player";
    }

    if (playerId === currentTurn) {
      classes += " active-turn";
    }

    if (explodedPlayers.includes(playerId)) {
      classes += " exploded-player";
    }

    if (playerId === targetPlayer) {
      classes += " targeted-player";
    }

    return classes;
  };

  const getPlayerStatus = (playerId) => {
    if (explodedPlayers.includes(playerId)) {
      return <span className="player-status exploded">Exploded</span>;
    }

    if (playerId === currentTurn) {
      return <span className="player-status turn">Current Turn</span>;
    }

    return null;
  };

  const getCardCount = (playerId) => {
    if (!playerHands || !playerHands[playerId]) return 0;
    return playerHands[playerId].length;
  };

  return (
    <div className="players-list">
      <h3 className="players-list-title">Players</h3>
      <div className="players-list-content">
        {players.map((playerId, index) => (
          <div
            key={playerId}
            className={getPlayerClass(playerId)}
            onClick={() => handlePlayerClick(playerId)}
          >
            <div className="player-header">
              <span className="player-name">{getPlayerName(playerId)}</span>
              {getPlayerStatus(playerId)}
            </div>
            <div className="player-stats">
              <span className="player-cards">
                Cards: {getCardCount(playerId)}
              </span>
            </div>
            {playerId === targetPlayer && (
              <div className="target-indicator">TARGET</div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

PlayersList.propTypes = {
  players: PropTypes.arrayOf(PropTypes.string).isRequired,
  currentPlayer: PropTypes.string.isRequired,
  currentTurn: PropTypes.string,
  explodedPlayers: PropTypes.arrayOf(PropTypes.string),
  targetPlayer: PropTypes.string,
  onSelectTarget: PropTypes.func.isRequired,
  getPlayerName: PropTypes.func.isRequired,
  playerHands: PropTypes.object,
};

PlayersList.defaultProps = {
  explodedPlayers: [],
  playerHands: {},
};

export default PlayersList;
