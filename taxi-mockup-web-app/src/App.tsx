import "./App.css";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import { LoginView } from "./views/LoginView";
import { RegisterView } from "./views/RegisterView";
import { UserDashboard } from "./views/User/UserDashboard";
import { AdminDashboard } from "./views/Admin/AdminDashboard";
import { DriverDashboard } from "./views/Driver/DriverDashboard";
import { Navbar } from "./components/Navbar";
import { AuthProvider } from "./hooks/useAuth";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { ROLE } from "./models/AuthModel";
import { AlertProvider } from "./hooks/useAlert";
import { UpdateProfileView } from "./views/User/UpdateProfile";
import { SignalRProvider } from "./hooks/useSignalR";
import { ChangePasswordForm } from "./components/User/ChangePasswordForm";
import { FullScreenDialog } from "./components/FullScreenDialog";

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <AuthProvider>
          <AlertProvider>
            <SignalRProvider>
              <Navbar />
              <Routes>
                <Route path="/" element={<LoginView />} />
                <Route path="/register" element={<RegisterView />} />
                <Route
                  path="/dashboard"
                  element={
                    <ProtectedRoute role={ROLE.User}>
                      <UserDashboard />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/admin-dashboard"
                  element={
                    <ProtectedRoute role={ROLE.Admin}>
                      <AdminDashboard />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/driver-dashboard"
                  element={
                    <ProtectedRoute role={ROLE.Driver}>
                      <DriverDashboard />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/:dashboard/:userId"
                  element={
                    <ProtectedRoute role={ROLE.LoggeIn}>
                      <UpdateProfileView />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/change-password"
                  element={
                    <ProtectedRoute role={ROLE.LoggeIn}>
                      <ChangePasswordForm />
                    </ProtectedRoute>
                  }
                />
                <Route
                  path="/ride-in-progress"
                  element={
                    <ProtectedRoute role={ROLE.LoggeIn}>
                      <FullScreenDialog />
                    </ProtectedRoute>
                  }
                />
              </Routes>
            </SignalRProvider>
          </AlertProvider>
        </AuthProvider>
      </BrowserRouter>
    </div>
  );
}

export default App;
