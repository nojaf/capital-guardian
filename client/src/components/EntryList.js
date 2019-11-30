import React from "react";
import { arrayOf, number, shape, string } from "prop-types";
import { Table } from "reactstrap";

const EntryList = ({ entries }) => {
  console.log(`entries`, entries);
  const rows = entries.map((e, i) => {
    return (
      <tr key={i}>
        <td>{e.name}</td>
        <td>&euro;{e.amount}</td>
      </tr>
    );
  });
  return (
    <Table striped className={"my-4"}>
      <thead>
        <tr>
          <th className={"w-75"}>Name</th>
          <th className={"w-25"}>Amount</th>
        </tr>
      </thead>
      <tbody>{rows}</tbody>
    </Table>
  );
};

EntryList.propTypes = {
  entries: arrayOf(
    shape({
      amount: number.isRequired,
      name: string.isRequired
    }).isRequired
  ).isRequired
};

export default EntryList;
