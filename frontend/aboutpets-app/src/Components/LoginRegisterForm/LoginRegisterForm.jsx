import React, { useState, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import API_BASE_URL from '../../config';
import { AuthContext } from '../../AuthContext/AuthContext';
import './LoginRegisterForm.css';
import Footer from '../FooterItem/Footer';

const LoginRegisterForm = ({ isHandleRegister }) => {
  const { login } = useContext(AuthContext);
  const navigate = useNavigate();
  const [saveUserName, setSaveUserName] = useState('');
  const [saveEmail, setSaveEmail] = useState('');
  const [savePassword, setSavePassword] = useState('');
  const [errors, setErrors] = useState({});

  const validateEmail = (email) => {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(String(email).toLowerCase());
  };

  const validatePassword = (password) => {
    return password.length >= 6;
  };

  const handleLogin = async (e) => {
    e.preventDefault();
    let valid = true;
    let newErrors = {};

    if (!validateEmail(saveEmail)) {
      newErrors.email = 'Invalid email format';
      toast.error('Invalid email format');
      valid = false;
    }
    if (!validatePassword(savePassword)) {
      newErrors.password = 'Password must be at least 6 characters long';
      toast.error('Password must be at least 6 characters long');
      valid = false;
    }

    setErrors(newErrors);

    if (!valid) {
      return;
    }

    try {
      const res = await fetch(`${API_BASE_URL}/Login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email: saveEmail,
          password: savePassword,
        }),
      });
      const data = await res.json();
      if (!data.token) {
        toast.error('Email or Password is incorrect!');
        return;
      }
      login(data);
      if (data.role === 'Admin') {
        navigate('/admin', { state: { message: 'Login was successful as Admin!' } });
      } else {
        navigate('/', { state: { message: 'Login was successful as User!' } });
      }
    } catch (error) {
      toast.error('Email or Password is incorrect!');
    }
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    let valid = true;
    let newErrors = {};

    if (!saveUserName) {
      newErrors.userName = 'Account Name is required';
      toast.error('Account Name is required');
      valid = false;
    }
    if (!validateEmail(saveEmail)) {
      newErrors.email = 'Invalid email format';
      toast.error('Invalid email format');
      valid = false;
    }
    if (!validatePassword(savePassword)) {
      newErrors.password = 'Password must be at least 6 characters long';
      toast.error('Password must be at least 6 characters long');
      valid = false;
    }

    setErrors(newErrors);

    if (!valid) {
      return;
    }

    try {
      const res = await fetch(`${API_BASE_URL}/Register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email: saveEmail,
          username: saveUserName,
          password: savePassword,
        }),
      });

      if (res.status === 201) {
        const data = await res.json();
        setErrors({});
        toast.success('Registration successful!');
        navigate('/login');
      } else {
        const errorData = await res.json();
        toast.error('The User Name or the Email is taken!');
        throw new Error(errorData.message || 'The Email or the User Name is bad');
      }
    } catch (error) {
      console.error('Registration error:', error.message);
      toast.error('Registration error: ' + error.message);
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
      {isHandleRegister && (
        <div className="userNameInputContainer">
          <input
            id="userName"
            className={`input ${errors.userName ? 'error-input' : ''}`}
            type="text"
            placeholder="Account Name"
            onChange={(e) => setSaveUserName(e.target.value)}
          />
          {errors.userName && <div className="error">{errors.userName}</div>}
        </div>
      )}
      <div className="userEmailInputContainer">
        <input
          id="email"
          className={`input ${errors.email ? 'error-input' : ''}`}
          type="text"
          placeholder="Email"
          onChange={(e) => setSaveEmail(e.target.value)}
        />
        {errors.email && <div className="error">{errors.email}</div>}
      </div>
      <div className="userPasswordInputContainer">
        <input
          id="password"
          className={`input ${errors.password ? 'error-input' : ''}`}
          type="password"
          placeholder="Password"
          onChange={(e) => setSavePassword(e.target.value)}
        />
        {errors.password && <div className="error">{errors.password}</div>}
      </div>
      {isHandleRegister ? (
        <>
          <button type="submit" className="submit" onClick={handleRegister}>
            Register
          </button>
          <div className="link-account">
            If you already have an account, you can log in here:
            <Link to="/login"> Login</Link>
          </div>
        </>
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
  );
};

export default LoginRegisterForm;