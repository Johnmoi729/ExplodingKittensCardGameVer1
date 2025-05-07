import PropTypes from "prop-types";
import React, { useState } from "react";
import Card from "../Card/Card";
import "./PlayerHand.css";

/**
 * PlayerHand component representing a player's hand of cards
 * @param {Object} props - Component props
 */
const PlayerHand = ({ cards, isCurrentPlayer, onPlayCard, onSelectCard }) => {
  const [selectedCards, setSelectedCards] = useState([]);

  // Handle card selection for combos
  const handleCardSelect = (card) => {
    if (!isCurrentPlayer) return;

    const isSelected = selectedCards.some((c) => c.id === card.id);

    if (isSelected) {
      setSelectedCards(selectedCards.filter((c) => c.id !== card.id));
    } else {
      // Only allow selecting cards with the same name for combos
      if (selectedCards.length === 0 || selectedCards[0].name === card.name) {
        setSelectedCards([...selectedCards, card]);
      }
    }

    if (onSelectCard) {
      onSelectCard(card, !isSelected);
    }
  };

  // Handle playing a card
  const handlePlayCard = (card) => {
    if (!isCurrentPlayer) return;

    if (onPlayCard) {
      onPlayCard(card);
    }
  };

  return (
    <div className="player-hand">
      <h3>Your Hand</h3>
      <div className="cards-container">
        {cards?.map((card) => (
          <Card
            key={card.id}
            card={card}
            isPlayable={isCurrentPlayer}
            onPlay={handlePlayCard}
            selected={selectedCards.some((c) => c.id === card.id)}
            onSelect={handleCardSelect}
          />
        ))}
        {cards?.length === 0 && <p>No cards in hand</p>}
      </div>
      {selectedCards.length > 0 && (
        <div className="selected-cards">
          <h4>Selected Cards</h4>
          <p>{selectedCards.length} cards selected</p>
          <button
            className="play-combo-button"
            disabled={
              !isCurrentPlayer ||
              (selectedCards.length !== 2 && selectedCards.length !== 3)
            }
            onClick={() => {
              if (onPlayCard) {
                onPlayCard(selectedCards);
              }
            }}
          >
            Play as Combo
          </button>
        </div>
      )}
    </div>
  );
};

PlayerHand.propTypes = {
  cards: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.string.isRequired,
      type: PropTypes.string.isRequired,
      name: PropTypes.string.isRequired,
      effect: PropTypes.string,
      imageUrl: PropTypes.string,
    })
  ),
  isCurrentPlayer: PropTypes.bool,
  onPlayCard: PropTypes.func,
  onSelectCard: PropTypes.func,
};

PlayerHand.defaultProps = {
  cards: [],
  isCurrentPlayer: false,
};

export default PlayerHand;
