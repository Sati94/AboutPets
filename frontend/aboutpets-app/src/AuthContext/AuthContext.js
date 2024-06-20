import React, { useState, useEffect, createContext } from 'react'
import { jwtDecode } from 'jwt-decode'
import API_BASE_URL from '../config';


const AuthContext = createContext();
const AuthProvider = ({ children }) => {


    const [authState, setAuthState] = useState({

        userId: null,
        email: null,
        userName: null,
        token: null,
        role: null,
        orderId: null
    });

    const login = (data) => {
        const { token } = data;
        const decodedToken = jwtDecode(token);


        const userData = {
            userId: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
            email: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
            userName: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
            token,
            role: decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
            orderId: null
        };
        setAuthState(userData);


        localStorage.setItem('userData', JSON.stringify(userData));


    };

    const logout = () => {
        setAuthState({
            userId: null,
            email: null,
            userName: null,
            token: null,
            role: null,
            orderId: null
        });

        localStorage.removeItem('userData');
    }

    const fetchOrderId = async () => {
        try {
            const response = await fetch(`${API_BASE_URL}/order/pending/${authState.userId}`, {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${authState.token}`,
                    'Role': authState.role,
                },
            });

            if (!response.ok) {
                throw new Error('Failed to fetch the data!');
            }

            const data = await response.json();
            const orderId = data.orderId;
            setAuthState(prevState => ({
                ...prevState,
                orderId: orderId // orderId beállítása az authState-ben
            }));
        } catch (error) {
            console.error("Error fetching orderId:", error);
        }
    };

    useEffect(() => {
        const storedData = localStorage.getItem('userData');
        if (storedData) {
            const userData = JSON.parse(storedData);
            setAuthState(userData);
            if (userData.userId !== null && userData.token !== null && userData.role !== null) {
                fetchOrderId();
            }
        }
    }, []);



    return (
        <AuthContext.Provider value={{ authState, login, logout, fetchOrderId }}>
            {children}
        </AuthContext.Provider>
    )
}

export { AuthContext, AuthProvider };