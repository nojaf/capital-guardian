import React from "react";
import { useXsOrSm } from "../hooks/breakpoints";
import { MonthPage } from "./index";
import { useAddEntry, useDefaultCreateDate } from "../bin/Main";
import { EntryForm } from "../components";

const currentYear = new Date().getFullYear();
const currentMonth = new Date().getMonth() + 1;

const HomePage = () => {
  const small = useXsOrSm();
  const addEntry = useAddEntry();
  const defaultCreateDate = useDefaultCreateDate(currentMonth, currentYear);

  return small ? (
    <EntryForm onSubmit={addEntry} created={defaultCreateDate} />
  ) : (
    <MonthPage year={currentYear} month={currentMonth} />
  );
};

export default HomePage;
