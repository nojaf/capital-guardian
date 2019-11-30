import React from "react";
import { Container } from "reactstrap";
import { EntryList, Header, Loader, EntryForm } from "../components";
import { useEntries, useIsLoading, useAddEntry } from "../bin/Main";

const HomePage = () => {
  const [income, expenses] = useEntries();
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

export default HomePage;
