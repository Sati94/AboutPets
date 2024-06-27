import React, { useState, createContext } from 'react'
import { jwtDecode } from 'jwt-decode'
import Cookies from 'js-cookie';
import API_BASE_URL from '../config';



const AuthContext = createContext();
const AuthProvider = ({ children }) => {


    const [authState, setAuthState] = useState({

        userId: null,
        email: null,
        userName: null,
        token: null,
        role: null,
        orderId: Cookies.get('orderId') || null,
    });

    const fetchOrderId = async (userId, role, token) => {
        if (role === 'User') {
            try {
                const response = await fetch(`${API_BASE_URL}/order/pending/${userId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role,
                    },
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch the data!');
                }

                const data = await response.json();
                const orderId = data.orderId !== null ? data.orderId : 0;

                Cookies.set('orderId', orderId);

                return orderId;

            } catch (error) {
                console.error("Error fetching OrderId:", error);
                Cookies.set('orderId', 0);
                return 0;
            }
        }
    };

    const login = async (data) => {
        const { token } = data;
        const decodedToken = jwtDecode(token);


        const userData = {
            userId: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
            email: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
            userName: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
            token,
            role: decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],

        };

        setAuthState(userData);

        localStorage.setItem('userData', JSON.stringify(userData));
        try {
            const orderId = await fetchOrderId(userData.userId, userData.role, userData.token);
            setAuthState(prevState => ({
                ...prevState,
                orderId: orderId,
            }));
        } catch (error) {
            console.error("Error fetching orderId:", error);
            setAuthState(prevState => ({
                ...prevState,
                orderId: 0,
            }));
        }

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
        Cookies.remove("orderId");
    }


    return (
        <AuthContext.Provider value={{ authState, setAuthState, login, logout }}>
            {children}
        </AuthContext.Provider>
    )
}

export { AuthContext, AuthProvider };