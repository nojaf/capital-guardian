import React from "react";
import { number } from "prop-types";
import { Container } from "reactstrap";
import { EntryList, Header, Loader, EntryForm } from "../components";
import { useEntries, useIsLoading, useAddEntry } from "../bin/Main";

const MonthPage = ({ month, year}) => {
  const [income, expenses] = useEntries(month, year);
  const isLoading = useIsLoading();
  const addEntry = useAddEntry();
  const body = (
    <div>
      <Header />
      <Container className={"my-4"}>
        <h2>Income</h2>
        <EntryList entries={income} />
        <h2>Expenses</h2>
        <EntryList entries={expenses} />
      </Container>
      <hr />
      <Container>
        <h2>Add entry</h2>
        <EntryForm onSubmit={addEntry} />
      </Container>
    </div>
  );

  return isLoading ? <Loader /> : body;
};

MonthPage.propTypes = {
    month: number.isRequired,
    year: number.isRequired
}

export default MonthPage;
