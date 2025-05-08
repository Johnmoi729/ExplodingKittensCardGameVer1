import PropTypes from "prop-types";
import React from "react";
import "./LoadingSpinner.css";

const LoadingSpinner = ({ size, message }) => {
  return (
    <div className="loading-spinner-container">
      <div className={`loading-spinner size-${size}`}></div>
      {message && <p className="loading-message">{message}</p>}
    </div>
  );
};

LoadingSpinner.propTypes = {
  size: PropTypes.oneOf(["small", "medium", "large"]),
  message: PropTypes.string,
};

LoadingSpinner.defaultProps = {
  size: "medium",
  message: "Loading...",
};

export default LoadingSpinner;
