import PropTypes from "prop-types";
import React from "react";
import Modal from "../../common/Modal/Modal";
import Card from "../Card/Card";
import "./Modals.css";

const SeeFutureModal = ({ isOpen, onClose, cards }) => {
  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Top 3 Cards">
      <div className="see-future-content">
        <p className="modal-intro">
          These are the next cards in the draw pile:
        </p>

        <div className="cards-container">
          {cards.length > 0 ? (
            cards.map((card, index) => (
              <div key={index} className="future-card">
                <Card card={card} />
                <p className="card-position">Position: {index + 1}</p>
              </div>
            ))
          ) : (
            <p className="no-cards">No cards to show</p>
          )}
        </div>

        <div className="modal-instructions">
          <p>Remember these cards. They will be drawn in this order.</p>
        </div>

        <button className="modal-button" onClick={onClose}>
          Close
        </button>
      </div>
    </Modal>
  );
};

SeeFutureModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  cards: PropTypes.array,
};

SeeFutureModal.defaultProps = {
  cards: [],
};

export default SeeFutureModal;
