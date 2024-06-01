import React, { createContext, useContext, useMemo, useState } from "react";
import Snackbar from "@mui/material/Snackbar";
import { Alert } from "@mui/material";

type AlertSeverity = "error" | "success" | "info" | "warning";

type AlertProp = {
  showAlert: (message: string, severity: AlertSeverity) => void;
  hideAlert: () => void;
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
  const [severity, setSeverity] = useState<AlertSeverity>("info");

  const showAlert = (newMessage: string, severity: AlertSeverity = "info") => {
    setMessage(newMessage);
    setSeverity(severity);
    setOpen(true);
  };

  const hideAlert = () => {
    setOpen(false);
  };

  const value: AlertProp = useMemo(
    () => ({
      showAlert,
      hideAlert,
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
        anchorOrigin={{ vertical: "top", horizontal: "center" }}
      >
        <Alert
          onClose={hideAlert}
          severity={severity}
          variant="filled"
          sx={{ width: "100%" }}
        >
          {message}
        </Alert>
      </Snackbar>
    </AlertContext.Provider>
  );
};
