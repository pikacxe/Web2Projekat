import {createContext,useContext, useMemo} from 'react';
import { useNavigate } from 'react-router-dom';
import {useLocalStorage} from './useLocalStorage';
import { AuthResponse } from '../models/User/UserModel';
import { AuthContext } from '../models/AuthModel';
import React from 'react';


const AuthorizationContext = createContext<AuthContext | null>(null);

export const AuthProvider : React.FC<{children:React.ReactNode}>=({children}) => {
    const [user,setUser] = useLocalStorage("user",null);
    const navigate = useNavigate();


    const login = async (data:AuthResponse) =>{
        setUser(data);
        switch(data.userRole){
            case "User": navigate("/dashboard");break;
            case "Driver": navigate("/driver-dashboard");break;
            case "Admin": navigate("/admin-dashboard");break;
            default: navigate("/");
        }
    }

    const logout = async () =>{
        setUser(null);
        navigate("/", {replace:true});
    }
    
    const value = useMemo(
        () => ({
            user,
            login,
            logout,
        }),
        [user]
    );

    return <AuthorizationContext.Provider value={value}>{children}</AuthorizationContext.Provider>
};

export const useAuth = () =>{
    return useContext(AuthorizationContext);
}


