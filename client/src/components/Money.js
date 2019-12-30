import React from "react";
import PropTypes from "prop-types";

const Money = ({ amount }) => {
  const rounded = Math.round(amount * 100) / 100;
  return <>&euro; {rounded}</>;
};

Money.propTypes = {
  amount: PropTypes.number.isRequired
};

export default Money;
