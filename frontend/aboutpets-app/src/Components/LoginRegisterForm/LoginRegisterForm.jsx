import React from 'react'
import { useState, useEffect } from 'react'
import "./LoginRegisterForm.css"
import { jwtDecode } from 'jwt-decode'
import Cookies from 'js-cookie'
import API_BASE_URL from '../../config'
import { useNavigate, Link } from 'react-router-dom'
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/ReactToastify.css'

const LoginRegisterForm = ({ isHandleRegister, onLogin }) => {

  const [saveUserName, setSaveUserName] = useState("");
  const [savePassword, setSavePassword] = useState("");
  const [saveEmail, setSaveEmail] = useState("");
  const [error, setError] = useState("");
  const [userToken, setUserToken] = useState("");
  const [id, setId] = useState("");
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
        navigate("/", { state: { message: 'Registering was successfully!' } });

      } else {
        const errorData = await res.json();
        toast.error('The User Name or the Email is taken!');
        throw new Error(errorData.message || "The Email or the User Name is bad");
      }

    } catch (error) {
      console.error("Registration error:", error.message);
      setError(error.message);
    }
  }

  const handleLogin = async (e) => {
    e.preventDefault();

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
      const dataId = data.userId;
      console.log(dataId);
      const dataToken = data.token;
      console.log(dataToken)
      setUserToken(dataToken);
      setId(dataId);
      console.log("Token set in state:", userToken);
      console.log("UserId", id)

      if (!data) {
        toast.error('Email or Password is bad!');
        throw new Error(data.message || "Email or Password is bad!");
      }

      const { userId, email, userName, token } = data;

      Cookies.set("userId", userId, { expires: expirationTime });
      Cookies.set("userEmail", email, { expires: expirationTime });
      Cookies.set("userUserName", userName, { expires: expirationTime });
      Cookies.set("userToken", token, { expires: expirationTime });

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
        navigate("/admin", { state: { message: ('Login was successfully as Admin!') } });
        onLogin();

      } else {
        // User is not an admin
        console.log("User is an user.")
        Cookies.set("Role", "User");
        navigate("/", { state: { message: "Login was successfull as User!" } });
      }
      onLogin(data.userId, data.token, data.role);
      getOrderId();

      setError("");

    } catch (error) {
      toast.error('Email or Password is bad!');

      console.error("Login error:", error.message);
      setError("Invalid Email or Password!");
    }
  };


  const getOrderId = async () => {

    const userId = Cookies.get('userId');
    const userRole = Cookies.get('Role');
    const token = Cookies.get('userToken');

    try {

      const response = await fetch(`${API_BASE_URL}/order/pending/${userId}`, {
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
          'Role': userRole,
        },
      });

      if (!response.ok) {
        throw new Error('Failed to fetch the data!');
      }

      const data = await response.json();
      console.log(data);
      const orderId = data.orderId;
      if (orderId !== null) {
        Cookies.set("orderId", orderId);
      }
      else {
        toast.info("You don't have got any Order now!")
      }

      console.log(document.cookie);
    } catch (error) {
      console.error("Error fetching OrderId:", error);
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
      <ToastContainer />
    </div>

  )
}

export default LoginRegisterForm