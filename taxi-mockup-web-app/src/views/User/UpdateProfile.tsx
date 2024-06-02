import { useParams } from "react-router-dom";
import { UserForm } from "../../components/User/UserForm";

export const UpdateProfileView = () => {
  const { userId } = useParams();
  return <UserForm userId={userId} />;
};
