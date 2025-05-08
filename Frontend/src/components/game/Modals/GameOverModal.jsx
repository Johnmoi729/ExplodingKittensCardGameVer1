import PropTypes from "prop-types";
import React from "react";
import Modal from "../../common/Modal/Modal";
import "./Modals.css";

const GameOverModal = ({ isOpen, onClose, winner, isWinner }) => {
  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Game Over">
      <div className="game-over-content">
        <div className={`game-result ${isWinner ? "winner" : "loser"}`}>
          {isWinner ? (
            <>
              <h3>🎉 You Win! 🎉</h3>
              <p>Congratulations! You're the last player standing.</p>
            </>
          ) : (
            <>
              <h3>Game Over</h3>
              <p>{winner} is the winner!</p>
            </>
          )}
        </div>

        <div className="game-stats">
          <h4>Game Statistics:</h4>
          <p>Will be implemented in future updates.</p>
        </div>

        <div className="action-buttons">
          <button
            className="play-again-button"
            onClick={() => (window.location.href = "/")}
          >
            Back to Lobby
          </button>
          <button className="close-button" onClick={onClose}>
            View Final Board
          </button>
        </div>
      </div>
    </Modal>
  );
};

GameOverModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  winner: PropTypes.string,
  isWinner: PropTypes.bool,
};

GameOverModal.defaultProps = {
  isWinner: false,
};

export default GameOverModal;
