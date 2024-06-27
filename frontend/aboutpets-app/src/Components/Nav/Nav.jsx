import React, { useState, useEffect, useContext } from 'react'
import './Nav.css'
import logo from '../Assets/logo.png'
import cart_icon from '../Assets/cart_icon.png'
import { Link } from 'react-router-dom'
import API_BASE_URL from '../../config'
import { AuthContext } from '../../AuthContext/AuthContext'
import { useNavigate } from 'react-router-dom'




const Nav = () => {

  const { authState, logout } = useContext(AuthContext);
  const [menu, setMenu] = useState("shop");
  const [orderItems, setOrderitems] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {

    const fetchOrderItems = async () => {
      try {
        const { token, role, orderId } = authState;




        if (orderId) {
          const url = new URL(`${API_BASE_URL}/order/orderItems/${orderId}`);
          const response = await fetch(url, {
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`,
              'Role': role
            },
          });
          if (!response.ok) {
            throw new Error('Failed to fetch the data!');
          }
          const data = await response.json();
          setOrderitems(data);
        }
      } catch (error) {
        console.error("Error fetching orderItems:", error);
      }
    };

    if (authState.token && authState.orderId) {
      fetchOrderItems();
    }
  }, [authState, orderItems]);



  const handleLogout = () => {
    logout();
    navigate('/')
  }


  const isAuthenticated = !!authState.token;

  const renderUserMenu = () => {
    return (
      <div className='nav'>
        <div className='nav-logo'>
          <img src={logo} alt='' />
          <p>About Pets</p>
        </div>
        <ul className='nav-menu'>
          <li onClick={() => setMenu("shop")}><Link style={{ textDecoration: 'none' }} to='/'>Shop</Link> {menu === "shop" && <hr />}</li>
          <li onClick={() => setMenu("dog")}><Link style={{ textDecoration: 'none' }} to='/dog'>Dog</Link>{menu === "dog" && <hr />}</li>
          <li onClick={() => setMenu("cat")}><Link style={{ textDecoration: 'none' }} to='/cat'>Cat</Link>{menu === "cat" && <hr />}</li>
          <li onClick={() => setMenu("action")}><Link style={{ textDecoration: 'none' }} to='/action'>Discounts</Link>{menu === "action" && <hr />}</li>
          <li onClick={() => setMenu("profile")}><Link style={{ textDecoration: 'none' }} to='/profile'>My Profile</Link>{menu === "profile" && <hr />}</li>
        </ul>
        {isAuthenticated ? (
          <div className='nav-login-cart'>
            <button onClick={handleLogout}>Logout</button>
            <Link to='/cart'><img src={cart_icon} alt='' /></Link>
            <div className='nav-cart-count'>{orderItems.length}</div>
          </div>
        ) : (
          <div className='nav-login-cart'>
            <Link to='/login'><button>Login</button></Link>
          </div>
        )}
      </div>
    )
  }

  const renderAdminMenu = () => {
    return (
      <div className='nav'>
        <div className='nav-logo'>
          <img src={logo} alt='' />
          <p>About Pets</p>
        </div>
        <ul className='nav-menu'>
          <li onClick={() => setMenu("adminDashboard")}><Link style={{ textDecoration: 'none' }} to='/admin'>Admin Dashboard</Link>{menu === "adminDashboard" && <hr />}</li>
          <li onClick={() => setMenu("manageUsers")}><Link style={{ textDecoration: 'none' }} to='/admin/users'>Manage Users</Link>{menu === "manageUsers" && <hr />}</li>
          <li onClick={() => setMenu("manageOrders")}><Link style={{ textDecoration: 'none' }} to='/admin/orders'>Manage Orders</Link>{menu === "manageOrders" && <hr />}</li>
          <li onClick={() => setMenu("manageProducts")}><Link style={{ textDecoration: 'none' }} to='/admin/products'>Manage Products</Link>{menu === "manageProducts" && <hr />}</li>
        </ul>
        {isAuthenticated && (
          <div className='nav-login-cart'>
            <button onClick={handleLogout}>Logout</button>
          </div>
        )}
      </div>
    )
  }
  if (authState?.role === 'Admin') {

    return renderAdminMenu();
  }

  return renderUserMenu();
}

export default Nav