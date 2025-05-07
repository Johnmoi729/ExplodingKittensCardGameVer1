// Card.jsx - Example of PropTypes and JSDoc usage
import React from "react";
import PropTypes from "prop-types";

/**
 * Card component representing a game card
 * @param {Object} props - Component props
 * @param {string} props.id - Unique identifier for the card
 * @param {string} props.type - Type of the card (e.g., "exploding_kitten", "defuse")
 * @param {string} props.name - Display name of the card
 * @param {string} props.imageUrl - URL to the card image
 * @param {Function} props.onPlay - Callback function when card is played
 */
function Card({ id, type, name, imageUrl, onPlay }) {
  return (
    <div className="card" onClick={() => onPlay(id)}>
      <img src={imageUrl} alt={name} />
      <h3>{name}</h3>
    </div>
  );
}

Card.propTypes = {
  id: PropTypes.string.isRequired,
  type: PropTypes.string.isRequired,
  name: PropTypes.string.isRequired,
  imageUrl: PropTypes.string.isRequired,
  onPlay: PropTypes.func.isRequired
};

export default Card;
