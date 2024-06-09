import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
} from "react";
import * as signalR from "@microsoft/signalr";
import { useAuth } from "./useAuth";

const hubUrl =
  process.env.REACT_APP_HUB_URL ?? "http://localhost:8621/hubs/ride";

interface SignalRContextType {
  connection: signalR.HubConnection | null;
}

const SignalRContext = createContext<SignalRContextType | undefined>(undefined);

export const useSignalR = (): SignalRContextType => {
  const context = useContext(SignalRContext);
  if (context === undefined) {
    throw new Error("useSignalR must be used within a SignalRProvider");
  }
  return context;
};

interface SignalRProviderProps {
  children: ReactNode;
}

export const SignalRProvider: React.FC<SignalRProviderProps> = ({
  children,
}) => {
  const user = useAuth().user;
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null
  );

  useEffect(() => {
    if (connection?.state !== signalR.HubConnectionState.Connected) {
      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, { accessTokenFactory: () => user?.token as string })
        .withAutomaticReconnect()
        .build();

      setConnection(newConnection);
      return () => {
        if (newConnection) {
          newConnection.stop();
        }
      };
    }
  }, [user?.token, connection?.state]);

  const value = { connection };

  return (
    <SignalRContext.Provider value={value}>{children}</SignalRContext.Provider>
  );
};
