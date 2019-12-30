import React from "react";
import { useOverviewPerMonth } from "../bin/Main";
import { Table } from "reactstrap";
import { A } from "hookrouter";
import { Money } from "../components";

const OverviewPage = () => {
  const data = useOverviewPerMonth();
  const years = data.map(({ name: yearName, months, balance }) => {
    const rows = months.map(({ name, balance, month }) => {
      return (
        <tr key={name}>
          <td>
            <A href={`/${yearName}/${month}`}>{name}</A>
          </td>
          <td><Money amount={balance}/></td>
        </tr>
      );
    });

    return (
      <div key={yearName} className={"my-2"}>
        <h2>{yearName}</h2>
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
              <td className={"border-top border-dark"}>
                <Money amount={balance} />
              </td>
            </tr>
          </tbody>
        </Table>
      </div>
    );
  });
  return <div>{years}</div>;
};

export default OverviewPage;
