import React, { useContext } from 'react';
import { Route, Redirect } from 'react-router-dom';
import { AuthContext } from '../AuthContext/AuthContext';
import { Navigate } from 'react-router-dom';

const ProtectedRoute = () => {
    const { authState } = useContext(AuthContext);

    if (!authState.token) {
        return <Navigate to="/login" />;
    }

    const isAdmin = authState.role === 'Admin';
    return <Navigate to={isAdmin ? "/admin" : "/"} />;


}

export default ProtectedRoute