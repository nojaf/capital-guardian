import React from "react";
import { Form, FormGroup, Input, Button, Label } from "reactstrap";
import { useForm } from "react-hook-form";
import * as yup from "yup";
import { useSpreadOverMonths, useFirstOfCurrentMonthDate } from "../bin/Main";
import { navigate } from "hookrouter";

const SpreadSchema = yup.object().shape({
  name: yup.string().required(),
  amount: yup.number().required().positive(),
  pieces: yup.number().required().positive().moreThan(1),
  start: yup
    .string()
    .required()
    .matches(/\d{4}-\d{2}-\d{2}/),
});

const SpreadPage = () => {
  const onSubmit = useSpreadOverMonths();
  const start = useFirstOfCurrentMonthDate();
  console.log(`today`, start);
  const { handleSubmit, register, errors, reset } = useForm({
    validationSchema: SpreadSchema,
    defaultValues: {
      name: "",
      amount: 0.0,
      pieces: 2,
      start,
    },
  });

  const onFormSubmit = (values) => {
    onSubmit(values);
    const date = values.start.split("-");
    const url = `/${date[0]}/${date[1]}`;
    reset();
    navigate(url);
  };

  return (
    <div className={"mt-4 offset-xl-8 col-xl-4"}>
      <Form onSubmit={handleSubmit(onFormSubmit)}>
        <FormGroup>
          <Label>Name</Label>
          <Input
            type="text"
            autoComplete={"off"}
            name="name"
            innerRef={register}
            placeholder={"Name*"}
          />
        </FormGroup>
        <FormGroup>
          <Label>Amount</Label>
          <Input
            type="number"
            name="amount"
            innerRef={register}
            placeholder={"Amount*"}
            step=".01"
          />
        </FormGroup>
        <FormGroup>
          <Label># of pieces</Label>
          <Input
            type="number"
            name="pieces"
            innerRef={register}
            placeholder={"Pieces*"}
            step="1"
          />
        </FormGroup>
        <FormGroup>
          <Label>Start date</Label>
          <Input type={"date"} name={"start"} innerRef={register} />
        </FormGroup>
        <div className={"text-right"}>
          <Button color={"primary"}>Save</Button>
        </div>
      </Form>
      {errors.name && <p className={"text-danger"}>Name is required</p>}
      {errors.amount && <p className={"text-danger"}>Amount is required</p>}
      {errors.pieces && <p className={"text-danger"}>Pieces is required</p>}
    </div>
  );
};

export default SpreadPage;
