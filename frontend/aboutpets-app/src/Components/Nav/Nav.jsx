import React, { useState } from 'react'
import './Nav.css'

import logo from '../Assets/logo.png'
import cart_icon from '../Assets/cart_icon.png'
import { Link } from 'react-router-dom'



const Nav = () => {

  const [menu, setMenu] = useState("shop");


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
        <li onClick={() => { setMenu("profile") }}><Link style={{ textDecoration: 'none' }} to='/profileId'>My Profile</Link>{menu === "profile" ? <hr /> : <></>}</li>

      </ul>
      <div className='nav-login-cart'>
        <Link to='/login' ><button>Login</button></Link>
        <Link to='/cart'><img src={cart_icon} alt='' /></Link>
        <div className='nav-cart-count'>0</div>
      </div>
    </div>
  )
}

export default Nav