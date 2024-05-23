import {Navigate} from 'react-router-dom'
import {useAuth} from '../hooks/useAuth'

export const ProtectedRoute:  React.FC<{children:any}> = ({children}) => {
    const auth = useAuth();
    if(!auth?.user){
        return <Navigate to="/"/>
    }
    return children;
}