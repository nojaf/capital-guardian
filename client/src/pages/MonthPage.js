import React from "react";
import { number } from "prop-types";
import { EntryList, Loader, EntryForm, Money } from "../components";
import {
  useEntries,
  useIsLoading,
  useAddEntry,
  useBalance,
  useDefaultCreateDate,
  useCancelEvent,
  useCloneEvent
} from "../bin/Main";
import { Alert } from "reactstrap";

const MonthPage = ({ month, year }) => {
  const [income, expenses] = useEntries(month, year);
  const balance = useBalance(month, year);
  const isLoading = useIsLoading();
  const addEntry = useAddEntry();
  const balanceColor =
    balance < 0 ? "danger" : balance < 200 ? "warning" : "success";
  const defaultCreateDate = useDefaultCreateDate(month, year);
  const cancelEvent = useCancelEvent();
  const cloneEvent = useCloneEvent();

  const body = (
    <div>
      <div className={"my-4"}>
        <h2>Income</h2>
        <EntryList
          entries={income}
          onDelete={cancelEvent}
          onClone={cloneEvent}
        />
        <h2>Expenses</h2>
        <EntryList
          entries={expenses}
          onDelete={cancelEvent}
          onClone={cloneEvent}
        />
      </div>
      <Alert color={balanceColor}>
        Balance{" "}
        <span className={"float-right font-weight-bold"}>
          <Money amount={balance} />
        </span>
      </Alert>
      <hr />
      <div>
        <h2>Add entry</h2>
        <EntryForm onSubmit={addEntry} created={defaultCreateDate} />
      </div>
    </div>
  );

  return isLoading ? <Loader /> : body;
};

MonthPage.propTypes = {
  month: number.isRequired,
  year: number.isRequired
};

export default MonthPage;
