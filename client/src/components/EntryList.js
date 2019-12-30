import React from "react";
import PropTypes from "prop-types";
import { Table, Button } from "reactstrap";
import {Money} from "./index";

const EntryList = ({ entries, onDelete, onClone }) => {
  const rows = entries.map((e, i) => {
    return (
      <tr key={i}>
        <td>{e.name}</td>
        <td>
          <Button
            color={"danger"}
            onClick={() => onDelete(e.id)}
            title={"Nullify event"}
          >
            <i className={"far fa-trash-alt"} />
          </Button>
          <Button
            color={"info"}
            className={"ml-2"}
            onClick={() => onClone(e.id)}
            title={"Clone to current month"}
          >
            <i className="far fa-clone"></i>
          </Button>
        </td>
        <td><Money amount={e.amount}/></td>
      </tr>
    );
  });
  return (
    <Table striped className={"my-4"}>
      <thead>
        <tr>
          <th className={"w-50"}>Name</th>
          <th className={"w-25"}>Actions</th>
          <th className={"w-25"}>Amount</th>
        </tr>
      </thead>
      <tbody>{rows}</tbody>
    </Table>
  );
};

EntryList.propTypes = {
  entries: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.any.isRequired,
      amount: PropTypes.number.isRequired,
      name: PropTypes.string.isRequired
    }).isRequired
  ).isRequired,
  onDelete: PropTypes.func.isRequired,
  onClone: PropTypes.func.isRequired
};

export default EntryList;
