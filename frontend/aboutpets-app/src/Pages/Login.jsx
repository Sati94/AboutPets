import React, { useContext } from 'react'
import LoginRegisterForm from '../Components/LoginRegisterForm/LoginRegisterForm'
import { AuthContext } from '../AuthContext/AuthContext'

const Login = () => {
    const { login } = useContext(AuthContext);
    return (
        <div className="container-logging">
            <div className="left-div">
                <LoginRegisterForm isHandleRegister={false} onLogin={login} />
            </div>
        </div>
    )
}

export default Login