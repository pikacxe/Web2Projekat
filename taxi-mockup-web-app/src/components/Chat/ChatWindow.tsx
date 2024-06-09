import React from "react";
import { ChatMessage } from "./ChatMessage";
import { Message } from "../../models/ChatModels";
import { Box, Typography } from "@mui/material";

export const ChatWindow: React.FC<{ messages: Message[] }> = ({ messages }) => {
  const chat = messages.map((m, index) => (
    <ChatMessage key={index} sender={m.sender} message={m.message} />
  ));

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        overflowY: "scroll",
        overflowX: "hidden",
        height: "630px",
      }}
    >
      {chat.length > 0 ? (
        chat
      ) : (
        <Typography variant="h5" textAlign="center" marginTop="1rem">
          You can start chating now
        </Typography>
      )}
    </Box>
  );
};
