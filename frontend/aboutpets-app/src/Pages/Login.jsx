import React from 'react'
import LoginRegisterForm from '../Components/LoginRegisterForm/LoginRegisterForm'

const Login = ({ onLogin }) => {
    return (
        <div className="container-logging">
            <div className="left-div">
                <LoginRegisterForm isHandleRegister={false} onLogin={onLogin} />
            </div>
        </div>
    )
}

export default Login