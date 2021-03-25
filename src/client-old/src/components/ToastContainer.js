import { Toast, ToastBody, ToastHeader } from "reactstrap";
import React from "react";
import { useToasts } from "../bin/Main";

const ToastContainer = () => {
  const toasts = useToasts();
  const toastTiles = toasts.map(({ id, title, icon, body }) => {
    return (
      <Toast isOpen key={id}>
        <ToastHeader icon={icon}>{title}</ToastHeader>
        <ToastBody>{body}</ToastBody>
      </Toast>
    );
  });

  return <div id={"toast-container"}>{toastTiles}</div>;
};

export default ToastContainer;
