import { Navigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import { ROLE } from "../models/AuthModel";
import { AccessDenied } from "../views/AccessDenied";

export const ProtectedRoute = ({
  children,
  role,
}: {
  children: JSX.Element;
  role: ROLE;
}) => {
  const auth = useAuth();
  if (!auth?.user) {
    return <Navigate to="/" />;
  }
  console.log(role);
  if (role === ROLE.LoggeIn) {
    return children;
  }
  if (auth?.user.userRole !== String(role)) {
    return <AccessDenied />;
  }
  return children;
};
