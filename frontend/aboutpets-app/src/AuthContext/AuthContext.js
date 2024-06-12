import React, { useContext, useState, useEffect, createContext } from 'react'
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
    });

    const login = (data) => {
        const { userId, userName, role, token } = data;
        const decodedToken = jwtDecode(token);


        setAuthState({
            userId: decodedToken.sub,
            email: decodedToken.email,
            userName: decodedToken.name,
            token,
            role: decodedToken.role,
        });
        localStorage.setItem('userId', userId);
        localStorage.setItem('userName', userName);
        localStorage.setItem('authToken', token);
        localStorage.setItem('role', role);


    };

    const logout = () => {
        setAuthState({
            userId: null,
            email: null,
            userName: null,
            token: null,
            role: null,
        });
        localStorage.removeItem('authToken');
    }

    useEffect(() => {
        const storedToken = localStorage.getItem('authToken');
        if (storedToken) {
            const decodedToken = jwtDecode(storedToken);

            setAuthState({
                userId: decodedToken.sub,
                email: decodedToken.email,
                userName: decodedToken.name,
                token: storedToken,
                role: decodedToken.role,
            });

            const fetchOrderId = async () => {
                try {
                    const response = await fetch(`${API_BASE_URL}/order/pending/${decodedToken.sub}`, {
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': `Bearer ${storedToken}`,
                            'Role': decodedToken.role,
                        },
                    });

                    if (!response.ok) {
                        throw new Error('Failed to fetch the data!');
                    }

                    const data = await response.json();
                    const orderId = data.orderId;
                    setAuthState(prevState => ({
                        ...prevState,
                        orderId: orderId
                    }));
                } catch (error) {
                    console.error("Error fetching orderId:", error);
                }
            };

            fetchOrderId();

        }
    }, []);

    return (
        <AuthContext.Provider value={{ authState, login, logout }}>
            {children}
        </AuthContext.Provider>
    )
}

export { AuthContext, AuthProvider };