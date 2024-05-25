import React, { createContext, useContext, useMemo, useState } from "react";
import Snackbar from "@mui/material/Snackbar";

type AlertProp = {
  showAlert: (message: string) => void;
};

const AlertContext = createContext<AlertProp | null>(null);

export const useAlert = () => {
  const context = useContext(AlertContext);
  if (!context) {
    throw new Error("useAlert must be used within an AlertProvider");
  }
  return context as AlertProp;
};

export const AlertProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [open, setOpen] = useState(false);
  const [message, setMessage] = useState("");

  const showAlert = (newMessage: string) => {
    setMessage(newMessage);
    setOpen(true);
  };

  const hideAlert = () => {
    setOpen(false);
  };

  const value: AlertProp = useMemo(
    () => ({
      showAlert,
    }),
    []
  );

  return (
    <AlertContext.Provider value={value}>
      {children}
      <Snackbar
        open={open}
        autoHideDuration={4000}
        onClose={hideAlert}
        message={message}
        anchorOrigin={{ vertical: "top", horizontal: "center" }}
      />
    </AlertContext.Provider>
  );
};
