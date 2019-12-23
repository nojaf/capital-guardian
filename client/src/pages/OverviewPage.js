import React from "react";
import { useOverviewPerMonth } from "../bin/Main";
import { Table } from "reactstrap";

const OverviewPage = () => {
  const data = useOverviewPerMonth();
  const years = data.map(({ name, months, balance }) => {
    const rows = months.map(({ name, balance }) => {
      return (
        <tr key={name}>
          <td>{name}</td>
          <td>&euro;{balance}</td>
        </tr>
      );
    });

    return (
      <div key={name} className={"my-2"}>
        <h2>{name}</h2>
        <Table>
          <thead>
            <tr>
              <th>Month</th>
              <th>Balance</th>
            </tr>
          </thead>
          <tbody>
            {rows}
            <tr className={"font-weight-bold"}>
              <td className={"border-top border-dark"}>Total</td>
              <td className={"border-top border-dark"}>&euro;{balance}</td>
            </tr>
          </tbody>
        </Table>
      </div>
    );
  });
  return <div>{years}</div>;
};

export default OverviewPage;
