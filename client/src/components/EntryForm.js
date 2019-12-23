import React from "react";
import { Form, FormGroup, Input, Button } from "reactstrap";
import useForm from "react-hook-form";
import PropTypes from "prop-types";
import * as yup from "yup";
import ToggleButton from "./ToggleButton";

const EntrySchema = yup.object().shape({
  name: yup.string().required(),
  amount: yup
    .number()
    .required()
    .positive(),
  isIncome: yup.bool().required()
});

const EntryForm = ({ onSubmit, created }) => {
  const { handleSubmit, register, errors, reset } = useForm({
    validationSchema: EntrySchema,
    defaultValues: {
      name: "",
      amount: 0.0,
      isIncome: false,
      created: created
    }
  });

  const onFormSubmit = values => {
    onSubmit(values);
    reset();
  };

  return (
    <div>
      <Form onSubmit={handleSubmit(onFormSubmit)} inline className={"my-3"}>
        <FormGroup>
          <Input
            type="text"
            autoComplete={"off"}
            name="name"
            innerRef={register}
            placeholder={"Name*"}
            className={"mr-2"}
          />
        </FormGroup>
        <FormGroup>
          <Input
            type="number"
            name="amount"
            innerRef={register}
            placeholder={"Amount*"}
            step=".01"
            className={"mr-2"}
          />
        </FormGroup>
        <FormGroup>
          <Input type={'date'}
                 name={'created'}
                 innerRef={register}
                 className={'mr-2'} />
        </FormGroup>
        <ToggleButton
          register={register}
          label={"Is income?"}
          name={"isIncome"}
        />
        <Button color={"primary"}>Add</Button>
      </Form>
      {errors.name && <p className={"text-danger"}>Name is required</p>}
      {errors.amount && <p className={"text-danger"}>Amount is required</p>}
    </div>
  );
};

EntryForm.propTypes = {
  onSubmit: PropTypes.func.isRequired,
  created: PropTypes.string.isRequired
};

export default EntryForm;
