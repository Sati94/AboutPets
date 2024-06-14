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
        });

        localStorage.removeItem('userData');
    }

    useEffect(() => {
        const storedData = localStorage.getItem('userData');
        if (storedData) {
            const userData = JSON.parse(storedData);
            setAuthState(userData);
            console.log(userData)
            if (userData.userId !== null && userData.token !== null && userData.role !== null) {

                const fetchOrderId = async () => {
                    try {
                        const response = await fetch(`${API_BASE_URL}/order/pending/${userData.userId}`, {
                            headers: {
                                'Content-Type': 'application/json',
                                'Authorization': `Bearer ${userData.token}`,
                                'Role': userData.role,
                            },
                        });

                        if (!response.ok) {
                            throw new Error('Failed to fetch the data!');
                        }

                        const data = await response.json();
                        const orderId = data.orderId;
                        setAuthState(prevState => ({
                            ...prevState,
                            orderId: orderId,
                        }));
                    } catch (error) {
                        console.error("Error fetching orderId:", error);
                    }
                }
                fetchOrderId();
            }
        }
    }, []);



    return (
        <AuthContext.Provider value={{ authState, login, logout }}>
            {children}
        </AuthContext.Provider>
    )
}

export { AuthContext, AuthProvider };