import { useEffect } from "react";
import { LoginForm } from "../components/LoginForm";
import { useAuth } from "../hooks/useAuth";
import { useNavigate } from "react-router-dom";

export const LoginView = () => {
  const user = useAuth().user;
  const navigate = useNavigate();
  useEffect(() => {
    if (user) {
      if (user.token) {
        switch (user.userRole) {
          case "Driver":
            navigate("/driver-dashboard");
            break;
          case "User":
            navigate("/dashboard");
            break;
          case "Admin":
            navigate("/admin-dashboard");
            break;
        }
      }
    }
  });
  return <LoginForm />;
};
