import React, { useState, useEffect } from 'react'
import './Nav.css'
import logo from '../Assets/logo.png'
import cart_icon from '../Assets/cart_icon.png'
import { Link } from 'react-router-dom'



const Nav = ({ isLoggedIn, userName, userId, onLogout }) => {

  const [menu, setMenu] = useState("shop");
  const [isAuthenticated, setIsAuthenticated] = useState(isLoggedIn)
  useEffect(() => {
    setIsAuthenticated(isLoggedIn);

  }, [isLoggedIn])


  return (
    <div className='nav'>
      <div className='nav-logo'>
        <img src={logo} alt='' ></img>
        <p>About Pets</p>
      </div>
      <ul className='nav-menu'>
        <li onClick={() => { setMenu("shop") }}><Link style={{ textDecoration: 'none' }} to='/'>Shop</Link> {menu === "shop" ? <hr /> : <></>}</li>
        <li onClick={() => { setMenu("dog") }}><Link style={{ textDecoration: 'none' }} to='/dog'>Dog</Link>{menu === "dog" ? <hr /> : <></>}</li>
        <li onClick={() => { setMenu("cat") }}><Link style={{ textDecoration: 'none' }} to='/cat'>Cat</Link>{menu === "cat" ? <hr /> : <></>}</li>
        <li onClick={() => { setMenu("action") }}><Link style={{ textDecoration: 'none' }} to='/action'>Actions</Link>{menu === "action" ? <hr /> : <></>}</li>
        <li onClick={() => { setMenu("profile") }}><Link style={{ textDecoration: 'none' }} to='/profile'>My Profile</Link>{menu === "profile" ? <hr /> : <></>}</li>

      </ul>
      {isAuthenticated ? (
        <div className='nav-login-cart'>
          <button onClick={onLogout}>Logout</button>
          <Link to='/cart'><img src={cart_icon} alt='' /></Link>
          <div className='nav-cart-count'>0</div>
        </div>
      ) : (
        <div className='nav-login-cart'>
          <Link to='/login'><button>Login</button></Link>
        </div>
      )}

    </div>
  )
}

export default Nav