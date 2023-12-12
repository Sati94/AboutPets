import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import reportWebVitals from './reportWebVitals';
import { createBrowserRouter, RouterProvider, useNavigate } from "react-router-dom";
import Cookies from "js-cookie";
import { useState } from 'react';


import "./index.css";

import Login from './Pages/LoginPage/Login';
import Registering from './Pages/Register/RegisterPage';
import Header from './Components/Header/HeaderForm';
import HomePage from './Pages/HomePage/Home';


const App = () => {

  const userId = Cookies.get("userId");
  const userName = Cookies.get("userUserName");
  const userToken = Cookies.get("userToken");
  const userEmail = Cookies.get("userEmail");

  const [isLoggedIn, setIsLoggedIn] = useState(userName ? true : false);

  const handleLogin = () => {
    setIsLoggedIn(true);
  };
  const handleLogout = () => {
    setIsLoggedIn(false);
    Cookies.remove("userId");
    Cookies.remove("userUserName");
    Cookies.remove("userToken");
    Cookies.remove("userEmail");

  };

  const router = createBrowserRouter([
    {
      path: "/",
      element: (
        <Header
          isLoggedIn={isLoggedIn}
          userName={userName}
          onLogout={handleLogout}
        />
      ),
      children: [
        {
          path: "/login",
          element: <Login onLogin={handleLogin} />,
        },
        {
          path: "/register",
          element: <Registering />
        },
        {
          path: "/",
          element: <HomePage />
        }

      ],
    },
  ]);


  return (
    <React.StrictMode>
      <RouterProvider router={router} />
    </React.StrictMode>
  );


};
const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(<App />);
reportWebVitals();
