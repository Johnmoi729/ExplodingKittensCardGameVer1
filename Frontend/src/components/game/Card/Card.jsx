import PropTypes from "prop-types";
import React, { memo } from "react";
import "./Card.css";

/**
 * Card component representing a game card (memoized for performance)
 * @param {Object} props - Component props
 */
const Card = memo(({ card, isPlayable, onPlay, selected, onSelect }) => {
  const handleClick = () => {
    if (isPlayable && onPlay) {
      onPlay(card);
    } else if (onSelect) {
      onSelect(card);
    }
  };

  // Determine card class based on type
  const getCardClassName = () => {
    const baseClass = "card";
    const typeClass = card ? `card-${card.type}` : "";
    const selectClass = selected ? "card-selected" : "";
    const playableClass = isPlayable ? "card-playable" : "";

    return `${baseClass} ${typeClass} ${selectClass} ${playableClass}`;
  };

  if (!card) {
    return <div className="card card-back" />;
  }

  return (
    <div className={getCardClassName()} onClick={handleClick}>
      <div className="card-inner">
        <div className="card-front">
          <h3 className="card-name">{card.name}</h3>
          <div className="card-image-container">
            <img
              src={card.imageUrl || `/images/${card.type}.png`}
              alt={card.name}
              className="card-image"
            />
          </div>
          <p className="card-effect">{card.effect}</p>
        </div>
      </div>
    </div>
  );
});

Card.propTypes = {
  card: PropTypes.shape({
    id: PropTypes.string.isRequired,
    type: PropTypes.string.isRequired,
    name: PropTypes.string.isRequired,
    effect: PropTypes.string,
    imageUrl: PropTypes.string,
  }),
  isPlayable: PropTypes.bool,
  onPlay: PropTypes.func,
  selected: PropTypes.bool,
  onSelect: PropTypes.func,
};

Card.defaultProps = {
  isPlayable: false,
  selected: false,
};

// Display name for React DevTools
Card.displayName = "Card";

export default Card;
