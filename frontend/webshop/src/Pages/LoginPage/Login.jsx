import React from "react";
import LoggingForm from "../../Components/LogginingForm/LoginForm";


const Login = ({ onLogin }) => {
    return (
        <div className="containerLogin">
            <div className="left-div">
                <LoggingForm isHandleRegister={false} onLogin={onLogin} />
            </div>
        </div>
    )
}

export default Login;