import React from "react";
import Switch from "react-switch";
import PropTypes from "prop-types";

const ToggleButton = ({ register, name, label }) => {
  const [value, setValue] = React.useState(false);
  return (
    <div className={'d-flex align-items-center'}>
      <input type={"hidden"} name={name} value={value} ref={register} />
      <div className={"mr-2"}>
        {label}
      </div>
      <Switch
        onChange={setValue}
        checked={value}
        onColor={"#F3E5AB"}
        offColor={"#223030"}
      />
    </div>
  );
};

ToggleButton.propTypes = {
  register: PropTypes.func.isRequired,
  name: PropTypes.string.isRequired,
  label: PropTypes.string
};

export default ToggleButton;
