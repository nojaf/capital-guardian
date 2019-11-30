import React from "react";
import Switch from "react-switch";
import PropTypes from "prop-types";

const ToggleButton = ({ register, name, label }) => {
  const [value, setValue] = React.useState(false);
  return (
    <>
      <input type={"hidden"} name={name} value={value} ref={register} />
      <label className={"mr-2"}>
        <span className={"mr-2"}>{label}</span>
        <Switch
          onChange={setValue}
          checked={value}
          onColor={"#BBDFDB"}
          offColor={"#223030"}
        />
      </label>
    </>
  );
};

ToggleButton.propTypes = {
  register: PropTypes.func.isRequired,
  name: PropTypes.string.isRequired,
  label: PropTypes.string
};

export default ToggleButton;
