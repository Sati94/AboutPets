import { useState } from "react";
import { useNavigate } from "react-router-dom";
import React from "react";
import { jwtDecode } from "jwt-decode";
import Cookies from "js-cookie";
import API_BASE_URL from "../../config";
import "./LogginingFormStyle.css";
import { Link } from "react-router-dom";

const LoggingForm = ({ isHandleRegister, onLogin }) => {
    const [saveUserName, setSaveUserName] = useState("");
    const [saveUserEmail, setSaveUserEmail] = useState("");
    const [saveUserPassword, setSaveUserPassword] = useState("");
    const [error, setError] = useState("");
    const [tokens, setTokens] = useState("");
    const navigate = useNavigate();
    const currentTime = new Date();
    const expirationTime = new Date(currentTime.getTime() + 30 * 60 * 1000);

    const handelRegister = (e) => {
        e.preventDefault();
        console.log("Registering...");
        console.log({ saveUserName, saveUserEmail, saveUserPassword });
        fetch(`${API_BASE_URL}/Register`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                email: saveUserEmail,
                username: saveUserName,
                password: saveUserPassword
            })
        })
            .then((res) => {
                if (res.status !== 201) {
                    console.log("Registration error", res);
                }
                else if (res.status === 400) {
                    console.log("Username or email is already taken, or password is not valid.");
                }
                return res.json();
            })
            .then((data) => {
                console.log("Registration respone :", data);
                navigate("/product/available");
            })
            .catch((error) => {
                console.log("Registration error", error.message);
            })
    };
    const handleLogin = (e) => {
        console.log("Loggigning...");
        fetch(`${API_BASE_URL}/Login`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                email: saveUserEmail,
                password: saveUserPassword
            }),
        })
            .then((res) => {
                if (res.status === 200) {
                    return res.json();
                }
                else {
                    console.log("Error :", res);
                    setError(res);
                }
            })
            .then((data) => {
                const { id, email, username, token } = data;
                console.log(data);
                Cookies.set("userId", id, { expires: expirationTime });
                Cookies.set("userEmail", email, { expires: expirationTime });
                Cookies.set("userUserName", username, { expires: expirationTime });
                Cookies.set("userToken", token, { expires: expirationTime });
                setTokens(token);
                const decodedToken = jwtDecode(token);
                if (
                    decodedToken &&
                    decodedToken[
                    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
                    ] === "Admin"
                ) {
                    // User is an admin
                    console.log("User is an admin.");
                    Cookies.set("Role", "Admin");
                    navigate("/admin");
                    onLogin();
                } else {
                    // User is not an admin
                    Cookies.set("Role", "User");
                    navigate("/product/avaiable");
                }
                onLogin();
            })
            .catch((error) => {
                console.error("Login error:", error);
            });
    };
    return (
        <>
            <div className="form">
                {!isHandleRegister ? (
                    <>
                        <div className="title2">Please Log In!</div>
                    </>
                ) : (
                    <>
                        <div className="title2">Welcome,</div>
                        <div className="subtitle">Let's create your account!</div>
                    </>
                )}
                {isHandleRegister ? (
                    <div className="input-container ic1">
                        <div className="cut"></div>
                        <input
                            id="firstname"
                            className="input"
                            type="text"
                            placeholder="user name"
                            onChange={(e) => setSaveUserName(e.target.value)}
                        />
                    </div>
                ) : (
                    <></>
                )}
                <div className="input-container ic2">
                    <input
                        id="email"
                        className="input"
                        type="text"
                        placeholder="email"
                        onChange={(e) => setSaveUserEmail(e.target.value)}
                    />
                    <div className="cut cut-short"></div>
                </div>

                <div className="input-container ic2">
                    <input
                        id="lastname"
                        className="input"
                        type="password"
                        placeholder="password"
                        onChange={(e) => setSaveUserPassword(e.target.value)}
                    />
                    <div className="cut"></div>
                </div>

                {isHandleRegister ? (
                    <React.Fragment>
                        <button type="submit" className="submit" onClick={handelRegister}>
                            Register
                        </button>
                        <div className="link-account">
                            If you already have an account, you can log in here:
                            <Link to="/login"> Login</Link>
                        </div>
                    </React.Fragment>
                ) : (
                    <>
                        <button type="submit" className="submit" onClick={handleLogin}>
                            Login
                        </button>
                        {error != null && <h1>ERROR: {error.status}</h1>}
                        <div className="link-account">
                            Don't have an account?
                            <Link to="/register"> Register</Link>
                        </div>
                    </>
                )}
            </div>
        </>
    );
};

export default LoggingForm;




