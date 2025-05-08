import PropTypes from "prop-types";
import React, { useEffect, useRef } from "react";
import "./ActivityLog.css";

const ActivityLog = ({ activities }) => {
  const logEndRef = useRef(null);

  // Auto-scroll to bottom when new activities are added
  useEffect(() => {
    logEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [activities]);

  const formatTime = (timestamp) => {
    const date = new Date(timestamp);
    return date.toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" });
  };

  return (
    <div className="activity-log">
      <h3 className="activity-log-title">Game Activity</h3>
      <div className="activity-log-content">
        {activities.length === 0 ? (
          <p className="no-activities">No activities yet</p>
        ) : (
          activities.map((activity, index) => (
            <div key={index} className="activity-item">
              <span className="activity-time">
                {formatTime(activity.timestamp)}
              </span>
              <span className="activity-text">{activity.text}</span>
            </div>
          ))
        )}
        <div ref={logEndRef} />
      </div>
    </div>
  );
};

ActivityLog.propTypes = {
  activities: PropTypes.arrayOf(
    PropTypes.shape({
      text: PropTypes.string.isRequired,
      timestamp: PropTypes.oneOfType([
        PropTypes.string,
        PropTypes.instanceOf(Date),
      ]).isRequired,
    })
  ).isRequired,
};

ActivityLog.defaultProps = {
  activities: [],
};

export default ActivityLog;
