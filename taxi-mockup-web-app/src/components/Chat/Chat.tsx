import { useState, useEffect, useRef } from "react";
import { HubConnectionState } from "@microsoft/signalr";

import { ChatInput } from "./ChatInput";
import { ChatWindow } from "./ChatWindow";
import { Message } from "../../models/ChatModels";
import { useAlert } from "../../hooks/useAlert";
import { Divider, Paper, Typography } from "@mui/material";
import { useSignalR } from "../../hooks/useSignalR";
import { useAuth } from "../../hooks/useAuth";

export const Chat: React.FC<{ recipients: string }> = ({ recipients }) => {
  const [chatMessages, setChatMessages] = useState<Message[]>([]);
  const latestChat = useRef<Message[]>([]);
  const alert = useAlert();
  const user = useAuth().user;
  const { connection } = useSignalR();

  latestChat.current = chatMessages;

  useEffect(() => {
    if (connection) {
      connection.on("sendMessage", (sender, message) => {
        const updatedChat = [...latestChat.current];
        updatedChat.push({ sender: sender, message: message });

        setChatMessages(updatedChat);
      });
    }
  }, [connection]);

  const sendMessage = async (message: string) => {
    if (connection?.state === HubConnectionState.Connected) {
      try {
        await connection.send(
          "sendMessage",
          recipients,
          user?.username,
          message
        );
      } catch (e) {
        console.log(e);
      }
    } else {
      alert.showAlert("No connection to server yet.", "warning");
    }
  };

  return (
    <Paper
      sx={{
        padding: "1rem",
        height: "100%",
      }}
    >
      <Typography variant="h4">Chat</Typography>
      <hr />
      <ChatWindow messages={chatMessages} />
      <Divider />
      <ChatInput sendMessage={sendMessage} />
    </Paper>
  );
};
