import React from "react";
import LoggingForm from "../../Components/LogginingForm/LoginForm";

function Registering() {
    return (
        <div className="container-logging">
            <div className="left-div">
                <LoggingForm isHandleRegister={true} />
            </div>
        </div>
    )
};

export default Registering;
