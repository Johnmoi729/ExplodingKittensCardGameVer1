import PropTypes from "prop-types";
import React, { useState } from "react";
import { useGame } from "../../../contexts/GameContext";
import Modal from "../../common/Modal/Modal";
import Card from "../Card/Card";
import "./Modals.css";

const DefuseKittenModal = ({ isOpen, onClose, deckSize, defuseCards }) => {
  const [selectedDefuseCard, setSelectedDefuseCard] = useState(null);
  const [selectedPosition, setSelectedPosition] = useState(null);
  const { defuseKitten } = useGame();

  const handleDefuseCardSelect = (card) => {
    setSelectedDefuseCard(card);
  };

  const handlePositionSelect = (position) => {
    setSelectedPosition(position);
  };

  const handleDefuse = async () => {
    if (!selectedDefuseCard || selectedPosition === null) return;

    const success = await defuseKitten(selectedDefuseCard.id, selectedPosition);
    if (success) {
      onClose();
      setSelectedDefuseCard(null);
      setSelectedPosition(null);
    }
  };

  // Generate position buttons
  const generatePositionButtons = () => {
    const buttons = [];

    // Top of the deck
    buttons.push(
      <button
        key="top"
        className={`position-button ${
          selectedPosition === 0 ? "selected" : ""
        }`}
        onClick={() => handlePositionSelect(0)}
      >
        Top
      </button>
    );

    // Bottom of the deck
    buttons.push(
      <button
        key="bottom"
        className={`position-button ${
          selectedPosition === deckSize ? "selected" : ""
        }`}
        onClick={() => handlePositionSelect(deckSize)}
      >
        Bottom
      </button>
    );

    // Random position
    buttons.push(
      <button
        key="random"
        className={`position-button ${
          selectedPosition === -1 ? "selected" : ""
        }`}
        onClick={() => handlePositionSelect(-1)}
      >
        Random
      </button>
    );

    return buttons;
  };

  return (
    <Modal isOpen={isOpen} onClose={null} title="You Drew an Exploding Kitten!">
      <div className="defuse-content">
        <p className="explosion-warning">
          💥 BOOM! You drew an Exploding Kitten! 💥
        </p>

        {defuseCards.length > 0 ? (
          <>
            <p className="defuse-instructions">
              Select a Defuse card to save yourself:
            </p>

            <div className="defuse-cards">
              {defuseCards.map((card) => (
                <div
                  key={card.id}
                  className={`defuse-card ${
                    selectedDefuseCard?.id === card.id ? "selected" : ""
                  }`}
                  onClick={() => handleDefuseCardSelect(card)}
                >
                  <Card card={card} />
                </div>
              ))}
            </div>

            {selectedDefuseCard && (
              <>
                <p className="position-instructions">
                  Where do you want to put the Exploding Kitten?
                </p>

                <div className="position-buttons">
                  {generatePositionButtons()}
                </div>

                <button
                  className="defuse-button"
                  disabled={selectedPosition === null}
                  onClick={handleDefuse}
                >
                  Defuse!
                </button>
              </>
            )}
          </>
        ) : (
          <div className="game-over-message">
            <p>You don't have any Defuse cards!</p>
            <p>Game over for you.</p>
          </div>
        )}
      </div>
    </Modal>
  );
};

DefuseKittenModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  deckSize: PropTypes.number.isRequired,
  defuseCards: PropTypes.array,
};

DefuseKittenModal.defaultProps = {
  defuseCards: [],
};

export default DefuseKittenModal;
