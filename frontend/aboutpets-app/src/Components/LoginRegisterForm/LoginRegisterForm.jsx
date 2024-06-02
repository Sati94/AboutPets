import React from 'react'
import { useState } from 'react'
import "./LoginRegisterForm.css"
import { jwtDecode } from 'jwt-decode'
import Cookies from 'js-cookie'
import API_BASE_URL from '../../config'
import { useNavigate, Link } from 'react-router-dom'

const LoginRegisterForm = ({ isHandleRegister, onLogin }) => {

  const [saveUserName, setSaveUserName] = useState("");
  const [savePassword, setSavePassword] = useState("");
  const [saveEmail, setSaveEmail] = useState("");
  const [error, setError] = useState("");
  const [token, setToken] = useState("");
  const navigate = useNavigate();
  const currentTime = new Date();
  const expirationTime = new Date(currentTime.getTime() + 30 * 60 * 1000);

  const handleRegister = async (e) => {
    e.preventDefault();
    console.log({ saveUserName, saveEmail, savePassword });
    try {
      const res = await fetch(`${API_BASE_URL}/Register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: saveEmail,
          username: saveUserName,
          password: savePassword,
        }),
      });

      if (res.status === 201) {
        const data = await res.json();
        console.log("Registration response:", data);
        setError("");
        navigate("/");
      } else {
        const errorData = await res.json();
        throw new Error(errorData.message || "The Email or the User Name is bad");
      }
    } catch (error) {
      console.error("Registration error:", error.message);
      setError(error.message);
    }
  }

  const handleLogin = async (e) => {
    e.preventDefault();
    console.log("Logging in...");
    try {
      const res = await fetch(`${API_BASE_URL}/Login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: saveEmail,
          password: savePassword,
        }),
      });

      const data = await res.json();
      console.log("Response data:", data);

      if (!data) {
        throw new Error(data.message || "Email or Password is bad!");
      }

      const { id, email, userName, token } = data;

      Cookies.set("userId", id, { expires: expirationTime });
      Cookies.set("userEmail", email, { expires: expirationTime });
      Cookies.set("userUserName", userName, { expires: expirationTime });
      Cookies.set("userToken", token, { expires: expirationTime });

      setToken(token);
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
        navigate("/");
      }
      onLogin();
      setError("");
    } catch (error) {
      console.error("Login error:", error.message);
      setError("Invalid Email or Password!");
    }
  };


  return (
    <div className="form">
      {!isHandleRegister ? (
        <>
          <div className="title">Please Log In!</div>
        </>

      ) : (
        <>
          <div className="title">Welcome,</div>
          <div className="subTitle">Let's create your account!</div>
        </>
      )}
      {isHandleRegister ? (
        <div className="userNameInputContainer">
          <div className="userNameInput"></div>
          <input
            id="firstname"
            className="input"
            type="text"
            placeholder="Account Name"
            onChange={(e) => setSaveUserName(e.target.value)}
          />
        </div>
      ) : (
        <></>
      )}
      <div className="userEmailInputContainer">
        <input
          id="email"
          className="input"
          type="text"
          placeholder="Email"
          onChange={(e) => setSaveEmail(e.target.value)}
        />
        <div className="place-1"></div>
      </div>

      <div className="userPasswordInputContainer">
        <input
          id="lastname"
          className="input"
          type="password"
          placeholder="Password"
          onChange={(e) => setSavePassword(e.target.value)}
        />
        <div className="place-2"></div>
      </div>
      {error && <h3 className="error">ERROR: {error}</h3>}
      {isHandleRegister ? (
        <React.Fragment>
          <button type="submit" className="submit" onClick={handleRegister}>
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
          <div className="link-account">
            Don't have an account?
            <Link to="/register"> Register</Link>
          </div>
        </>
      )}
    </div>
  )
}

export default LoginRegisterForm