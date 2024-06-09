import { Button, FormControl, Stack, TextField } from "@mui/material";
import React, { ChangeEvent, FormEvent, useState } from "react";
import { useAlert } from "../../hooks/useAlert";

export const ChatInput = (props: any) => {
  const [message, setMessage] = useState("");
  const alert = useAlert();

  const onSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const isMessageProvided = message && message !== "";

    if (isMessageProvided) {
      props.sendMessage(message);
      setMessage("");
    } else {
      alert.showAlert("Please provide a message first", "warning");
    }
  };

  const onMessageUpdate = (
    e: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    setMessage(e.target.value);
  };

  return (
    <form onSubmit={onSubmit}>
      <Stack direction="row" gap={2} padding="0.5rem">
        <FormControl>
          <TextField
            value={message}
            placeholder="Your message"
            onChange={onMessageUpdate}
          />
        </FormControl>
        <Button variant="contained" type="submit">
          Send
        </Button>
      </Stack>
    </form>
  );
};
