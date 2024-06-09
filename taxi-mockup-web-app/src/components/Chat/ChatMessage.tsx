import { useEffect, useState } from "react";
import { Box, Typography } from "@mui/material";
import { Message } from "../../models/ChatModels";
import { useAuth } from "../../hooks/useAuth";

export const ChatMessage = ({ sender, message }: Message) => {
  const username = useAuth().user?.username as string;
  const [alignment, setAlignment] = useState<string>("end");

  useEffect(() => {
    if (username !== sender) {
      setAlignment("start");
    }
  }, [sender, username]);

  return (
    <Box
      sx={{
        border: "1px solid gray",
        borderRadius: "1rem",
        padding: "0.5rem 1rem",
        width: "60%",
        marginBottom: "1rem",
      }}
      alignSelf={alignment}
    >
      <Typography
        variant="h6"
        textAlign={sender === username ? "right" : "left"}
      >
        {sender}
      </Typography>
      <hr />
      <Typography variant="body1" noWrap={false}>
        {message}
      </Typography>
    </Box>
  );
};
