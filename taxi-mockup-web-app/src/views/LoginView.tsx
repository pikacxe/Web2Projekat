import { useEffect } from "react";
import { LoginForm } from "../components/LoginForm";
import { useAuth } from "../hooks/useAuth";
import { useNavigate } from "react-router-dom";
import { CredentialResponse, GoogleLogin } from "@react-oauth/google";
import authService from "../services/AuthService";
import { useAlert } from "../hooks/useAlert";
import { AxiosError } from "axios";

export const LoginView = () => {
  const { user, login } = useAuth();
  const alert = useAlert();
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
  async function handleSuccess(
    credentialResponse: CredentialResponse
  ): Promise<void> {
    console.log(credentialResponse);
    const { credential } = credentialResponse;
    authService
      .loginExternal(credential as string)
      .then((res) => {
        if (res) {
          console.log(res);
          login(res);
        } else {
          alert.showAlert("Could not login. Please try again later", "warning");
        }
      })
      .catch((err) => {
        if (err instanceof AxiosError) {
          alert.showAlert(err.response?.data, "error");
        } else {
          alert.showAlert(err.message, "error");
        }
      });
  }

  function handleError(): void {
    console.log("error");
  }

  return (
    <>
      <LoginForm />
      <GoogleLogin onSuccess={handleSuccess} onError={handleError} />
    </>
  );
};
