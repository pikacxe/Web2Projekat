import './App.css';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { LoginView } from './views/LoginView';
import { RegisterView } from './views/RegisterView';
import { UserDashboard } from './views/User/UserDashboard';
import { AdminDashboard } from './views/Admin/AdminDashboard';
import { DriverDashboard } from './views/Driver/DriverDashboard';
import { Navbar } from './components/Navbar';

function App() {
  return (
    <div className='App'>
    <BrowserRouter>
      <Navbar/>
      <Routes>
        <Route path='/' element={<LoginView/>}/>
        <Route path='/register' element={<RegisterView/>}/>
        <Route path='/dashboard' element={<UserDashboard/>}/>
        <Route path='/admin-dashboard' element={<AdminDashboard/>}/>
        <Route path='/driver-dashboard' element={<DriverDashboard/>}/>
      </Routes>
    </BrowserRouter>
    </div>
  );
}

export default App;
