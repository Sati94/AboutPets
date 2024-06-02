import React from 'react'
import LoginRegisterForm from '../Components/LoginRegisterForm/LoginRegisterForm'

const Register = () => {
    return (
        <div className="container-logging">
            <div className="left-div">
                <LoginRegisterForm isHandleRegister={true} />
            </div>
        </div>
    )
}

export default Register